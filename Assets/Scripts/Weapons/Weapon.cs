using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject go;
    public string title;
    public int slot;
    public Texture matTexture;

    public int maxAmmo;
    public int currentAmmo;
    public float reloadAnimDuration;

    public int firerate;
    public int damage;
    public int range;

    public AudioClip shootSfx;

    public Animator animator;
    private void Start()
    {
        animator = go.GetComponent<Animator>();
        setTexture();
    }

    public void setTexture() {

        if (TexturesManager.instance == null) return;

        if (matTexture == null) {
            matTexture = TexturesManager.instance.weapon_skin;
            if (matTexture == null) { 
                matTexture = TexturesManager.instance.default_weapon;
            }
        }

        Renderer[] allRendereres = go.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in allRendereres)
        {
            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].mainTexture = matTexture;
            }
            rend.materials = mats;
        }
        
        foreach (Transform child in go.transform)
        {
            allRendereres = child.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in allRendereres)
            {
                Material[] mats = rend.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i].mainTexture = matTexture;
                }
                rend.materials = mats;
            }
        }
    }

    public virtual void onLeftClick(WeaponsManager plr) {
        plr.Shoot();
    }
    public virtual void onRightClick(WeaponsManager plr)
    {

    }
    public virtual void onStopRightClick(WeaponsManager plr) { 
        
    }

    public bool shootRay(WeaponsManager plr, out RaycastHit hit)
    {
        if (Physics.Raycast(plr.fpsCamera.position, plr.fpsCamera.forward * range, out hit, range, plr.shootLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
