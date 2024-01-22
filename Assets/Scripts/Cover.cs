using UnityEngine;

public class Cover : MonoBehaviour
{
    //setup
    public Transform coverPoint;
    public Transform shootingPoint;
    public enum coverType { horizontal, vertical };
    [SerializeField] coverType type;
    //data
    [HideInInspector]public Vector3 coverPosition;
    [HideInInspector] public Vector3 shootingPosition;
    [HideInInspector] public bool busy;

    private void Awake()
    {
        //setting up
        coverPosition = coverPoint.position;
        if (type == coverType.horizontal)
        {
            shootingPosition = coverPosition;
        }
        else {
            shootingPosition = shootingPoint.position;
        }
    }
}
