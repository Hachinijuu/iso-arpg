using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoX_Class {NONE, BERSERKER, HUNTER, ELEMENTALIST}

[System.Serializable]
public struct CharacterPair
{
    public GoX_Class Guardian;
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
    [SerializeField] HUDController HUD;
    public List<CharacterPair> characters;
    public Dictionary<GoX_Class, PlayerController> playableCharacters;
    public GoX_Class currentClass;
    public PlayerController currentPlayer;

    public delegate void PlayerChanged(PlayerSelectionArgs e);
    public event PlayerChanged onPlayerChanged;

    public void FirePlayerChanged(PlayerSelectionArgs e)
    {
        if (onPlayerChanged != null) { onPlayerChanged(e); }
    }
    void Awake()
    {
        Debug.Log("[PlayerManager]: Loading characters into gameplay");
        if (characters != null)
        {
            if (playableCharacters == null)
                playableCharacters = new Dictionary<GoX_Class, PlayerController>();
            foreach (CharacterPair c in characters)
            {
                playableCharacters.Add(c.Guardian, c.Character);
                Debug.Log("[PlayerManager]: Loaded " + c.Character.Stats.Class.className + " as a playable character");
                if (c.Character.gameObject.activeInHierarchy)
                {
                    c.Character.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ActivatePlayer(GoX_Class toActivate)
    {
        // Given the class passed to this fuction, see which character should be activated
        playableCharacters.TryGetValue(toActivate, out PlayerController controller);
        if (controller != null)
        {
            if (!controller.gameObject.activeInHierarchy)
            {
                currentClass = toActivate;
                currentPlayer = controller;
                GameManager.Instance.PlayerRespawn(currentPlayer);
                if (!HUD.gameObject.activeInHierarchy)
                {
                    HUD.gameObject.SetActive(true);
                }
                HUD.SetPlayer(currentPlayer);
            }
            foreach (KeyValuePair<GoX_Class, PlayerController> pair in playableCharacters)
            {
                if (pair.Key != currentClass && pair.Value.gameObject.activeInHierarchy)
                {
                    pair.Value.gameObject.SetActive(false);
                    pair.Value.EnablePlayer(false);
                }
            }
            PlayerSelectionArgs arg = new PlayerSelectionArgs(currentClass, currentPlayer);
            FirePlayerChanged(arg);
        }
    }
}
