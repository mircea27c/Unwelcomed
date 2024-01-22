using UnityEngine;

public class faceCamera : MonoBehaviour
{
    Transform cam;
    Transform thisTransform;
    private void Awake()
    {
        cam = FindObjectOfType<Camera>().transform;
        thisTransform = transform;
    }
    private void Update()
    {
        if (cam != null) {
            thisTransform.LookAt(cam);
        }
    }
}
