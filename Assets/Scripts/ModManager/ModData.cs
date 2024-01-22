using UnityEngine;
using System.IO;
[System.Serializable]
public class WeaponData
{
    public string title;
    public int damage;
    public int rate_of_fire;
    public int max_ammo;
    
    public string texture_name;
}
[System.Serializable]
public class PlayerWeapons
{
    public WeaponData[] playerWeapons;
}

[System.Serializable]
public class Enemy
{
    public string name;
    public int health;
    public float size;
    public float accuracy;
    public float speed;
    public float take_cover_chance;

    public WeaponData weapon;

    public string skin_texture_name;
}

[System.Serializable]
public class EnemiesList {
    public Enemy[] enemiesList;
}

[System.Serializable]
public class Skins
{
    public string[] instructions;

    public string default_player_skin;
    public string default_enemy_skin;
    public string default_weapon_skin;
    public string default_ground_texture;
    public string default_ground_grass_texture;
}
[System.Serializable]
public class KeyBinds {
    public string move_forward;
    public string move_backward;
    public string move_left;
    public string move_right;

    public string attack;
    public string aim;
    public string jump;
    public string crouch;
    public string sprint;
    public string reload;
}

[System.Serializable]
public class ModData
{
    public delegate void dataUpdateDeleg();
    public static dataUpdateDeleg onDataUpdate;

    private static ModData _current;
    public static ModData current
    {
        get { return _current; }
        set
        {
            _current = value;
            if (onDataUpdate != null)
                onDataUpdate();
        }
    }

public string path;

    public string title;
    public Sprite icon;


    public PlayerWeapons weaponsData;
    public EnemiesList enemyData;
    public Skins skinData;
    public KeyBinds keybindData;


}

