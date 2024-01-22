using UnityEngine;

public class ModPanel : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text nameField;
    [SerializeField] TMPro.TMP_Text errorField;
    [SerializeField] UnityEngine.UI.Image icon;
    [SerializeField] UnityEngine.UI.Toggle toggle;

    public ModData thisModData;

    private void Awake()
    {
        //errorField.enabled = false;
        ModsMenuManager.onSelectMod += modWasSelected;
    }

    public void updateModData(ModData newModData) {
        thisModData = newModData;
        nameField.text = newModData.title;
        icon.sprite = newModData.icon;
    }

    public void updateError(string error) {
        errorField.enabled = true;
        errorField.text = error;
    }

    public void onToggle() {
        if (toggle.isOn)
        {
            //activate mod
            ModsMenuManager.selectMod(this);
        }
        else {
            ModsMenuManager.clearMod(this);
        }
    }

    void modWasSelected(ModPanel newMod) {
        if (newMod != this) {
            toggle.isOn = false;
        }
    }
}
