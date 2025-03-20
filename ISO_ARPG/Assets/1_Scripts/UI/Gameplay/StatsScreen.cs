using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreen : MonoBehaviour
{
    PlayerStats stats;
    [SerializeField] TMP_Text fusion;
    [SerializeField] TMP_Text strValue;
    [SerializeField] TMP_Text dexValue;
    [SerializeField] TMP_Text intValue;
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text mana;
    [SerializeField] TMP_Text damage;
    [SerializeField] TMP_Text primaryDamage;
    [SerializeField] TMP_Text secondaryDamage;
    [SerializeField] TMP_Text attackSpeed;
    [SerializeField] TMP_Text critChance;
    [SerializeField] TMP_Text critDamage;
    [SerializeField] TMP_Text projectiles;
    [SerializeField] TMP_Text chains;
    [SerializeField] TMP_Text armour;
    [SerializeField] TMP_Text dodge;
    [SerializeField] TMP_Text dmgMana;
    [SerializeField] TMP_Text healthRegen;
    [SerializeField] TMP_Text moveSpeed;
    [SerializeField] TMP_Text cdr;
    [SerializeField] TMP_Text manaRegen;
    [SerializeField] TMP_Text manaOnHit;
    [SerializeField] TMP_Text idGain;
    public void Start()
    {
        PlayerManager.Instance.onPlayerChanged += context => { GetPlayerStats(); };
    }

    private void GetPlayerStats()
    {
        UnlistenStats();    // Stop listening to the previous guy
        stats = PlayerManager.Instance.currentPlayer.Stats;
        // Once a player is found, listen for stats
        ListenForStats();   // Start listening to the new guy
    }

    public void ListenForStats()
    {
        if (stats != null)
        {
            stats.onFusionSelected += UpdateFusion;
            stats.STR.Changed += context => { UpdateStat(strValue, context); };
            stats.DEX.Changed += context => { UpdateStat(dexValue, context); };
            stats.INT.Changed += context => { UpdateStat(intValue, context); };
            stats.Health.Changed += context => { UpdateStat(health, stats.Health.MaxValue); };
            stats.Mana.Changed += context => { UpdateStat(mana, stats.Mana.MaxValue); };
            stats.Damage.Changed += context => { UpdateStat(damage, context); };
            stats.PrimaryDamage.Changed += context => { UpdateStat(primaryDamage, context); };
            stats.SecondaryDamage.Changed += context => { UpdateStat(secondaryDamage, context); };
            stats.AttackSpeed.Changed += context => { UpdateStat(attackSpeed, context); };
            stats.CritChance.Changed += context => { UpdateStat(critChance, context); };
            stats.CritDamage.Changed += context => { UpdateStat(critDamage, context); };
            stats.Projectiles.Changed += context => { UpdateStat(projectiles, context); };
            stats.Chains.Changed += context => { UpdateStat(chains, context); };
            stats.Armour.Changed += context => { UpdateStat(armour, context); };
            stats.Dodge.Changed += context => { UpdateStat(dodge, context); };
            stats.DamageFromMana.Changed += context => { UpdateStat(dmgMana, context); };
            stats.HealthRegen.Changed += context => { UpdateStat(healthRegen, context); };
            stats.MoveSpeed.Changed += context => { UpdateStat(moveSpeed, context); };
            stats.CooldownReduction.Changed += context => { UpdateStat(cdr, context); };
            stats.ManaRegen.Changed += context => { UpdateStat(manaRegen, context); };
            stats.ManaOnHit.Changed += context => { UpdateStat(manaOnHit, context); };
            stats.IDGain.Changed += context => { UpdateStat(idGain, context); };
        }
    }

    public void OnEnable()
    {
        if (stats == null)
        {
            GetPlayerStats();
        }
        if (stats != null)
        {
            LoadStats();
        }
    }
    public void LoadStats()
    {
            UpdateFusion();
            UpdateStat(strValue, stats.STR.Value);
            UpdateStat(dexValue, stats.DEX.Value);
            UpdateStat(intValue, stats.INT.Value);
            UpdateStat(health, stats.Health.MaxValue);
            UpdateStat(mana, stats.Mana.MaxValue);
            UpdateStat(damage, stats.Damage.Value);
            UpdateStat(primaryDamage, stats.PrimaryDamage.Value);
            UpdateStat(secondaryDamage, stats.SecondaryDamage.Value);
            UpdateStat(attackSpeed, stats.AttackSpeed.Value);
            UpdateStat(critChance, stats.CritChance.Value);
            UpdateStat(critDamage, stats.CritDamage.Value);
            UpdateStat(projectiles, stats.Projectiles.Value);
            UpdateStat(chains, stats.Chains.Value);
            UpdateStat(armour, stats.Armour.Value);
            UpdateStat(dodge, stats.Dodge.Value);
            UpdateStat(dmgMana, stats.DamageFromMana.Value);
            UpdateStat(healthRegen, stats.HealthRegen.Value);
            UpdateStat(moveSpeed, stats.MoveSpeed.Value);
            UpdateStat(cdr, stats.CooldownReduction.Value);
            UpdateStat(manaRegen, stats.ManaRegen.Value);
            UpdateStat(manaOnHit, stats.ManaOnHit.Value);
            UpdateStat(idGain, stats.IDGain.Value);
    }

    public void UnlistenStats()
    {
        if (stats != null)
        {
            stats.STR.Changed -= context => { UpdateStat(strValue, context); };
            stats.DEX.Changed -= context => { UpdateStat(dexValue, context); };
            stats.INT.Changed -= context => { UpdateStat(intValue, context); };
            stats.Health.Changed -= context => { UpdateStat(health, stats.Health.MaxValue); };
            stats.Mana.Changed -= context => { UpdateStat(mana, stats.Mana.MaxValue); };
            stats.Damage.Changed -= context => { UpdateStat(damage, context); };
            stats.PrimaryDamage.Changed -= context => { UpdateStat(primaryDamage, context); };
            stats.SecondaryDamage.Changed -= context => { UpdateStat(secondaryDamage, context); };
            stats.AttackSpeed.Changed -= context => { UpdateStat(attackSpeed, context); };
            stats.CritChance.Changed -= context => { UpdateStat(critChance, context); };
            stats.CritDamage.Changed -= context => { UpdateStat(critDamage, context); };
            stats.Projectiles.Changed -= context => { UpdateStat(projectiles, context); };
            stats.Chains.Changed -= context => { UpdateStat(chains, context); };
            stats.Armour.Changed -= context => { UpdateStat(armour, context); };
            stats.Dodge.Changed -= context => { UpdateStat(dodge, context); };
            stats.DamageFromMana.Changed -= context => { UpdateStat(dmgMana, context); };
            stats.HealthRegen.Changed -= context => { UpdateStat(healthRegen, context); };
            stats.MoveSpeed.Changed -= context => { UpdateStat(moveSpeed, context); };
            stats.CooldownReduction.Changed -= context => { UpdateStat(cdr, context); };
            stats.ManaRegen.Changed -= context => { UpdateStat(manaRegen, context); };
            stats.ManaOnHit.Changed -= context => { UpdateStat(manaOnHit, context); };
            stats.IDGain.Changed -= context => { UpdateStat(idGain, context); };
        }
    }

    void UpdateStat(TMP_Text uiText, float value)
    {
        uiText.text = value.ToString();
        //Debug.Log("Set " + uiText + " to :" + uiText.text);
    }

    void UpdateFusion()
    {
        if (stats.Fusion != null)
        {
            fusion.text = stats.Fusion.Name;
        }
        else
        {
            fusion.text = "";
        }
    }
}
