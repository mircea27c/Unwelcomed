using UnityEngine;

public class GetPlasmaLevel : Level
{
    [SerializeField] PlasmaContainer plasmaContainer;
    [SerializeField] BoxCollider doorCollider;
    [SerializeField] BoxCollider doorColliderTrigger;

    bool completed;

    private void Start()
    {
        plasmaContainer.onPickUp += aquiredPlasma;

    }

    public override void startLevel()
    {
        base.startLevel();
        doorCollider.enabled = false;
        doorColliderTrigger.enabled = false;

        HUDManager.instance.updateObjective("Find and aquire plasma");
    }

    public override bool levelCompleted()
    {
        return completed;
    }

    void aquiredPlasma() {
        completed = true;
    }
}
