using UnityEngine;

public class CrashLevel : Level
{
    [SerializeField] float completeCheckInterval;
    public bool completed;

    [SerializeField] Transform enemiesParent;
    Transform backupParent;
    Transform[] allEnemies;

    float completeCheckTimer;

    private void Awake()
    {
        enemiesParent.gameObject.SetActive(false);
    }
    void initializeEnemies() {
        backupParent = Instantiate(enemiesParent.gameObject, enemiesParent.position, enemiesParent.rotation).transform;
        backupParent.gameObject.SetActive(false);


        enemiesParent.gameObject.SetActive(true);

        allEnemies = new Transform[enemiesParent.childCount];
        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            enemiesParent.GetChild(i).gameObject.SetActive(true);
            allEnemies[i] = enemiesParent.GetChild(i);
        }
    }
    public override void startLevel()
    {
        base.startLevel();
        initializeEnemies();
    }

    public override void restartLevel()
    {
        base.restartLevel();
        Destroy(enemiesParent.gameObject);
        enemiesParent = backupParent;
        startLevel();
    }

    private void Update()
    {
        if (!active) { return; }
        if (!completed)
        {
            completeCheckTimer += Time.deltaTime;
            if (completeCheckTimer > completeCheckInterval)
            {
                checkCompletion();
            }
        }
    }

    void checkCompletion() {
        completed = true;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i] != null) {
                completed = false;
            }
        }
    }
    public override bool levelCompleted()
    {
        return completed;
    }

    public override void onLevelEnd()
    {
        base.levelCompleted();
    }
}
