using UnityEngine;

public class ShipLevel : Level
{
    [SerializeField] ShipController ship;
    [SerializeField] meteorGenerator meteorGen;

    [SerializeField] Transform destination;
    [SerializeField] float destinationReach;
    bool completed;

    [SerializeField] GameObject steerTip;
    public override void startLevel()
    {
        base.startLevel();
        ship.setActiveControl();
        PlayerManager.instance.setCutsceneMode(true);
        meteorGen.startGenerator();

        steerTip.SetActive(true);
        ship.onShipMove += hideSteerTip;
    }

    public override void onLevelEnd()
    {
        base.onLevelEnd();
        meteorGen.enabled = false;
        ship.stopMovement();
    }

    private void Update()
    {
        if (!active) return;

        if (Vector3.Distance(ship.transform.position, destination.position) < destinationReach) {
            completed = true;
        }
    }


    public override bool levelCompleted()
    {
        return completed;
    }

    public void hideSteerTip() {
        steerTip.SetActive(false);
    }
}
