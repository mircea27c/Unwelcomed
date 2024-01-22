using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    WeaponsManager mng_weapons;
    FPSController mng_fps;

    public static PlayerManager instance;

    public int health = 100;
    Healthbar healthbar;

    Animator animator;

    void getComponents() {
        mng_weapons = GetComponent<WeaponsManager>();
        mng_fps = GetComponent<FPSController>();
        animator = GetComponent<Animator>();
        healthbar = FindObjectOfType<Healthbar>();
    }
    private void Awake()
    {
        instance = this;
        getComponents();
    }

    public void takeDamage(int damage) {
        health -= damage;
        healthbar.updateHealth(health);
        if (health <= 0) {
            GameManager.instance.restartLevel();
        }
    }

    public void setCutsceneMode(bool state) {
        mng_fps.enabled = !state;
        if (state == false)
        {
            mng_fps.resetCamPos();
        }
        mng_weapons.enabled = !state;
        animator.enabled = state;
    }
}
