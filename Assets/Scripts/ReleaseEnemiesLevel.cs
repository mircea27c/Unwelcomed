using UnityEngine;

public class ReleaseEnemiesLevel : MonoBehaviour
{
    [SerializeField]Transform enemiesParent;
    Transform backupParent;

    private void Awake()
    {
        GetComponent<GetToDestinationLevel>().onRestart += restartLevel;
        initializeEnemies();
    }
    void initializeEnemies()
    {
        backupParent = Instantiate(enemiesParent.gameObject, enemiesParent.position, enemiesParent.rotation).transform;
        backupParent.gameObject.SetActive(false);

    }
    public void restartLevel()
    {
        Destroy(enemiesParent.gameObject);
        enemiesParent = backupParent;

        initializeEnemies();
        enemiesParent.gameObject.SetActive(true);
    }
}
