using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] Transform cam;

    [SerializeField] TMP_Text objectiveText;
    [SerializeField] Animator objectiveAnim;

    [Header("HUD")]


    [SerializeField] float oreCheckDistance;
    [SerializeField] float hudCheckDistance;
    [SerializeField] float hudProcessInterval;
    [SerializeField] LayerMask hudMask;
    [SerializeField] TMP_Text hudTextField;
    [SerializeField] Canvas hudBox;
    [SerializeField] int hudCyclesDuration;
    RectTransform hudRect;
    Camera camComponent;
    int hudCycles = 0;

    [SerializeField] Animator hudAnim;

    [SerializeField] RectTransform arrow;
    [SerializeField] GameObject arrowParent;

    [SerializeField] Transform crosshair;
    [SerializeField] Transform hitMarker;

    [SerializeField] GameObject failScreen;
    [SerializeField] Animator failAnim;

    [Header("Ammo")]
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Color normalAmmoColor;
    [SerializeField] Color lowAmmoColor;


    [Header("Metals")]
    [SerializeField] TMP_Text alluminiumText;
    [SerializeField] TMP_Text titaniumText;
    [SerializeField] bool noMetals;

    [SerializeField] GameObject metalTextBox;
    Animator allumAnim;
    Animator titaAnim;

    Transform target;
    Transform plrTransform;

    float hudProcessTimer;

    bool hasDestionation;
    Vector3 destination;


    [Header("Weapons")]
    [SerializeField] GameObject pref_weapPanel;
    [SerializeField] Transform weapContentParent;
    [SerializeField] RectTransform scrollView;
    WeaponPanelScript[] weaps;
    int weapsLength;

    Vector2 targetPos;

    private void Awake()
    {
        instance = this;
        hudBox.enabled = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;


        hudRect = hudBox.GetComponent<RectTransform>();
        camComponent = cam.GetComponent<Camera>();
        if (!noMetals)
        {
            allumAnim = alluminiumText.GetComponent<Animator>();
            titaAnim = titaniumText.GetComponent<Animator>();

            hideMetals();
        }
    }

    private void Start()
    {
        plrTransform = PlayerManager.instance.transform;
        hideDestination();

        createWeaponsUi();
    }

    public void createWeaponsUi() {
        Weapon[] weapons = PlayerManager.instance.GetComponent<WeaponsManager>().weaponSlots;
        weaps = new WeaponPanelScript[weapons.Length];
        for (int i = 0; i < weaps.Length; i++)
        {
            WeaponPanelScript panel = Instantiate(pref_weapPanel, weapContentParent).GetComponent<WeaponPanelScript>();
            panel.updateInfo(weapons[i]);
            weaps[i] = panel;
        }
        weapContentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(weapContentParent.GetComponent<RectTransform>().sizeDelta.x, 101 * weaps.Length);
        weapsLength = weapons.Length;
    }
    public void selectWeapon(int index) {
        targetPos = new Vector2(0,Mathf.Clamp(index - 1,0, 100) / (float)weapsLength * scrollView.sizeDelta.y);
        for (int i = 0; i < weapsLength; i++)
        {
            weaps[i].updateDistance(Mathf.Abs(i - index));
        }
    }
    void processWeaponsList() {
        if (scrollView.anchoredPosition != targetPos) {
            scrollView.anchoredPosition = Vector2.Lerp(scrollView.anchoredPosition, targetPos, 0.1f);
        }
    }
    public void updateObjective(string newText) {
        objectiveAnim.Play("ObjectiveShow");
        objectiveText.text = newText;
    }

    private void Update()
    {
        processHud();
        processWeaponsList();
        if (hasDestionation)
        {
            processDestination();
        }
    }

    void processHud() {
        hudProcessTimer += Time.deltaTime;
        if (hudProcessTimer > hudProcessInterval) {
            hudProcessTimer = 0;
            hudCycles++;
            checkForTargets();
        }
        if (target != null && target.gameObject.activeSelf)
        {
            hudRect.position = camComponent.WorldToScreenPoint(target.position);
        }
        else {
            if (hudBox.enabled) {
                hideBox();
            }
        }
    }

    void checkForTargets() {
        if (!raycastTargets())
        {
            checkHudCycles();
        }
        else {
            hudCycles++;
        }
    }

    bool raycastTargets() {
        if (Physics.Raycast(cam.position, cam.forward * oreCheckDistance, out RaycastHit hit, oreCheckDistance, hudMask))
        {
            Transform hitTransform = hit.transform;

            if (hitTransform == null) return false;
            if (hitTransform.gameObject.CompareTag("Ore"))
            {
                if (target == null || hitTransform != target)
                {
                    drawBox(hitTransform.GetComponent<oreScript>().type.ToString());
                    target = hitTransform;
                }
                 return true;
            }
        }
        if (Physics.Raycast(cam.position, cam.forward * hudCheckDistance, out RaycastHit hit2, hudCheckDistance, hudMask))
        {
            Transform hitTransform = hit.transform;
            if (hitTransform == null) return false;
            if (hitTransform.gameObject.CompareTag("Interactable"))
            {
                if (target == null || hitTransform != target)
                {
                    drawBox(hitTransform.GetComponent<HudTarget>().title);
                    target = hitTransform;
                }
               return true;
            }
        }
        return false;
    }

    void checkHudCycles() {
        if (hudBox.enabled)
        {
            if (hudCycles >= hudProcessInterval)
            {
                hideBox();
            }
            else
            {
                hudCycles++;
            }
        }
    }

    void hideBox() {
        hudBox.enabled = false;
        target = null;
    }
    void drawBox(string text) {
        hudTextField.text = text;
        hudBox.enabled = true;
        hudAnim.Play("BoxShow");
    }

    void processDestination() {
        Vector2 fwd = new Vector2(plrTransform.forward.x, plrTransform.forward.z);
        Vector3 toDest3 = destination - plrTransform.position;
        Vector2 toDest = new Vector2(toDest3.x, toDest3.z);

        arrow.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(fwd, toDest));
    }

    public void setDestination(Vector3 _destination) {
        hasDestionation = true;
        destination = _destination;

        arrowParent.SetActive(true);
    }

    public void hideDestination() {
        hasDestionation = false;
        arrowParent.SetActive(false);
    }

    public RectTransform getCrosshair() {
        return crosshair.GetComponent<RectTransform>();
    }

    public UnityEngine.UI.Image getHitmarker()
    {
        return hitMarker.GetComponent<UnityEngine.UI.Image>();
    }

    public void playLevelFailAnim() {

        StartCoroutine(failAnimEnum());
    }

    IEnumerator failAnimEnum() {
        failScreen.SetActive(true);
        failAnim.Play("FailScreenAnim");

        yield return new WaitForSeconds(2f);

        failScreen.SetActive(false);
    }

    public void updateAmmo(int ammo, int maxAmmo, int weaponIndex) {
        weaps[weaponIndex].updateAmmo(ammo,maxAmmo);

        ammoText.text = ammo + "/" + maxAmmo;
        if (ammo < 5)
        {
            ammoText.color = lowAmmoColor;
        }
        else {
            ammoText.color = normalAmmoColor;
        }
    }

    public void showMetals() {
        metalTextBox.SetActive(true);
        metalTextBox.GetComponent<Animator>().Play("entry");
    }
    public void hideMetals()
    {
        metalTextBox.SetActive(false);
    }
    public void updateAlluminium(int count, int req) {
        alluminiumText.text = count + "/" + req;
        allumAnim.Play("pop");
    }
    public void updateTitanium(int count, int req)
    {
        titaniumText.text = count + "/" + req;
        titaAnim.Play("pop");
    }
}
