using UnityEngine;

public class meteorGenerator : MonoBehaviour
{

    [SerializeField] float spawnRadius;
    [SerializeField] GameObject pref_meteor;
    [SerializeField] float speed;
    [SerializeField] float maxSpawnInterval;
    [SerializeField] float distanceFromShip;

    [SerializeField] float maxSize;
    [SerializeField] float minSize;

    Transform plrTransform;

    float spawnTimer;
    float spawnInterval;
    bool generating;

    [SerializeField] Transform ship;

    private void Start()
    {
        
        plrTransform = PlayerManager.instance.transform;
    }
    private void Update()
    {
        if (!generating) return;
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnInterval) {
            spawnMeteor();
            if (maxSpawnInterval > 1f) {
                maxSpawnInterval -= 0.2f;
            }
        }
    }

    void spawnMeteor() {
        spawnTimer = 0;
        spawnInterval = Random.Range(0f, maxSpawnInterval);

        Transform meteorTransform = Instantiate(pref_meteor, transform).transform;
        meteorTransform.localPosition = generateSpawnPos();
        meteorTransform.LookAt(new Vector3(plrTransform.position.x, meteorTransform.position.y, meteorTransform.position.z));
        meteorTransform.GetComponent<Rigidbody>().velocity = Vector3.right* speed;
        meteorTransform.localScale *= Random.Range(minSize, maxSize);
        meteorTransform.SetParent(null);
    }
    Vector3 generateSpawnPos() {
        return new Vector3(0f, Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius));
    }

    public void startGenerator() {
        generating = true;
    }
}
