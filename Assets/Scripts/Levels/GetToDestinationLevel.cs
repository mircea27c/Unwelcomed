using UnityEngine;
public class GetToDestinationLevel : Level
{
    public Transform destination;
    [SerializeField] float destinationRange;

    [SerializeField] string objectiveText;

    Transform playerTransform;
    public bool completed = false;

    public delegate void restartDelegate();
    public restartDelegate onRestart;
    public override void startLevel()
    {
        base.startLevel();
        playerTransform = FindObjectOfType<PlayerManager>().transform;

        if (!string.IsNullOrEmpty(objectiveText))
        {
            HUDManager.instance.updateObjective(objectiveText);
        }

        HUDManager.instance.setDestination(destination.position);
    }
    private void Update()
    {
        if (!active || completed) return;

        if (Vector3.Distance(playerTransform.position, destination.position) <= destinationRange)
        {
            completed = true;
        }
    }
    public override void restartLevel()
    {
        base.restartLevel();
        if (onRestart != null) onRestart();
    }

    public override void onLevelEnd()
    {
        base.onLevelEnd();
        HUDManager.instance.hideDestination();
    }
    public override bool levelCompleted()
    {
        return completed;
    }
}
