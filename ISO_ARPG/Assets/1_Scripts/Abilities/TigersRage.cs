using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TigersRage", menuName = "sykcorSystems/Abilities/Berserker/TigersRage", order = 4)]
public class TigersRage : PassiveAbility
{
    #region VARIABLES
    AudioSource source;
    private Vector3 defaultScale;

    [Header("Stat Adjusments")]
    public float scaleFactor = 1.5f;

    // Currently used as multipliers
    public float maxHealthIncrease = 1f;
    public float defenceIncrease = 1f;
    public float critChanceIncrease = 1f;
    public float critDamageIncrease = 1f;

    PlayerStats stats;
    #endregion
    #region FUNCTIONALITY
    protected override void Fire(Ability ab, GameObject actor)
    {
        // When this is fired, apply the effects / bonuses for the passive ability, call end once duration has passed.

        stats = actor.GetComponent<PlayerStats>();
        source = actor.GetComponent<AudioSource>();

        if (stats != null)
        {
            defaultScale = actor.transform.localScale;      // cache the scale
            actor.transform.localScale *= scaleFactor;      // resize the character

            source.PlayOneShot(abilityActivated);

            // add increased stats
            stats.Health.MaxValue *= maxHealthIncrease;
            stats.Armour.Value *= defenceIncrease;
            stats.CritChance.Value *= critChanceIncrease;
            stats.CritDamage.Value *= critDamageIncrease;
        }

        //Debug.Log("Tigers Rage");
        //throw new System.NotImplementedException();
    }

    public override void EndAbility(GameObject actor)
    {
        if (stats != null)
        {
            Debug.Log("Reset Values");
            actor.transform.localScale = defaultScale;              // Return the character to normal size
    
            // add increased stats
            stats.Health.MaxValue /= maxHealthIncrease;
            stats.Armour.Value /= defenceIncrease;
            stats.CritChance.Value /= critChanceIncrease;
            stats.CritDamage.Value /= critDamageIncrease;
        }
        //base.EndAbility(actor);
    }
    #endregion
}
