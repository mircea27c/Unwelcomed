using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class WeaponsManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform weaponParent;
    [SerializeField] public Transform fpsCamera;
    [SerializeField] public LayerMask shootLayer;
    [SerializeField] Camera plrCam;

    [SerializeField] Weapon defaultWeapon;

    [SerializeField] Transform weaponItemsParent;
    public Weapon[] weaponSlots;
    int equippedSlot;

    [Header("Crosshair")]
    RectTransform crossHair;
    [SerializeField] float startRotation;
    [SerializeField] float aimedRotation;
    [SerializeField] float regularSize;
    [SerializeField] float aimedSize;
    [SerializeField] float aimedFov;
    [SerializeField] float regularFov;

    Image hitMarker;
    [SerializeField] float hitMarkerDuration;
    [SerializeField] Color regularColor;
    [SerializeField] Color headshotColor;

    [SerializeField]AudioSource playerAudioSource;

    float hitMarkerTimer;

    float targetCrosshairRotation;
    float targetCrosshairSize;
    float targetFov;

    Vector2 shootCrosshairSize;

    bool aiming;
    bool reloading;

    private void Awake()
    {
        shootCrosshairSize = new Vector2(regularSize * 1.3f, regularSize * 1.3f);
        targetCrosshairRotation = startRotation;
        targetCrosshairSize = regularSize;
        targetFov = regularFov;

        updateModWeapons();

        hideAllWeaps();
    }
    
    #region modUpdate
    void updateModWeapons()
    {
        if (ModData.current == null)
            return;

        if (ModData.current.weaponsData == null || ModData.current.weaponsData.playerWeapons == null)
        {
            return;
        }
        if (HUDManager.instance == null) return;

        WeaponData[] data = ModData.current.weaponsData.playerWeapons;
        weaponSlots = new Weapon[data.Length + 1];
        weaponSlots[0] = defaultWeapon;

        for (int i = 1; i < weaponSlots.Length; i++)
        {
            GameObject weapContainer = new GameObject();
            weapContainer.transform.parent = weaponItemsParent;
            if (weapContainer == null) print("weap container is null");

            weapContainer.AddComponent<LaserPistol>();

            LaserPistol newWeap = weapContainer.GetComponent<LaserPistol>();

            checkAndAssign(ref newWeap.title, data[i - 1].title);
            checkAndAssign(ref newWeap.damage, data[i - 1].damage);
            checkAndAssign(ref newWeap.firerate, data[i - 1].rate_of_fire);
            checkAndAssign(ref newWeap.maxAmmo, data[i - 1].max_ammo);


            newWeap.go = Instantiate(defaultWeapon.go, defaultWeapon.go.transform.position, defaultWeapon.go.transform.rotation);
            newWeap.go.transform.parent = defaultWeapon.go.transform.parent;

            newWeap.slot = i;
            newWeap.currentAmmo = newWeap.maxAmmo;
            newWeap.reloadAnimDuration = defaultWeapon.reloadAnimDuration;
            newWeap.range = defaultWeapon.range;
            newWeap.shootSfx = defaultWeapon.shootSfx;
            LaserPistol pistol = (LaserPistol)defaultWeapon;
            newWeap.pref_bullet = pistol.pref_bullet;

            Texture gunTexture = getTexture(ModData.current.path + "/Textures/" +  data[i - 1].texture_name);
            if (gunTexture == null)
            {
                newWeap.matTexture = defaultWeapon.matTexture;
            }
            else {
                newWeap.matTexture = gunTexture;
            }
            weaponSlots[i] = newWeap;

        }

    }

    Texture getTexture(string path)
    {

        string filePath;
        if (File.Exists(path + ".png"))
        {
            filePath = path +".png";
        }
        else if (File.Exists(path + ".jpg"))
        {
            filePath = path +".jpg";
        }
        else
        {
            return null;
        }

        byte[] iconData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(300, 300);
        tex.LoadImage(iconData);
        return tex;
    }

    void checkAndAssign<T>(ref T destination, T value)
    {
        if (value != null)
        {
            destination = value;
        }
    }
    #endregion

    void hideAllWeaps() {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if(weaponSlots[i] != null)
            weaponSlots[i].go.SetActive(false);
        }
    }
    public void equipWeapon(int slotIndex) {
        if (HUDManager.instance == null) return;

        if (slotIndex >= weaponSlots.Length || slotIndex < 0) return;
        if (weaponSlots[slotIndex] == null) {
            return;
        }

        //deactivate the old weapon
        weaponSlots[equippedSlot].go.SetActive(false);
        //equip the new one
        equippedSlot = slotIndex;
        weaponSlots[equippedSlot].go.SetActive(true);

        HUDManager.instance?.updateAmmo(weaponSlots[equippedSlot].currentAmmo, weaponSlots[equippedSlot].maxAmmo, equippedSlot);
        HUDManager.instance?.selectWeapon(slotIndex);
    }

    private void Start()
    {
        if (HUDManager.instance != null)
        {
            crossHair = HUDManager.instance.getCrosshair();
            hitMarker = HUDManager.instance.getHitmarker();
        }


        equipWeapon(0);
    }

    private void Update()
    {
        if (equippedSlot >= 0) {
            processInput();
            processCrosshairSize();
            processhitmarker();
        }

    }

    void processInput()
    {
        if (Input.GetKeyUp(Keybindings.secondary))
        {
            weaponSlots[equippedSlot].onStopRightClick(this);
        }

        if (reloading) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            equipWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equipWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            equipWeapon(2);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                
                equipWeapon(equippedSlot - 1);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                equipWeapon(equippedSlot + 1);
            }
        }


        if (Input.GetKeyDown(Keybindings.reload)) {
            reload();
            return;
        }
        if (Input.GetKeyDown(Keybindings.primary))
        {
            weaponSlots[equippedSlot].onLeftClick(this);
        }
        else if (Input.GetKeyDown(Keybindings.secondary))
        {
            weaponSlots[equippedSlot].onRightClick(this);
        }

    }
    public void Shoot() {
        Weapon currentWeap = weaponSlots[equippedSlot];

        //subsctract the ammo
        if (currentWeap.currentAmmo <= 0) {
            reload();

            return;
        }
        currentWeap.currentAmmo--;
        HUDManager.instance.updateAmmo(weaponSlots[equippedSlot].currentAmmo, weaponSlots[equippedSlot].maxAmmo, equippedSlot);

        //display the shooting effects
        currentWeap.animator.Play("shoot");
        playerAudioSource.PlayOneShot(currentWeap.shootSfx);


        crosshairShoot();
        //check for enemies
        hitEnemy(currentWeap.damage);

        if (currentWeap.currentAmmo <= 0)
        {
            reload();

            return;
        }
    }

    void reload() {
        if (reloading) return;
        StartCoroutine(reloadEnum());
    }
    IEnumerator reloadEnum() {
        reloading = true;

        weaponSlots[equippedSlot].animator.Play("Reload");
        yield return new WaitForSeconds(weaponSlots[equippedSlot].reloadAnimDuration);

        weaponSlots[equippedSlot].currentAmmo = weaponSlots[equippedSlot].maxAmmo;
        HUDManager.instance.updateAmmo(weaponSlots[equippedSlot].currentAmmo, weaponSlots[equippedSlot].maxAmmo, equippedSlot);

        reloading = false;
    }

    public void Aim() {
        aiming = true;
        targetCrosshairSize = aimedSize;
        targetCrosshairRotation = aimedRotation;
        targetFov = aimedFov;
    }
    public void Unaim() {
        aiming = false;
        targetCrosshairSize = regularSize;
        targetCrosshairRotation = startRotation;
        targetFov = regularFov;
    }
    void hitEnemy(int damage) {
        if (Physics.Raycast(fpsCamera.position, fpsCamera.forward * weaponSlots[equippedSlot].range, out RaycastHit hit, weaponSlots[equippedSlot].range, shootLayer))
        {
            Transform explosion = poolingManager.instance.requestLaserExplosion().transform;
            explosion.position = hit.point + hit.normal * 0.5f;
            if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                bodyPart.type limb = hit.transform.GetComponent<bodyPart>().takeDamage(weaponSlots[equippedSlot].damage);

                if (limb == bodyPart.type.head)
                {
                    showHitmarker(headshotColor);
                }
                else { 
                    showHitmarker(regularColor);
                }
            } else if (hit.transform.gameObject.CompareTag("Ore")) {
                oreScript ore = hit.transform.GetComponent<oreScript>();
                if (ore.enabled) {
                    ore.takeDamage();
                    Transform debris = poolingManager.instance.requestOreBreak().transform;
                    debris.position = hit.point + hit.normal * 0.5f;
                    debris.up = hit.normal;
                }
            }
            else {
                Transform debris = poolingManager.instance.requestDebris().transform;
                debris.position = hit.point + hit.normal * 0.5f;
                debris.up = hit.normal;
            }
        }
    }

    void showHitmarker(Color color) {
        hitMarker.color = color;
        hitMarker.enabled = true;
    }
    void processCrosshairSize() {
        if (crossHair.sizeDelta.x != targetCrosshairSize) {
            crossHair.sizeDelta = Vector2.Lerp(crossHair.sizeDelta, Vector2.one * targetCrosshairSize, 0.02f);
        }
        if (crossHair.rotation.z != targetCrosshairRotation)
        {
            crossHair.rotation = Quaternion.Lerp(crossHair.rotation, Quaternion.Euler(new Vector3(0f, 0f, targetCrosshairRotation)), 0.1f);
        }
        if (plrCam.fieldOfView != targetFov) {
            plrCam.fieldOfView = Mathf.Lerp(plrCam.fieldOfView, targetFov, 0.1f);
        }
    }

    void processhitmarker() {
        if (!hitMarker.enabled) return;

        hitMarkerTimer += Time.deltaTime;
        if (hitMarkerTimer >= hitMarkerDuration) {
            hitMarker.enabled = false;
            hitMarkerTimer = 0;
        }
    }

    public void crosshairShoot() {
        crossHair.sizeDelta = shootCrosshairSize;
    }
}
