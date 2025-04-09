using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUISlot : MonoBehaviour
{
    public Button slotButton;
    public Image playerImage;
    public TMP_Text playerName;
    public TMP_Text playerClass;
    public TMP_Text playerFusion;
    public TMP_Text playerDifficulty;
    PlayerProfile profile;
    public void LoadPlayerSlot(PlayerProfile profile)
    {
        this.profile = profile;
        playerImage.sprite = profile.character.Character.Stats.Class.icon;
        playerName.text = profile.name;
        if (profile.character.Character.Stats.Class.entityName != null)
        {
            playerClass.text = profile.character.Character.Stats.Class.entityName;
        }
        if (profile.fusionAbility != null)
        {
            playerFusion.text = SaveSystem.Instance.GetFusion(profile).Name;
        }
        else
        {
            playerFusion.text = "None";
        }
        playerDifficulty.text = profile.difficulty.ToString();
    }
}
