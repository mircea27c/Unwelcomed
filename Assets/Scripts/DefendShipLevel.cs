using UnityEngine;

public class DefendShipLevel : Level
{
    [SerializeField] Canvas progressUI;
    [SerializeField] float defendTime;
    float defendTimer;

    [SerializeField] UnityEngine.UI.Slider progressSlider;

    [SerializeField] Transform[] enemySetsParents;
    [SerializeField] Transform[] enemyCopySets;

    float releaseInterval;
    float releaseTimer;
    int enemyWaveIndex = 0;

    bool completed;

    private void Start()
    {
        progressSlider.maxValue = defendTime;

        releaseInterval = defendTime / (enemySetsParents.Length + 1);
        progressUI.enabled = false;

    }

    public override void startLevel()
    {
        base.startLevel();
        releaseNextEnemyWave();
        progressUI.enabled = true;
        HUDManager.instance.updateObjective("Defend the ship while your mechanic refuels");

        enemyWaveIndex = 0;

        releaseTimer = 0;
        defendTimer = 0;
        updateTimerUi();

        initializeEnemyCopies();

        MusicManager.instance.playTrack(1, 0.4f);
    }
    public override void onLevelEnd()
    {
        progressUI.enabled = false;
    }
    void releaseNextEnemyWave()
    {
        if (enemyWaveIndex >= enemySetsParents.Length) return;
        Transform thisWave = enemySetsParents[enemyWaveIndex];
        thisWave.gameObject.SetActive(true);
        enemyWaveIndex++;
    }

    void initializeEnemyCopies() {
        enemyCopySets = new Transform[enemySetsParents.Length];
        for (int i = 0;i < enemySetsParents.Length; i++)
        {
            enemyCopySets[i] = Instantiate(enemySetsParents[i], enemySetsParents[i].position, enemySetsParents[i].rotation);
            enemyCopySets[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!active) return;
        defendTimer += Time.deltaTime;
        releaseTimer += Time.deltaTime;
        
        updateTimerUi();
        if (defendTimer >= defendTime) {
            completed = true;
        }
        if (releaseTimer >= releaseInterval)
        {
            releaseNextEnemyWave();
            releaseTimer = 0;
        }
    }


    public override bool levelCompleted()
    {
        return completed;
    }
    void updateTimerUi() {
        progressSlider.value = defendTimer;
    }
    void destroyEnemyParents() {
        foreach (Transform enemySet in enemySetsParents)
        {
            Destroy(enemySet.gameObject);
        }
    }
    public override void restartLevel()
    {
        base.restartLevel();
        destroyEnemyParents();
        enemySetsParents = enemyCopySets;
        initializeEnemyCopies();

        startLevel();
    }
}
