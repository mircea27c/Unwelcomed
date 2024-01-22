using UnityEngine;
using System.IO;

public class ModsMenuManager : MonoBehaviour
{
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject pref_modPanel;

    [SerializeField] Sprite defaultModsIcon;

    string folderName = "Mods";

    string path;

    public delegate void onSetPanelActive(ModPanel panel);
    public static onSetPanelActive onSelectMod;

    #region mod data

    string weaponsFileName = "PlayerWeapons.json";

    string enemiesFileName = "EnemiesList.json";

    string skinsFileName = "Skins.json";

    string keybindsFilename = "KeyBinds.json";
    #endregion

    private void Awake()
    {
        path = Application.dataPath + "/" + folderName + "/";
    }
    void Start()
    {
        populateModsList();
    }

    void populateModsList()
    {
        if (!Directory.Exists(Application.dataPath + "/" + folderName))
        {
            Directory.CreateDirectory(Application.dataPath + "/" + folderName);
            return;
        }

        DirectoryInfo modsInfo = new DirectoryInfo(Application.dataPath + "/" + folderName);
        DirectoryInfo[] allModDirs = modsInfo.GetDirectories();

        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentParent.GetComponent<RectTransform>().sizeDelta.x, 133 * allModDirs.Length);

        for (int i = 0; i < allModDirs.Length; i++)
        {
            ModPanel panel = Instantiate(pref_modPanel, contentParent).GetComponent<ModPanel>();

            DirectoryInfo thisDir = allModDirs[i];

            ModData newMod = new ModData();

            newMod.title = thisDir.Name;
            newMod.icon = getModIcon(path + newMod.title);

            newMod.path = path + newMod.title;

            if (!getModData(path + newMod.title, newMod)) {
                //mod was imported, but some files had error
                panel.updateError("Some of the files couldn't be loaded and were skipped.");
            }

            if (!Directory.Exists(newMod.path + "/Audio")) {
                Directory.CreateDirectory(newMod.path + "/Audio");
            }
            if (!Directory.Exists(newMod.path + "/Textures"))
            {
                Directory.CreateDirectory(newMod.path + "/Textures");
            }

            TexturesManager.instance.pullTextures();
            KeybindingsManager.instance.updateKeybinds();

            panel.updateModData(newMod);
        }

    }

    Sprite getModIcon(string modPath) {

        string filePath;
        if (File.Exists(modPath + "/" + "icon.png"))
        {
            filePath = modPath + "/" + "icon.png";
        }
        else if (File.Exists(modPath + "/" + "icon.jpg"))
        {
            filePath = modPath + "/" + "icon.jpg";
        }
        else {
            return defaultModsIcon;
        }

        byte[] iconData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(300, 300);
        tex.LoadImage(iconData);
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        return Sprite.Create(tex, rect, Vector2.one * 0.5f);
    }

    bool getModData(string modPath, ModData mod) {
        int error = 1;

        error += deserializeData(weaponsFileName, ref mod.weaponsData, modPath);
        error += deserializeData(enemiesFileName, ref mod.enemyData, modPath);
        error += deserializeData(skinsFileName, ref mod.skinData, modPath);
        error += deserializeData(keybindsFilename, ref mod.keybindData, modPath);

        if (error < 0) {
            return false; 
        }
        return true;
    }

    int deserializeData<T>(string fileName, ref T destination, string modPath)
    {
        if (File.Exists(modPath +"/" + fileName))
        {
            StreamReader sr = new StreamReader(modPath + "/" + fileName);
            string textJson = sr.ReadToEnd();
            sr.Close();

            //messageDisplay.instance.updateText(textJson);
            try
            {
                destination = JsonUtility.FromJson<T>(textJson);
                return 1;
            }
            catch {
                return -10;
            }
        }
        return 0;
    }

    public static void selectMod(ModPanel mod) {
        if (onSelectMod != null) {
            onSelectMod(mod);
        }

        if (mod.thisModData != null)
        {
            ModData.current = mod.thisModData;
        }
    }

    public static void clearMod(ModPanel mod) {
        if (ModData.current == mod.thisModData) {
            ModData.current = null;
        }
    }

}
