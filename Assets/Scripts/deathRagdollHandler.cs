using UnityEngine;

public class deathRagdollHandler : MonoBehaviour
{
    [SerializeField]Rigidbody[] partsToApplyForce;
    [SerializeField] float forcePower;

    [SerializeField] float timeBeforeFreezing;
    float freezeTimer;

    [SerializeField]GameObject[] allParts;


    public Renderer meshRenderer;
    private void Update()
    {
        freezeTimer += Time.deltaTime;
        if (freezeTimer >= timeBeforeFreezing) {
            freeze();
        }
    }

    public void setScale(float factor) {
        foreach (GameObject go in allParts)
        {
            go.GetComponent<Rigidbody>().mass *= factor;
        }

        transform.localScale = Vector3.one * factor;
        reconfigure();
    }

    private void Awake()
    {
        reconfigure();
    }

    private void Start()
    {
        int index = Random.Range(0, partsToApplyForce.Length);
        partsToApplyForce[index].AddForce((transform.forward * -1 + Vector3.up) * forcePower, ForceMode.Impulse);
    }

    void reconfigure() {
        foreach (GameObject go in allParts)
        {
            CharacterJoint joint = go.GetComponent<CharacterJoint>();
            if (joint != null)
            {
             joint.autoConfigureConnectedAnchor = true;
            }
        }
    }
    void freeze() {
        foreach (GameObject go in allParts)
        {
            Destroy(go.GetComponent<CharacterJoint>());
            Destroy(go.GetComponent<Rigidbody>());
            Destroy(go.GetComponent<Collider>());
        }
        Destroy(this);
    }
}
