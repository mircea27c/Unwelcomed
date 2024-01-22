using UnityEngine;
using UnityEngine.Playables;



public class FollowEnemyLevel : Level
{


    [SerializeField] Transform enemiesParent;

    [SerializeField] float destinationReachDistance;
    [SerializeField] float alertDistance;
    [SerializeField] float crouchAlertDistance;

    [SerializeField] int trackIndex;

    [SerializeField] Transform destination;

    Transform plrTransform;
    Transform enemyTransform;

    [SerializeField]PlayableDirector animator;
    [SerializeField] string objectiveMessage;

    FPSController fpsControl;

    bool enemiesAlerted;
    bool completed;
    private void Awake()
    {
        enemyTransform = enemiesParent.GetChild(0).transform;
    }

    private void Start()
    {

        plrTransform = PlayerManager.instance.transform;
        fpsControl = FindObjectOfType<FPSController>();
    }
    public override void startLevel()
    {
        completed = false;
        base.startLevel();
        HUDManager.instance.updateObjective(objectiveMessage);


        MusicManager.instance.playTrack(trackIndex, 0.5f);

        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            EnemyController enemy = enemiesParent.GetChild(i).gameObject.GetComponent<EnemyController>();
            enemy.invincible = true;
        }
    }

    public override void onLevelEnd()
    {
        base.onLevelEnd();

        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            EnemyController enemy = enemiesParent.GetChild(i).gameObject.GetComponent<EnemyController>();
            enemy.invincible = false;
        }
    }
    public void alertEnemies() {
        enemiesAlerted = true;
        animator.Stop();
        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            EnemyController enemy = enemiesParent.GetChild(i).gameObject.GetComponent<EnemyController>();
            enemy.setTarget(plrTransform.gameObject);
            enemy.invincible = true;
        }

        GameManager.instance.restartLevel();
    }

    private void Update()
    {
        if (enemiesAlerted) completed = false;
        if (!active || completed || enemiesAlerted) return;
        if (Vector3.Distance(PlayerManager.instance.transform.position, destination.position) <= destinationReachDistance) {
            completed = true;
            return;
        }
        checkDistanceToEnemies();
    }

    public override bool levelCompleted()
    {
        return completed;

    }

    void checkDistanceToEnemies() {
        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            enemyTransform = enemiesParent.GetChild(i);
            float distance;
            if (fpsControl.crouching)
            {
                distance = crouchAlertDistance;
            }
            else {
                distance = alertDistance;
            }
            if (Vector3.Distance(plrTransform.position, enemyTransform.position) <= distance)
            {
                alertEnemies();
            }

        }
    }

    public override void restartLevel() {
        completed = false;
        base.restartLevel();
        print(PlayerManager.instance.transform.position);
        resetEnemies();
        startLevel();
    }

    void resetEnemies() {
        animator.time = 0;
        animator.Stop();
        animator.Evaluate();

        enemiesAlerted = false;
        animator.Play();
        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            enemiesParent.GetChild(i).gameObject.GetComponent<EnemyController>().clearTarget();
        }
    }
}
