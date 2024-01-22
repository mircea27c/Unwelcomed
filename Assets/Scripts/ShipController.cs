using UnityEngine;

public class ShipController : MonoBehaviour
{
    public bool active;
    Rigidbody rb;
    [SerializeField] Transform shipGfx;
    [SerializeField]cameraShake shipShake;

    [SerializeField] Transform destination;
    [SerializeField] float speed;
    [SerializeField] float steerForce;

    Vector3 initialTrajectory;

    [SerializeField] float maxSteerHorizontal;
    [SerializeField] float maxSteerVertical;

    float horizontalSteer;
    float verticalSteer;

    Quaternion targetRotation;

    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip hitSfx;

    public delegate void shipMoveDelegate();
    public shipMoveDelegate onShipMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void setActiveControl() {
        active = true;
        initialTrajectory = transform.forward;
        rb.isKinematic = false;
    }
    public void stopMovement()
    {
        active = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

    }
    private void Update()
    {
        if (!active)
            return;

        processInput();
    }
    void processInput() {
        

        horizontalSteer = Input.GetAxis("Horizontal") * steerForce;
        verticalSteer = Input.GetAxis("Vertical") * steerForce;

        if (horizontalSteer != 0 || verticalSteer != 0) {
            if (onShipMove != null) {
                onShipMove();
            }
        }

        float difH = transform.position.z - destination.position.z;
        float difV = transform.position.y - destination.position.y;

        targetRotation = Quaternion.Euler(new Vector3(horizontalSteer, 0f, -verticalSteer));

        shipGfx.rotation = Quaternion.Lerp(shipGfx.rotation, targetRotation, 0.05f);

        if (difH >= maxSteerHorizontal) {
            if (horizontalSteer > 0) {
                horizontalSteer = 0;
            }
        }
        else if (difH <= -maxSteerHorizontal)
        {
            if (horizontalSteer < 0)
            {
                horizontalSteer = 0;
            }
        }

        if (difV >= maxSteerVertical)
        {
            if (verticalSteer > 0)
            {
                verticalSteer = 0;
            }
        }
        else if (difV <= -maxSteerVertical)
        {
            if (verticalSteer < 0)
            {
                verticalSteer = 0;
            }
        }

        rb.velocity = transform.forward * horizontalSteer + transform.right * -speed + transform.up*verticalSteer;
    }

    void getHit() {
        shipShake.shakeDuration = 0.7f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Meteor")) {
            getHit();
            Destroy(collision.gameObject);

            PlayHitSfx();
        }
    }

    void PlayHitSfx() {
        audioPlayer.PlayOneShot(hitSfx);
    }
}
