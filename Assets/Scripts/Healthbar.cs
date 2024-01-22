using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]Slider healthSlider;

    public void updateHealth(int health) {
        healthSlider.value = health;
    }
}
