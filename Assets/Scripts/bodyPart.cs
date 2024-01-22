using UnityEngine;

public class bodyPart : MonoBehaviour
{
    public EnemyController controller;
    public float damageModifier;

    public enum type { head, body};
    public type part;

    public type takeDamage(float damage) {
        controller.takeDamage((int)(damage* damageModifier));
        return part;
    }
}
