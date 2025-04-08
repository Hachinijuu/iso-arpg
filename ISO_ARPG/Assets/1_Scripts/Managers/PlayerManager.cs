using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GoX_Class { NONE, BERSERKER, HUNTER, ELEMENTALIST }
public enum GoX_Body { ONE, TWO }

[System.Serializable]
public struct CharacterPair
{
    public GoX_Class Guardian;
    public GoX_Body Body;
    public PlayerController Character;
}

public class PlayerSelectionArgs
{
    public GoX_Class currentClass;
    public PlayerController player;

    public PlayerSelectionArgs()
    {
        currentClass = GoX_Class.NONE;
        player = null;
    }

    public PlayerSelectionArgs(GoX_Class selectedClass, PlayerController player)
    {
        currentClass = selectedClass;
        this.player = player;
    }
}

[System.Serializable]
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerManager>();

            if (!instance)
                Debug.LogError("[PlayerManager]: No Player Manager exists!");

            return instance;
        }
    }
    //[SerializeField] CharacterSelection selectUI;
    public MoveInput moveType;  // THIS IS THE MOVEMENT TYPE OF THE PLAYER
    public delegate void MovementChanged();
    public event MovementChanged OnMovementChanged;
    public void FireMovementChanged() { if (OnMovementChanged != null) OnMovementChanged(); }
    [SerializeField] HUDController HUD;
    public CharacterPair[] playableCharacters;
    //public Dictionary<GoX_Class, PlayerController> playableCharacters;
    public GoX_Class currentClass;
    public PlayerController currentPlayer;

    public Material bodyShader;
    public Material weaponShader;
    //public Shader weaponShader;
    //public Shader bodyShader;
    public Color berserkerColor;
    public Color hunterColor;
    public Color elementalistColor;

    public void SetShaderColor()
    {
        //bodyShader.SetFloat("_OutlineThickness" , 0.05f);   // ONLY SET WHEN ACTIVE
        //weaponShader.SetFloat("_OutlineThickness", 0.05f);

        switch (currentClass)
        {
            case GoX_Class.BERSERKER:
                bodyShader.SetColor("_Outline_Color", berserkerColor);
                weaponShader.SetColor("_Outline_Color", berserkerColor);
            break;
            case GoX_Class.HUNTER:
                bodyShader.SetColor("_Outline_Color", hunterColor);
                weaponShader.SetColor("_Outline_Color", hunterColor);
            break;
            case GoX_Class.ELEMENTALIST:
                bodyShader.SetColor("_Outline_Color", elementalistColor);
                weaponShader.SetColor("_Outline_Color", elementalistColor);
            break;
        }
        //Debug.LogWarning("SHADER:" + bodyShader.shader);
        //Debug.LogWarning("SHADER:" + weaponShader.shader);
    }

    public void SetShaderColor(GoX_Class _class)
    {
        //bodyShader.SetFloat("_OutlineThickness" , 0.05f);   // ONLY SET WHEN ACTIVE
        //weaponShader.SetFloat("_OutlineThickness", 0.05f);

        switch (_class)
        {
            case GoX_Class.BERSERKER:
                bodyShader.SetColor("_Outline_Color", berserkerColor);
                weaponShader.SetColor("_Outline_Color", berserkerColor);
            break;
            case GoX_Class.HUNTER:
                bodyShader.SetColor("_Outline_Color", hunterColor);
                weaponShader.SetColor("_Outline_Color", hunterColor);
            break;
            case GoX_Class.ELEMENTALIST:
                bodyShader.SetColor("_Outline_Color", elementalistColor);
                weaponShader.SetColor("_Outline_Color", elementalistColor);
            break;
        }
        //Debug.LogWarning("SHADER:" + bodyShader.shader);
        //Debug.LogWarning("SHADER:" + weaponShader.shader);
    }

    public float outlineThickness = 0.05f;
    public float weaponThickness = 0.005f;
    public void ShowOutline()
    {
        bodyShader.SetFloat("_OutlineThickness" , outlineThickness);   // ONLY SET WHEN ACTIVE
        weaponShader.SetFloat("_OutlineThickness", weaponThickness);
    }

    public void HideOutline()
    {
        bodyShader.SetFloat("_OutlineThickness" , 0.0f);   // ONLY SET WHEN ACTIVE
        weaponShader.SetFloat("_OutlineThickness", 0.0f);
    }

    public delegate void PlayerChanged(PlayerSelectionArgs e);
    public event PlayerChanged onPlayerChanged;

    public void FirePlayerChanged(PlayerSelectionArgs e)
    {
        if (onPlayerChanged != null) { onPlayerChanged(e); }
    }
    // void Awake()
    // {
    //     Debug.Log("[PlayerManager]: Loading characters into gameplay");
    //     if (characters != null)
    //     {
    //         if (playableCharacters == null)
    //             playableCharacters = new Dictionary<GoX_Class, PlayerController>();
    //         foreach (CharacterPair c in characters)
    //         {
    //             playableCharacters.Add(c.Guardian, c.Character);
    //             c.Character.InitializePlayer();
    //             Debug.Log("[PlayerManager]: Loaded " + c.Character.Stats.Class.entityName + " as a playable character");
    //             if (c.Character.gameObject.activeInHierarchy)
    //             {
    //                 c.Character.gameObject.SetActive(false);
    //             }

    //             // How to account for male / female variations?
    //             // Add more GoX_Classes? Add gender to player controller?
    //         }
    //     }
    //     //DeactivatePlayer();
    // }

    public void Start()
    {
        Debug.Log("[PlayerManager]: Loading characters into gameplay");
        // if (characters != null)
        // {
        //     if (playableCharacters == null)
        //         playableCharacters = new Dictionary<GoX_Class, PlayerController>();
        //     foreach (CharacterPair c in characters)
        //     {
        //         playableCharacters.Add(c.Guardian, c.Character);
        //         c.Character.InitializePlayer();
        //         Debug.Log("[PlayerManager]: Loaded " + c.Character.Stats.Class.entityName + " as a playable character");
        //         if (c.Character.gameObject.activeInHierarchy)
        //         {
        //             c.Character.gameObject.SetActive(false);
        //         }

        //         // How to account for male / female variations?
        //         // Add more GoX_Classes? Add gender to player controller?
        //     }
        // }

        if (playableCharacters != null && playableCharacters.Length > 0)
        {
            foreach (CharacterPair pair in playableCharacters)
            {
                PlayerStats stats = pair.Character.Stats;
                pair.Character.InitializePlayer();
                //pair.valu.InitializePlayer();
                stats.OnDied += context => { HandleDeath(); };
                Debug.Log("Added death listeners");
                Debug.Log("[PlayerManager]: Loaded " + pair.Character.Stats.Class.entityName + " as a playable character");
                if (pair.Character.gameObject.activeInHierarchy)
                {
                    pair.Character.gameObject.SetActive(false);
                }
            }
        }

        SetShaderColor();
        HideOutline();
    }

    public void GivePlayerTrackedStat(TrackedStatTypes type, float value)
    {
        foreach (Stat stat in currentPlayer.Stats.statList)
        {
            if (stat is TrackedStat ts)
            {
                if (ts.type == type)
                {
                    ts.Value += value;
                }
            }
        }
    }

    public void GivePlayerMainStat(MainStatTypes type, float value)
    {
        foreach (Stat stat in currentPlayer.Stats.statList)
        {
            if (stat is MainStat ms)
            {
                if (ms.type == type)
                {
                    ms.Value += value;
                }
            }
        }
    }

    public void GivePlayerSubstat(SubStatTypes type, float value)
    {
        foreach (Stat stat in currentPlayer.Stats.statList)
        {
            if (stat is SubStat ss)
            {
                if (ss.type == type)
                {
                    ss.Value += value;
                }
            }
        }
    }

    bool canGiveOnDamage = false;

    public void SetGiveDamage(bool value, float duration)
    {
        IEnumerator OnDamagedGiver = HandleDamageGiver(duration);
        if (value)
        {
            StartCoroutine(OnDamagedGiver);
        }
        //else
        //{
        //    StopCoroutine(OnDamagedGiver);
        //}
        //canGiveOnDamage = value;
    }

    IEnumerator HandleDamageGiver(float duration)
    {
        canGiveOnDamage = true;
        yield return new WaitForSeconds(duration);
        canGiveOnDamage = false;
    }

    public void GiveResourceOnDamage()
    {
        if (!canGiveOnDamage) { return; }
        // Only do this if the attack is the basic / standard attack
        // How does this know which player attack was performed

        PlayerStats stats = currentPlayer.Stats;
        stats.Mana.Value += stats.ManaOnHit.Value;
        Debug.Log("[PlayerManager]: Gave " + stats.ManaOnHit.Value + " Mana on hit");
    }

    public void GiveResourceOnKill()
    {
        PlayerStats stats = currentPlayer.Stats;
        float idGain = stats.Class.BaseIDGain * stats.IDGain.Value;     // This will increase based on the amount of idGain the player has
        GivePlayerTrackedStat(TrackedStatTypes.ID_BAR, idGain);         // This will be base gain modified
    }

    public void OnDisable()
    {
        if (playableCharacters != null && playableCharacters.Length > 0)
        {
            foreach (CharacterPair pair in playableCharacters)
            {
                PlayerStats stats = pair.Character.Stats;
                stats.OnDied -= context => { HandleDeath(); };
            }

            // foreach (KeyValuePair<GoX_Class, PlayerController> pair in playableCharacters)
            // {
            //     PlayerStats stats = pair.Value.Stats;
            //     stats.OnDied -= context => { HandleDeath(); };
            // }
        }
    }
    public void HandleDeath()
    {
        Debug.Log("Player has died, telling the manager");
        GameManager.Instance.PlayerDied();
    }

    public void DeactivatePlayer()
    {
        currentClass = GoX_Class.NONE;
        currentPlayer = null;
        GameManager.Instance.SetPlayer(currentPlayer);
        if (Inventory.Instance != null) Inventory.Instance.CleanupInventory();
        EnableCharacters(false);
    }

    public void SetPlayer(GoX_Class toActivate, GoX_Body bodyId)
    {
        foreach (CharacterPair pair in playableCharacters)
        {
            if (pair.Guardian == toActivate && pair.Body == bodyId)
            {
                currentClass = pair.Guardian;
                currentPlayer = pair.Character;
                SetShaderColor();
                break;
            }
        }
        // playableCharacters.TryGetValue(toActivate, out PlayerController controller);
        // if (controller != null)
        // {
        //     currentClass = toActivate;
        //     currentPlayer = controller;
        //     SetShaderColor();
        // }
    }

    public void ActivatePlayer()
    {
        PlayerController controller = currentPlayer;
        if (controller != null)
        {
            foreach (CharacterPair pair in playableCharacters)
            {
                if (pair.Guardian != currentClass && pair.Character.gameObject.activeInHierarchy)
                {
                    pair.Character.gameObject.SetActive(false);
                    pair.Character.EnablePlayer(false);
                }
            }
            PlayerSelectionArgs arg = new PlayerSelectionArgs(currentClass, currentPlayer);
            FirePlayerChanged(arg);
        }
    }
    public void ActivatePlayer(GoX_Class toActivate, GoX_Body bodyId)
    {
        // Given the class passed to this fuction, see which character should be activated

        PlayerController controller = null;
        foreach (CharacterPair pair in playableCharacters)
        {
            if (pair.Guardian == toActivate && pair.Body == bodyId)
            {
                controller = pair.Character;
                break;
            }
        }

        //playableCharacters.TryGetValue(toActivate, out PlayerController controller);
        if (controller != null)
        {
            if (!controller.gameObject.activeInHierarchy)
            {
                currentClass = toActivate;
                currentPlayer = controller;
                //controller.InitializePlayer();
                //if (Inventory.Instance != null) Inventory.Instance.SetupInventory(currentPlayer);
                //HUD.SetPlayer(currentPlayer);
                //GameManager.Instance.PlayerRespawn(currentPlayer);

                //if (!HUD.gameObject.activeInHierarchy)
                //{
                //    HUD.gameObject.SetActive(true);
                //}
                //HUD.SetPlayer(currentPlayer);
            }
            foreach (CharacterPair pair in playableCharacters)
            {
                if (pair.Guardian != currentClass && pair.Character.gameObject.activeInHierarchy)
                {
                    pair.Character.gameObject.SetActive(false);
                    pair.Character.EnablePlayer(false);
                }
            }
            PlayerSelectionArgs arg = new PlayerSelectionArgs(currentClass, currentPlayer);
            FirePlayerChanged(arg);
        }
    }


    // This function sets ALL the characters in the playable characters dictionary to be either on / off, only use for scripted characters selection logic sequences
    // (Don't want to have all the characters enabled at once)
    public void EnableCharacters(bool on)
    {
        // foreach (KeyValuePair<GoX_Class, PlayerController> pair in playableCharacters)
        // {
        //     pair.Value.EnablePlayer(on);
        //     pair.Value.gameObject.SetActive(on);
        // }

        foreach (CharacterPair pair in playableCharacters)
        {
            pair.Character.EnablePlayer(on);
            pair.Character.gameObject.SetActive(on);
        }
    }

    //public void HandlePlayerSelect()
    //{
    //    // Place the camera in the proper position
    //
    //    // Activate the character selection UI
    //    selectUI.ShowCharacterSelection(true);
    //}
}
