using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int health;
    [SerializeField] float speed;
    [SerializeField] float stoppingDistance;
    [SerializeField] float shootingDistance;
    [SerializeField] float rotationSpeed;
    [SerializeField] int takeCoverChance;

    [Header("Setup")]
    [SerializeField] Renderer meshRenderer;
    [SerializeField] Transform gun;
    [SerializeField] Transform torso;
    [SerializeField] Transform head;
    [SerializeField]Animator animator;
    [SerializeField] Transform debugSphere;
    [SerializeField] bool startIdle;
    [SerializeField] AudioSource mainAudio;
    [SerializeField] AudioSource feetAudio;
    [SerializeField] AudioClip shootSfx;

    [SerializeField] GameObject deathRagdoll_pref;

    float scaleFactor = 1;

    public GameObject target;
    state currentState;
    NavMeshAgent agent;
    Transform _transform;

    Transform lookTarget;

    //----Cover-----
    bool hasCover;
    Cover activeCover;

    //state machine
    #region state handling
    [SerializeField]attackState attack;
    [SerializeField]chaseState chase;
    [SerializeField] flankState flank;
    [SerializeField] gettingCoverState getCover;
    [SerializeField] idleState idle;


    public bool invincible;
    public class state {
        [HideInInspector]public EnemyController e;
        public virtual void onEnter() { }
        public virtual void onUpdate() { }
        public virtual void onExit() { }
    }

    [System.Serializable]
    public class idleState :state{
        public override void onUpdate()
        {
            if (e.target != null) {
                e.setState(e.chase);
            }
        }
    }
    
    [System.Serializable]
    public class attackState : state
    {
        public int damage;
        public float firerate;
        public float range;
        public float accuracy;

        [SerializeField] Animator gunAnim;
        [SerializeField] Transform shootingPoint;
        [SerializeField] LayerMask shootingLayer;

        [SerializeField] Transform thisTransform;


        float nextShootTimer;
        float shootingPeriodTimer;
        float shootingTime;

        public override void onEnter()
        {
            shootingTime = Random.Range(1f, 3f);
            e.setAnimationBool("shooting", 0);
            e.setAnimationBool("idleLegs", 1);
        }

        public override void onUpdate()
        {
            nextShootTimer += Time.deltaTime;
            shootingPeriodTimer += Time.deltaTime;
            if (nextShootTimer >= 1 / firerate)
            {
                nextShootTimer = 0;
                shoot();
            }
            if (shootingPeriodTimer > shootingTime)
            {
                //switch state
                shootingPeriodTimer = 0;
                if (!e.hasCover)
                {
                    e.setState(e.chase);
                }
                else {
                    e.setState(e.getCover);
                }
            }
        }

        void shoot()
        {
            //play the effects
            gunAnim.Play("shoot");
            e.mainAudio.PlayOneShot(e.shootSfx);

            Vector3 randomDir = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            Vector3 shootDir = e.head.forward * range + randomDir*(1/accuracy);

            Transform bullet = poolingManager.instance.requestLaserBullet().transform;
            bullet.position = shootingPoint.position;
            bullet.LookAt(e.head.position + shootDir);

            if (Physics.Raycast(e.head.position, shootDir, out RaycastHit hit, range, shootingLayer))
            {

                Transform explosion = poolingManager.instance.requestLaserExplosion().transform;
                explosion.position = hit.point + hit.normal * 0.5f;
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PlayerManager>().takeDamage(damage);
                }
                else {
                    Transform debris = poolingManager.instance.requestDebris().transform;
                    debris.position = hit.point + hit.normal * 0.5f;
                    debris.up = hit.normal;
                }
            }
        }
    }

    [System.Serializable]
    public class chaseState : state {
        float walkTimer;
        float walkTime;
        float distance;

        public override void onEnter()
        {

            //try looking for cover
            if (Random.Range(0, 100f) < e.takeCoverChance) {
                if (CoverManager.instance.findCoverInRange(e.transform.position, 10f, out Cover cov)) {
                    e.activeCover = cov;
                    e.hasCover = true;
                    e.setState(e.getCover);
                    return;
                }
            }

            distance = Vector3.Distance(e.transform.position, e.target.transform.position);
            if (distance >= e.shootingDistance)
            {
                calculateDestination();
            }
            else
            {
                e.setState(e.flank);
                return;
            }

            e.setAnimationBool("idle", 0);
            e.setAnimationBool("walking", 1);

            e.feetAudio.Play();

            e.agent.isStopped = false;
            walkTime = Random.Range(1f, 5f);
            walkTimer = 0;

        }

        public override void onUpdate()
        {
            walkTimer += Time.deltaTime;

            distance = Vector3.Distance(e.transform.position, e.target.transform.position);
            if (walkTimer > walkTime)
            {
                e.setState(e.attack);
            }
            if (distance < e.shootingDistance) {
                e.setState(e.flank);
            }

        }
        public override void onExit()
        {
            e.agent.isStopped = true;

            e.feetAudio.Stop();
        }
        void calculateDestination() {
            e.agent.SetDestination(e.target.transform.position);
        }

    }

    [System.Serializable]
    public class flankState:state {
        float distance;
        int maxTries;
        int tries;
        public override void onEnter()
        {
            e.setAnimationBool("idle", 0);

            generateFlank();
            e.agent.isStopped = false;

            tries = 0;
            //daca ruta aleasa este invalida, se va genera o noua ruta de maxim "max tries" ori 
            while (tries < maxTries)
            {
                if (e.agent.pathStatus == NavMeshPathStatus.PathPartial || e.agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    print(e.agent.pathStatus);
                    generateFlank();
                    tries++;
                }
            }
            if (tries >= maxTries && e.agent.pathStatus == NavMeshPathStatus.PathPartial || e.agent.pathStatus == NavMeshPathStatus.PathInvalid) {
                e.setState(e.attack);
            }
        }
        void generateFlank() {

            distance = Random.Range(1f, 7f);
            int dir;
            if (Random.Range(-1f, 1f) >= 0)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }

            e.setAnimationBool("walkingSide", 1, dir);
            Vector3 destination = e.transform.position + (e.transform.right * dir * distance) + e.transform.forward * -1 * Random.Range(0f, 3f);

            //gasim altitudinea corecta la pozitia aleasa
            if (Physics.Raycast(destination + Vector3.up * 30f, Vector3.down * 80f, out RaycastHit hit, LayerMask.NameToLayer("Terrain"))) {
                destination.y = hit.point.y;
            }

            if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, 3f, 1))
            {
                destination = navHit.position;
            }
            e.agent.SetDestination(destination);
        }
        public override void onUpdate()
        {
            distance = Vector3.Distance(e.transform.position, e.agent.destination);
            if (distance < 0.5f) {
                e.setState(e.attack);
            }
        }
    }

    [System.Serializable]
    public class gettingCoverState : state {
        bool goingToCover;
        bool goingToShoot;
        Vector3 destination;

        float coverTimer = 0;
        float coverTime;

        public override void onEnter()
        {
            goingToCover = false;
            goingToShoot = false;
            if (!e.hasCover) {
                e.setState(e.chase);
                return;
            }
            //if it's not at cover, get cover
            if (Vector3.Distance(e.transform.position, e.activeCover.coverPosition) > 0.5f)
            {
                goToCover();
            }
            else {
                hideForRandomTime();
            }

            e.setAnimationBool("covering", 0);
        }

        public override void onUpdate()
        {
            //safeguard
            
            if (!e.hasCover) {
                e.setState(e.chase);
                return;
            }

            if (goingToCover)
            {
                if (Vector3.Distance(e.transform.position, destination) <= 0.2f)
                {
                    //reached cover
                    goingToCover = false;
                    hideForRandomTime();
                }
            } else if (goingToShoot) {
                if (Vector3.Distance(e.transform.position, destination) <= 0.1f)
                {
                    //reached cover
                    goingToShoot = false;
                    e.setState(e.attack);
                }
            }
            else {
                //hide for x seconds then shoot
                coverTimer += Time.deltaTime;
                if (coverTimer > coverTime && coverTime != 0) {
                    goToShoot();
                    return;
                }
            }
        }
        void hideForRandomTime() {
            goingToCover = false;
            goingToShoot = false;
            coverTime = Random.Range(2f, 5f);
            coverTimer = 0;

            e.lookTarget = e.activeCover.shootingPoint;
            e.setAnimationBool("idleLegs", 1);
        }
        void goToCover() {
            goingToCover = true;
            goingToShoot = false;
            setDestination(e.activeCover.coverPosition);
            e.lookTarget = e.activeCover.shootingPoint;
            e.setAnimationBool("walking", 1);
        }
        void goToShoot() {
            goingToShoot = true;
            goingToCover = false;
            setDestination(e.activeCover.shootingPosition);

            e.lookTarget = e.target.transform;
            e.setAnimationBool("walkingSide", 1);
        }

        void setDestination(Vector3 position) {

            destination = position;
            //gasim altitudinea corecta la pozitia aleasa
            if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down * 20f, out RaycastHit hit, LayerMask.NameToLayer("Terrain")))
            {
                destination.y = hit.point.y;
            }

            if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, 1f, 1))
            {
                destination = navHit.position;
            }
            e.agent.SetDestination(destination);
            destination = e.agent.destination;
            e.agent.isStopped = false;
        }

    }

    void initializeStates()
    {
        attack.e = this;
        chase.e = this;
        flank.e = this;
        getCover.e = this;
        idle.e = this;
    }
    #endregion
    private void Awake()
    {
        
        
        _transform = transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = 0f;
        initializeStates();

        loadModData();

        if (debugSphere != null) {
            debugSphere.parent = null;
        }
        if (attack.accuracy == 0) attack.accuracy = 0.1f;


    }

    void loadModData() {
        if (ModData.current == null || ModData.current.enemyData == null)
            return;

        EnemiesList listData = ModData.current.enemyData;

        Enemy data = listData.enemiesList[Random.Range(0, listData.enemiesList.Length)];
        if (data == null) return;

        if (data.health != 0)
        {
            health = data.health;
        }
        if (data.size != 0)
        {
            scaleFactor = data.size;
        }
        if (scaleFactor != 0)
        {
            transform.localScale = transform.localScale * scaleFactor;
        }
        attack.accuracy = data.accuracy;
        if (data.speed != 0)
        {
            speed = data.speed;
        }
        takeCoverChance = (int)data.take_cover_chance;

        Texture tex = TexturesManager.instance.getTexture(ModData.current.path + "/Textures/" +  data.skin_texture_name);
        if (tex != null) {
            Material mat = new Material(meshRenderer.material);
            mat.mainTexture = tex;
            meshRenderer.material = mat;
        }

        //load gun
        WeaponData weap = data.weapon;
        if (weap != null) {
            checkAndAssign(ref attack.damage, weap.damage);
            checkAndAssign(ref attack.firerate, weap.rate_of_fire);

            Texture gunTexture = TexturesManager.instance.getTexture(ModData.current.path + "/Textures/" + weap.texture_name);
            if (gunTexture != null) {
                Renderer[] allRendereres = gun.transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in allRendereres)
                {
                    Material[] mats = rend.materials;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i].mainTexture = gunTexture;
                    }
                    rend.materials = mats;
                }

                foreach (Transform child in gun.transform)
                {
                    allRendereres = child.GetComponentsInChildren<Renderer>();
                    foreach (Renderer rend in allRendereres)
                    {
                        Material[] mats = rend.materials;
                        for (int i = 0; i < mats.Length; i++)
                        {
                            mats[i].mainTexture = gunTexture;
                        }
                        rend.materials = mats;
                    }
                }
            }
        }

    }

    void checkAndAssign(ref int destination, int value) {
        if (value != 0)
            destination = value;
    }
    void checkAndAssign(ref float destination, float value)
    {
        if (value != 0)
            destination = value;
    }

    private void Start()
    {
        //elmina asta
        if (!startIdle)
        {
            setTarget(FindObjectOfType<FPSController>().gameObject);
            setState(chase);
            lookTarget = target.transform;
        }
        else {
            setState(idle);
        }
    }
    public void setState(state newState) {
        //print("setting state to " + newState.GetType().ToString());
        if (currentState != null)
        {
            currentState.onExit();
        }
        currentState = newState;
        currentState.onEnter();
    }

    private void Update()
    {

        if (debugSphere != null && target != null) { debugSphere.position = agent.destination; }

        if (currentState == null) {
            return;
        }

        //always look at enemy
        if (target != null) {
            lookAtEnemy();
        }

        currentState.onUpdate();
    }
    void setAnimationBool(string boolName, int layer)
    {
        setAnimationBool(boolName, layer, 1);
    }
    void setAnimationBool(string boolName, int layer, int value) {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (layer == 0)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(param.name, false);
                }
            }
            else {
                if (param.type == AnimatorControllerParameterType.Int)
                {
                animator.SetInteger(param.name, 0);
                }
            }
        }
        if (layer == 0)
        {
            animator.SetBool(boolName, true);
        }
        else {
            animator.SetInteger(boolName, value);
        }
    }

    void lookAtEnemy() {
        
        Vector3 lookPos = lookTarget.transform.position - _transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, rotation, Time.deltaTime * rotationSpeed);     
        
        torso.rotation = Quaternion.Slerp(torso.rotation, rotation, Time.deltaTime * rotationSpeed * 1.5f);

        Vector3 headLook = target.transform.position - head.position;
        Quaternion headRotation = Quaternion.LookRotation(headLook);
        head.rotation = Quaternion.Slerp(head.rotation, headRotation, Time.deltaTime * rotationSpeed * 1.5f);

    }
    public void takeDamage(int amount) {
        //animator.Play("damageEffect");

        if (invincible) return;
        health -= amount;
        if (health <= 0) {
            Die();
        }
    }
    void Die() {
        if (hasCover) {
            activeCover.busy = false;
        }
        spawnRagdoll();
        Destroy(gameObject);
    }
    void spawnRagdoll() {
        Transform doll = Instantiate(deathRagdoll_pref, transform.position, transform.rotation).transform;
        deathRagdollHandler ragdoll = doll.GetComponent<deathRagdollHandler>();
        ragdoll.meshRenderer.material = meshRenderer.material;
        ragdoll.setScale(scaleFactor);
    }

    public void setTarget(GameObject _target) {
        target = _target;
        lookTarget = _target.transform;
    }

    public void clearTarget() {
        setState(idle);
        target = null;
    }
}
