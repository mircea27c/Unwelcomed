using UnityEngine;

public class bulletScript : MonoBehaviour
{
    [SerializeField] float speed;
    // Update is called once per frame
    void Update()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime * 10f;
        if (Physics.Raycast(transform.position, transform.forward, 1f)) {
            gameObject.SetActive(false);
        }
    }
}
