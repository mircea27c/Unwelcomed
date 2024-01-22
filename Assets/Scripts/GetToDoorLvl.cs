using UnityEngine;

public class GetToDoorLvl : GetToDestinationLevel
{
    [SerializeField] Animator doorAnim;
    public override void onLevelEnd()
    {
        doorAnim.Play("Open");

        base.onLevelEnd();
    }

}
