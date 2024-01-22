using UnityEngine;

public class GatherMetalsLevel : Level
{
    [SerializeField] int requiredAluminium;
    [SerializeField] int requiredTitanium;
    [SerializeField] ResourcesManager resManager;
    
    [SerializeField] Transform enemiesParent;
    Transform backupEnemiesParent;

    [SerializeField] Transform oresParent; 
    Transform backupOresParent;

    int gatheredAluminium = 0;
    int gatheredTitanium = 0;

    bool sentEnemies;

    public enum metalType { aluminium, titanium };
    public static GatherMetalsLevel instance;

    oreScript[] allOres;

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            enemiesParent.GetChild(i).gameObject.SetActive(false);
        }

        allOres = FindObjectsOfType<oreScript>();
        setOresActive(false);
    }
    void initializeReplaceables()
    {
        backupEnemiesParent = Instantiate(enemiesParent.gameObject, enemiesParent.position, enemiesParent.rotation).transform;
        backupEnemiesParent.gameObject.SetActive(false);

        backupOresParent = Instantiate(oresParent.gameObject, oresParent.position, oresParent.rotation).transform;
        backupOresParent.gameObject.SetActive(false);

        enemiesParent.gameObject.SetActive(true);
        oresParent.gameObject.SetActive(true);
    }
    void setOresActive(bool state) {
        for (int i = 0; i < allOres.Length; i++)
        {
            allOres[i].active = state;
        }
    }
    public override void onLevelEnd()
    {
        base.onLevelEnd();

        MusicManager.instance.playTrack(0, 0.05f);
        HUDManager.instance.hideMetals();
    }
    public override void startLevel()
    {
        base.startLevel();
        sentEnemies = false;
        gatheredAluminium = 0;
        gatheredTitanium = 0;

        resManager.collectingOres = true;
        active = true;
        setOresActive(true);
        initializeReplaceables();

        HUDManager.instance.updateObjective("Gather the required metals to repair the ship");
        HUDManager.instance.showMetals();

        HUDManager.instance.updateAlluminium(gatheredAluminium, requiredAluminium);
        HUDManager.instance.updateTitanium(gatheredTitanium, requiredTitanium);

        MusicManager.instance.playTrack(0, 0.05f);
    }

    public override bool levelCompleted()
    {
        if (gatheredAluminium >= requiredAluminium && gatheredTitanium >= requiredTitanium) {
            return true;
        }
        else
        {
            return false;
        }
    }


    void Update()
    {
        if (!active) return;

        if (gatheredAluminium + gatheredTitanium == requiredAluminium + requiredTitanium - 4)
        {
            sendEnemies();
        }

    }

    void sendEnemies() {
        if (sentEnemies) return;
        MusicManager.instance.playTrack(2, 0.8f);
        sentEnemies = true;
        for (int i = 0; i < enemiesParent.childCount; i++) {
            enemiesParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void addMetal(metalType type)
    {
        switch(type){
            case metalType.aluminium:
                gatheredAluminium++;
                HUDManager.instance.updateAlluminium(gatheredAluminium,requiredAluminium);
                break;
            case metalType.titanium:
                gatheredTitanium++;
                HUDManager.instance.updateTitanium(gatheredTitanium,requiredTitanium);
                break;
        }

    }

    public override void restartLevel()
    {
        Destroy(enemiesParent.gameObject);
        enemiesParent = backupEnemiesParent;
        
        Destroy(oresParent.gameObject);
        oresParent = backupOresParent;

        base.restartLevel();
        startLevel();
    }
}
