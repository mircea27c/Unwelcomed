using UnityEngine;
using System.IO;

public class TexturesManager : MonoBehaviour
{
    public static TexturesManager instance;

    [SerializeField] Material defaultPlayerMat;
    [SerializeField] Material defaultEnemyMat;
    [SerializeField] Material defaultWeaponMat;
    [SerializeField] TerrainLayer defaultGroundMat;
    [SerializeField] TerrainLayer defaultGrassMat;

    public Texture default_plr;
    public Texture default_enemy;
    public Texture default_weapon;
    public Texture default_ground;
    public Texture default_grass;

    public Texture player_skin;
    public Texture enemy_skin;
    public Texture weapon_skin;
    public Texture ground_texture;
    public Texture ground_grass_texture;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(transform.parent.gameObject);
        }
        else
        {

            instance = this;
        }
    }
    private void Start()
    {
        resetDefault();
    }


    void resetDefault() {
        updateMat(ref defaultPlayerMat, default_plr, default_plr);
        updateMat(ref defaultEnemyMat, default_enemy, default_enemy);
        updateMat(ref defaultWeaponMat, default_weapon, default_weapon);
        updateMat(ref defaultGroundMat, default_ground, default_ground);
        updateMat(ref defaultGrassMat, default_grass, default_grass);
    }

    public void pullTextures() {
        if (ModData.current == null) { resetDefault(); return; }
        if (ModData.current.skinData == null) {resetDefault(); return; }
        string path = ModData.current.path + "/Textures/";
        checkAndAssign(ref player_skin, getTexture(path + ModData.current.skinData.default_player_skin));
        checkAndAssign(ref enemy_skin, getTexture(path + ModData.current.skinData.default_enemy_skin));
        checkAndAssign(ref weapon_skin, getTexture(path + ModData.current.skinData.default_weapon_skin));
        checkAndAssign(ref ground_texture, getTexture(path + ModData.current.skinData.default_ground_texture));
        checkAndAssign(ref ground_grass_texture, getTexture(path + ModData.current.skinData.default_ground_grass_texture));

        updateMaterials();
    }

    void updateMaterials() {
        updateMat(ref defaultPlayerMat, player_skin, default_plr);
        updateMat(ref defaultEnemyMat, enemy_skin, default_enemy);
        updateMat(ref defaultWeaponMat, weapon_skin, default_weapon);
        updateMat(ref defaultGroundMat, ground_texture, default_ground);
        updateMat(ref defaultGrassMat, ground_grass_texture, default_grass);
    }

    public Texture getTexture(string path)
    {

        string filePath;
        if (File.Exists(path + ".png"))
        {
            filePath = path + ".png";
        }
        else if (File.Exists(path + ".jpg"))
        {
            filePath = path + ".jpg";
        }
        else
        {
            return null;
        }

        byte[] iconData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(300, 300);
        tex.LoadImage(iconData);
        return tex;
    }

    void checkAndAssign(ref Texture destination, Texture value)
    {
        destination = value;
    }

    void updateMat(ref Material mat, Texture tex, Texture def) {
        if (tex != null)
        {
            mat.mainTexture = tex;
        }
        else {
            mat.mainTexture = def;
        }
    }
    void updateMat(ref TerrainLayer mat, Texture tex, Texture def)
    {
        if (tex != null)
        {
            if (tex != def)
            {
                mat.diffuseTexture = removeAlpha((Texture2D)tex);
                removeAlpha(mat.diffuseTexture);
                mat.metallic = 0;
                mat.smoothness = 0;
            }
            else
            {
                mat.diffuseTexture = (Texture2D)tex;
            }
        }
        else {
            mat.diffuseTexture = (Texture2D)def; 
            mat.metallic = 0;
            mat.smoothness = 0;
        }
    }

    private Texture2D removeAlpha(Texture2D tex)
    {
        Color pixA;
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                pixA = tex.GetPixel(i, j);
                pixA.a = 0;
                tex.SetPixel(i,i,pixA);
            }
        }
        tex.Apply();
        return tex;
    }

}
