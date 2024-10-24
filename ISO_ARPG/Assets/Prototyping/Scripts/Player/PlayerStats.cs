using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    // Player Stats ARE a character class, but player stats have to keep track of some additional variables
    // Current Health, Current Mana
    // Player Stats has an active tracker of what the player currently has, health, mana, and their stats.
    #region VARIABLES
    [SerializeField]
    CharacterClass playerClass;                 // the base class information that the player will load

    [SerializeField]
    CharacterClass mixedWith;                   // the class the player has chosen to mix with

    #region Public Accessors
    public CharacterClass Class { get { return playerClass; } }
    public CharacterClass ClassMix { get { return mixedWith; } }
    public float Speed { get { return moveSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    public float MaxHealth { get { return maxHealth; } }
    public float MaxMana { get { return maxMana; } }
    public float Strength { get { return strength; } }
    public float Agility { get { return agility; } }
    public float Wisdom { get { return wisdom; } }
    public float Health { get { return currentHealth; } }
    public float Mana { get { return currentMana; } }
    public float IdentityValue { get { return idBuildup; } }
    #endregion
    #region Stats
    // movement
    float moveSpeed;
    float rotationSpeed;

    // stats
    float maxHealth;
    float maxMana;
    float strength;
    float agility;
    float wisdom;

    // tracked stats
    float currentHealth;
    float currentMana;
    float idBuildup;
    #endregion
    #endregion
    // functionality
    #region LOADING
    public void InitializePlayerStats()
    {
        LoadBaseStats();
    }
    void LoadBaseStats()
    {
        SetSpeed(playerClass.Speed);
        SetRotationSpeed(playerClass.RotationSpeed);
        SetMaxHealth(playerClass.MaxHealth);
        SetMaxMana(playerClass.MaxMana);
        SetStrength(playerClass.Strength);
        SetAgility(playerClass.Agility);
        SetWisdom(playerClass.Wisdom);

        SetHealth(playerClass.MaxHealth);
        SetMana(playerClass.MaxMana);
        SetIdValue(0);
    }

    #endregion
    #region SET FUNCTIONALITY
    public void SetMixture(CharacterClass toMix)
    {
        mixedWith = toMix;
    }

    // movement
    public void SetSpeed(float newSpeed)
    {
        if ((moveSpeed + newSpeed) < CharacterClass.MAX_SPEED)
            moveSpeed = newSpeed;
        else
            moveSpeed = CharacterClass.MAX_SPEED;
        FireSpeedChanged(moveSpeed);
    }

    public void SetRotationSpeed(float newSpeed)
    {
        if ((rotationSpeed + newSpeed) < CharacterClass.MAX_ROTSPEED)
            rotationSpeed = newSpeed;
        else
            rotationSpeed = CharacterClass.MAX_ROTSPEED;
        FireRotationChanged(rotationSpeed);
    }
    // stats
    public void SetMaxHealth(float newHealth)
    {
        if ((maxHealth + newHealth) < CharacterClass.MAX_HEALTH)
            maxHealth = newHealth;
        else
            maxHealth = CharacterClass.MAX_HEALTH;
        FireMaxHealthChanged(maxHealth);
    }

    public void SetMaxMana(float newMana)
    {
        if ((maxMana + newMana) < CharacterClass.MAX_MANA)
            maxMana = newMana;
        else
            maxMana = CharacterClass.MAX_MANA;
        FireMaxManaChanged(maxMana);
    }

    public void SetStrength(float newStrength)
    {
        if ((strength + newStrength) < CharacterClass.MAX_STRENGTH)
            strength = newStrength;
        else
            strength = CharacterClass.MAX_STRENGTH;
        FireStrengthChanged(strength);
    }

    public void SetAgility(float newAgility)
    {
        if ((agility + newAgility) < CharacterClass.MAX_AGILITY)
            agility = newAgility;
        else
            agility = CharacterClass.MAX_AGILITY;
        FireAgilityChanged(agility);
    }

    public void SetWisdom(float newWisdom)
    {
        if ((wisdom + newWisdom) < CharacterClass.MAX_WISDOM)
            wisdom = newWisdom;
        else
            wisdom = CharacterClass.MAX_WISDOM;
        FireWisdomChanged(wisdom);
    }

    // tracked stats
    public void SetHealth(float newHealth)
    {
        if ((currentHealth + newHealth) < CharacterClass.MAX_HEALTH)
            currentHealth = newHealth;
        else
            currentHealth = CharacterClass.MAX_HEALTH;
        FireHealthChanged(newHealth);
    }

    public void SetMana(float newMana)
    {
        if ((currentMana + newMana) < CharacterClass.MAX_MANA)
            currentMana = newMana;
        else
            currentMana = CharacterClass.MAX_MANA;
        FireManaChanged(currentMana);
    }

    public void SetIdValue(float idValue)
    {
        if ((idBuildup + idValue) < playerClass.IdentityCost)
            idBuildup = idValue;
        else
            idBuildup = playerClass.IdentityCost;
        FireIdentityChanged(idBuildup);
    }
    #endregion
    #region EVENT SETUP
    // delegates
    public delegate void ClassMixUpdated(CharacterClass toMix);
    public delegate void HealthUpdated(float health);
    public delegate void ManaUpdated(float mana);
    public delegate void IdentityUpdated(float idValue);
    public delegate void SpeedUpdated(float speed);
    public delegate void RotationUpdated(float rotation);
    public delegate void HealthMaxUpdated(float health);
    public delegate void ManaMaxUpdated(float mana);
    public delegate void StrengthUpdated(float strength);
    public delegate void AgilityUpdated(float agility);
    public delegate void WisdomUpdated(float wisdom);
    // event callbacks
    public event ClassMixUpdated onClassMixed;
    public event HealthUpdated onHealthChanged;
    public event ManaUpdated onManaChanged;
    public event IdentityUpdated onIdentityUpdated;
    public event SpeedUpdated onSpeedChanged;
    public event RotationUpdated onRotationChanged;
    public event HealthMaxUpdated onMaxHealthChanged;
    public event ManaMaxUpdated onMaxManaChanged;
    public event StrengthUpdated onStrengthChanged;
    public event AgilityUpdated onAgilityChanged;
    public event WisdomUpdated onWisdomChanged;

    // functions
    void FireClassMixed(CharacterClass toMix) { if (onClassMixed != null) onClassMixed(toMix); }
    void FireHealthChanged(float health) { if (onHealthChanged != null) onHealthChanged(health); }
    void FireManaChanged(float mana) { if (onManaChanged != null) onManaChanged(mana); }
    void FireIdentityChanged(float idValue) { if (onIdentityUpdated != null) onIdentityUpdated(idValue); }
    void FireSpeedChanged(float speed) { if (onSpeedChanged != null) onSpeedChanged(speed); }
    void FireRotationChanged(float rotation) { if (onRotationChanged != null) onRotationChanged(rotation); }
    void FireMaxHealthChanged(float health) { if (onMaxHealthChanged != null) onMaxHealthChanged(health); }
    void FireMaxManaChanged(float mana) { if (onMaxManaChanged != null) onMaxManaChanged(mana); }
    void FireStrengthChanged(float strength) { if (onStrengthChanged != null) onStrengthChanged(strength); }
    void FireAgilityChanged(float agility) { if (onAgilityChanged != null) onAgilityChanged(agility); }
    void FireWisdomChanged(float wisdom) { if (onWisdomChanged != null) onWisdomChanged(wisdom); }
    #endregion
}
