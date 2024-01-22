using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public bool collectingOres;
    public void OnCollisionEnter(Collision collision)
    {
        if (!collectingOres) return;

        if (collision.gameObject.CompareTag("Ore"))
        {
            collision.gameObject.GetComponent<oreScript>().collect();
        }
    }
}
