using UnityEngine;

public class SecondaryCamCutscene : Cutscene
{
    public override void startLevel()
    {
        PlayerManager.instance.gameObject.SetActive(false);
        HUDManager.instance.gameObject.SetActive(false);
        base.startLevel();
    }
    public override void onLevelEnd()
    {
        base.onLevelEnd();
        PlayerManager.instance.gameObject.SetActive(true);

    }
}
