using UnityEngine;

public class GravityGunWeapon : Weapon
{
    [SerializeField] GameObject pref_rock;
    [SerializeField] Transform pullPoint;
    [SerializeField] float pushForce;

    Rigidbody projectile;
    bool hasProjectile;
    bool pulling;

    private void Update()
    {
        if (pulling)
        {

            print("pulling");
            if (Vector3.Distance(projectile.position, pullPoint.position) > 0.1f)
            {
                pullProj();
            }
            else
            {
                attachProj();
            }
        }
    }

    void pullProj()
    {
        projectile.position = Vector3.Slerp(projectile.position, pullPoint.position, 10f * Time.deltaTime);
    }
    void attachProj()
    {
        projectile.transform.SetParent(pullPoint);
        projectile.transform.localPosition = Vector3.zero;
        pulling = false;
        hasProjectile = true;
        print("attach");
    }
    void shootProj(WeaponsManager plr)
    {
        if (!hasProjectile || projectile == null) { hasProjectile = false; return; }
        projectile.useGravity = true;
        projectile.isKinematic = false;
        projectile.transform.parent = null;

        if (shootRay(plr, out RaycastHit hit))
        {
            print("hit " + hit.transform.gameObject.name);
            projectile.AddForce((hit.point - projectile.position).normalized * pushForce * projectile.mass);
        }
        else {
            print("hit nothing");
            projectile.AddForce(plr.fpsCamera.transform.forward* pushForce * projectile.mass);
        }

        hasProjectile = false;
        projectile = null;
    }
    public override void onRightClick(WeaponsManager plr)
    {
        if (shootRay(plr, out RaycastHit hit))
        {
            projectile = Instantiate(pref_rock, hit.point, Quaternion.identity).GetComponent<Rigidbody>();
            projectile.useGravity = false;
            projectile.isKinematic = true;

            pulling = true;
        }
    }
    public override void onLeftClick(WeaponsManager plr)
    {
        if (hasProjectile)
        {
            shootProj(plr);
        }
    }

}
