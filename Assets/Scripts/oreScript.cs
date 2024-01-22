using UnityEngine;

public class oreScript : MonoBehaviour
{
    public GatherMetalsLevel.metalType type;
    
    public bool active;
    bool detached;

    bool collecting;

    Rigidbody rb;
    private void Awake()
    {

       rb = GetComponent<Rigidbody>();
       rb.isKinematic = true;
       rb.useGravity = false;
    }

    public void takeDamage() {
        if (!active) return;

        rb.isKinematic = false;
        rb.useGravity = true;

        detached = true;
    }
    private void Update()
    {
        if (collecting) {
            collectAnimation();
        }
    }

    void collectAnimation() {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.01f, 0.02f);
        transform.position += Vector3.up * 0.02f;
        if (transform.localScale.x <= 0.02f) {
            collecting = false;
            gameObject.SetActive(false);
        }
    }
    public void collect() {
        if (!detached) return;
        rb.useGravity = false;
        rb.isKinematic = true;
        collecting = true;
        GatherMetalsLevel.instance?.addMetal(type);
    }
}
