using System.Collections.Generic;
using System.Collections;

using UnityEngine;

public class Boomerang : Weapon
{

    bool hasBoomerang = true;
    [SerializeField] Transform goParent;
    [SerializeField] Transform destination;
    [SerializeField] float speed;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject trailEffect;

    //calculul traiectoriei
    float initialDistance;

    //damage inamicilor
    List<Collider> ignoredCollisions;

    Rigidbody rb;

    enum state { neutral, leaving, returning };
    state status = state.neutral;
    public override void onLeftClick(WeaponsManager plr)
    {
        if (!hasBoomerang) { return; }

        Vector3 shootDirection;
        if (shootRay(plr, out RaycastHit hit))
        {
            shootDirection = hit.point;
        }
        else
        {
            shootDirection = plr.fpsCamera.position + plr.fpsCamera.forward * 20f;
        }

        status = state.leaving;
        destination.parent = null;
        throwAt(shootDirection);
    }

    private void Awake()
    {
        animator = go.GetComponent<Animator>();
        rb = go.GetComponent<Rigidbody>();
        ignoredCollisions = new List<Collider>();


        trailEffect.SetActive(false);
    }

    private void Update()
    {
        if (!hasBoomerang)
        {

            if (Vector3.Distance(go.transform.position, destination.position) < 0.125f * speed)
            {
                if (status == state.leaving)
                {
                    callback();
                }
                else if (status == state.returning)
                {
                    resetBoomerang();
                }
                else
                {
                    resetBoomerang();
                }
            }
            else
            {
                checkForEnemies();
                calculateTrajectory();
            }


        }
    }

    //--state handling (throwing/returning)
    public void callback()
    {
        status = state.returning;

        rb.velocity = Vector3.zero;

        destination.parent = transform;
        throwAt(transform.position);
    }
    private void resetBoomerang()
    {

        trailEffect.SetActive(false);

        animator.SetBool("rotating", false);

        status = state.neutral;
        hasBoomerang = true;

        rb.velocity = Vector3.zero;
        go.transform.parent = goParent;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localPosition = Vector3.zero;

        rb.isKinematic = true;

    }
    void throwAt(Vector3 position)
    {

        trailEffect.SetActive(true);
        animator.SetBool("rotating", true);

        destination.position = position;
        initialDistance = Vector3.Distance(go.transform.position, destination.position);

        hasBoomerang = false;
        go.transform.parent = null;

        rb.isKinematic = false;
        go.transform.LookAt(destination);
    }
    void calculateTrajectory()
    {
        //va calcula traiectoria astfel incat boomerangul sa aiba un traseu circular indiferent
        //de destinatie

        go.transform.LookAt(destination);

        float distance = Vector3.Distance(go.transform.position, destination.position);
        float zValue = distance / initialDistance;
        float yValue;

        if (status == state.returning)
        {
            yValue = 0;
        }
        else { 
            yValue = (zValue - 0.4f) * 0.5f;
        }

        Vector3 dir = new Vector3(zValue - 0.4f, yValue, 1f);


        rb.velocity = go.transform.TransformDirection(dir * speed);
        Debug.DrawRay(go.transform.position, rb.velocity.normalized, Color.blue);
    }

    //----dealing damage

    void checkForEnemies() {
        Collider[] hits = Physics.OverlapSphere(go.transform.position, 0.6f, enemyLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!ignoredCollisions.Contains(hits[i]))
            {
                hits[i].gameObject.GetComponent<EnemyController>()?.takeDamage(damage);
            }
        }
        
        ignoredCollisions.Clear();
        for (int i = 0; i < hits.Length; i++)
        {
            ignoredCollisions.Add(hits[i]);
        }
    }
}
