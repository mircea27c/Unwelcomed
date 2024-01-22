using UnityEngine;

public class GetInTheShipLevel : GetToDestinationLevel
{
    [SerializeField] Transform shipTransform;
    public override void onLevelEnd()
    {
        PlayerManager.instance.transform.parent = shipTransform;
        PlayerManager.instance.transform.localPosition = Vector3.zero;
        base.onLevelEnd();
    }
}
