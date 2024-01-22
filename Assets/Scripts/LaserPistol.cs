using UnityEngine;

public class LaserPistol : Weapon
{
    [SerializeField]public GameObject pref_bullet;
    [SerializeField]public Transform muzzle;
    // Update is called once per frame
    [SerializeField]public Transform magTransform;

    private void Start()
    {

        animator = go.GetComponent<Animator>();
        muzzle = findWithTagInChildren(go.transform, "Muzzle").transform;
        magTransform = findWithTagInChildren(go.transform, "Mag").transform;
        setTexture();
    }

    GameObject findWithTagInChildren(Transform parent, string tag) {
        foreach (Transform child in parent)
        {
            if (child.gameObject.CompareTag(tag)) return child.gameObject;
            foreach (Transform child2 in child)
            {
                if (child2.gameObject.CompareTag(tag)) return child.gameObject;
            }
        }
        return null;
    }

    public override void onLeftClick(WeaponsManager plr)
    {
        base.onLeftClick(plr);
        laserEffect(plr);
        //updateMagPosition();
    }
    void laserEffect(WeaponsManager plr) {
        Vector3 destination;

        Transform bullet = poolingManager.instance.requestLaserBullet().transform;
        bullet.position = muzzle.position;
        if (shootRay(plr, out RaycastHit hit))
        {
            destination = hit.point;
        }
        else {
            destination = plr.fpsCamera.position + plr.fpsCamera.forward * 200f;
        }
        bullet.LookAt(destination);
    }
    public override void onRightClick(WeaponsManager plr)
    {
        plr.Aim();
    }
    public override void onStopRightClick(WeaponsManager plr)
    {
        plr.Unaim();
    }
}
