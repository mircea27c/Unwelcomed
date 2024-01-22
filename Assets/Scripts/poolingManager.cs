using UnityEngine;

public class poolingManager : MonoBehaviour
{
    public static poolingManager instance;
    [SerializeField]GameObjectPool laserBulletsPool;
    [SerializeField]GameObjectPool laserExplosionPool;
    [SerializeField] GameObjectPool debrisExplosionPool;
    [SerializeField] GameObjectPool oreBreakPool;

    [System.Serializable]
    public class GameObjectPool
    {
        public GameObject go;

        public int maxCount;
        public int startCount;

        public GameObject[] gameObjects;
    }

    void initializePools() {
        initializePool(laserBulletsPool);
        initializePool(laserExplosionPool);
        initializePool(debrisExplosionPool);
        initializePool(oreBreakPool);
    }

    void initializePool(GameObjectPool pool) {
        pool.gameObjects = new GameObject[pool.maxCount];

        for (int i = 0; i < pool.startCount; i++) {
            pool.gameObjects[i] = Instantiate(pool.go);
            pool.gameObjects[i].SetActive(false);
        }
    }

    GameObject requestFromPool(GameObjectPool pool) {
        for (int i = 0; i < pool.maxCount; i++) {
            if (pool.gameObjects[i] != null)
            {
                if (!pool.gameObjects[i].activeSelf)
                {
                    pool.gameObjects[i].SetActive(true);
                    return pool.gameObjects[i];
                }
            }
            else {
                break;
            }
        }
        pool.gameObjects[0].SetActive(true);
        return pool.gameObjects[0];
    }
    private void Awake()
    {
        instance = this;
        initializePools();
    }

    public GameObject requestLaserBullet() {
        return requestFromPool(laserBulletsPool);
    }    
    public GameObject requestLaserExplosion() {
        return requestFromPool(laserExplosionPool);
    }

    public GameObject requestDebris() {
        return requestFromPool(debrisExplosionPool);
    }

    public GameObject requestOreBreak()
    {
        return requestFromPool(oreBreakPool);
    }

}
