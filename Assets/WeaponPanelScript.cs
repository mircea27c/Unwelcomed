using UnityEngine;
using UnityEngine.UI;
public class WeaponPanelScript : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text weaponTitle;
    [SerializeField] TMPro.TMP_Text ammoInfo;
    [SerializeField] Image icon;
    [SerializeField] RectTransform iconTransform;

    [SerializeField] Color regularColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Vector2 regularSize;
    [SerializeField] Vector2 selectedSize;

    public void updateInfo(Weapon newWeap) {
        weaponTitle.text = newWeap.title;
        ammoInfo.text = newWeap.currentAmmo + "/" + newWeap.maxAmmo;
    }
    public void updateAmmo(int curr, int max) {
        ammoInfo.text = curr + "/" + max;
    }

    public void updateDistance(int distance) {
        if (distance == 0)
        {
            iconTransform.sizeDelta = selectedSize;
            icon.color = selectedColor;
            weaponTitle.color = selectedColor;
        }
        else { 
            iconTransform.sizeDelta = regularSize; 
            icon.color = regularColor;
            weaponTitle.color = regularColor;
        }

        Color color = icon.color;
        distance = Mathf.Clamp(distance, 0, 3);
        color.a = (3 - distance) / 3f;
        icon.color = color;
    }
}
