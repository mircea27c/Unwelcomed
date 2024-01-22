using UnityEngine;

using UnityEngine.Playables;

public class FindVehicleLevel : GetToDestinationLevel
{
    [SerializeField] Transform enemiesParent;
    public override void startLevel()
    {
        base.startLevel();
        MusicManager.instance.playTrack(1,0.4f);
        alertEnemies();
    }

    public void alertEnemies()
    {
        enemiesParent.GetComponent<PlayableDirector>().Stop();
        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            enemiesParent.GetChild(i).gameObject.GetComponent<EnemyController>().setTarget(PlayerManager.instance.gameObject);
        }
    }
}
