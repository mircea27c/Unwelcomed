using UnityEngine;

public class PlasmaContainer : MonoBehaviour
{
    public delegate void onPickUpDelegate();
    public onPickUpDelegate onPickUp;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            pickUp();
        }   
    }
    void pickUp() {
        if (onPickUp != null)
        {
            onPickUp();
        }

        Destroy(gameObject);
    }
}
