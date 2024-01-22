using UnityEngine;

public class KeybindingsManager : MonoBehaviour
{
    public static KeybindingsManager instance;
    private void Awake()
    {
        if (instance != null) {
            Destroy(transform.parent.gameObject);
        }
        instance = this;
        ModData.onDataUpdate += updateKeybinds;
    }
    public void updateKeybinds() {
        if (ModData.current == null) { resetKeybinds(); return; }
        if (ModData.current.keybindData == null) { resetKeybinds(); return; }

        KeyBinds currentData = ModData.current.keybindData;

        assignKey(currentData.move_forward, ref Keybindings.forward,Keybindings.forDef);
        assignKey(currentData.move_backward, ref Keybindings.backward, Keybindings.backDef);
        assignKey(currentData.move_left, ref Keybindings.left, Keybindings.leftDef);
        assignKey(currentData.move_right, ref Keybindings.right, Keybindings.rightDef);
        assignKey(currentData.attack, ref Keybindings.primary, Keybindings.primDef);
        assignKey(currentData.aim, ref Keybindings.secondary, Keybindings.secDef);
        assignKey(currentData.sprint, ref Keybindings.sprint, Keybindings.sprintDef);
        assignKey(currentData.crouch, ref Keybindings.crouch, Keybindings.crouchDef);
        assignKey(currentData.jump, ref Keybindings.jump, Keybindings.jumpDef);
        assignKey(currentData.reload, ref Keybindings.reload, Keybindings.reloadDef);
    }
    void resetKeybinds() {
        assignKey(null, ref Keybindings.forward, Keybindings.forDef);
        assignKey(null, ref Keybindings.backward, Keybindings.backDef);
        assignKey(null, ref Keybindings.left, Keybindings.leftDef);
        assignKey(null, ref Keybindings.right, Keybindings.rightDef);
        assignKey(null, ref Keybindings.primary, Keybindings.primDef);
        assignKey(null, ref Keybindings.secondary, Keybindings.secDef);
        assignKey(null, ref Keybindings.sprint, Keybindings.sprintDef);
        assignKey(null, ref Keybindings.crouch, Keybindings.crouchDef);
        assignKey(null, ref Keybindings.jump, Keybindings.jumpDef);
        assignKey(null, ref Keybindings.reload, Keybindings.reloadDef);
    }
    void assignKey(string stringKey, ref KeyCode bind, KeyCode defBind) {
        if (!string.IsNullOrEmpty(stringKey))
        {
            KeyCode code = 0;
            try
            {
                code = (KeyCode)System.Enum.Parse(typeof(KeyCode), stringKey);
            }
            catch
            {
                code = 0;
            }

            if (code != 0 && !isTaken(code))
            {
                bind = code;
            }
            else
            {
                bind = defBind;
            }
        }
        else {

            bind = defBind;
        }
    }
    bool isTaken(KeyCode code) {
        if (Keybindings.forward == code) return true;
        if (Keybindings.backward == code) return true;
        if (Keybindings.left == code) return true;
        if (Keybindings.right == code) return true;
        if (Keybindings.primary == code) return true;
        if (Keybindings.secondary == code) return true;
        if (Keybindings.crouch == code) return true;
        if (Keybindings.sprint == code) return true;
        if (Keybindings.jump == code) return true;
        if (Keybindings.reload == code) return true;
        return false;
    }

    private void OnDestroy()
    {
        ModData.onDataUpdate -= updateKeybinds;
    }
}
