using UnityEngine;

[CreateAssetMenu(fileName = "TigersRage", menuName = "sykcorSystems/Abilities/Berserker/TigersRage", order = 4)]
public class TigersRage : IdentityAbility
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
    PlayerStats defaultCopy;
    PlayerController controller;

    #endregion
    #region FUNCTIONALITY
    public override void InitAbility(Ability ab, GameObject actor)
    {
        stats = actor.GetComponent<PlayerStats>();
        source = actor.GetComponent<AudioSource>();
        controller = actor.GetComponent<PlayerController>();

        defaultCopy = stats;

        if (asFusion)
        {
            // stop the ability from being timed out, reduce the stat buff values to be expected amounts
        }
    }
    protected override void Fire(Ability ab, GameObject actor)
    {
        // When this is fired, apply the effects / bonuses for the passive ability, call end once duration has passed.
        //Debug.Log("fired");
        if (stats != null)
        {
            defaultScale = actor.transform.localScale;      // cache the scale
            actor.transform.localScale *= scaleFactor;      // resize the character

            defaultCopy.CopyFromStats(stats);

            if (abilityActivated)
                source.PlayOneShot(abilityActivated);

            controller.SetAura(true);

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

            controller.SetAura(false);

            //stats.CopyFromStats(defaultCopy);
            //Debug.Log("What is the health:" + stats.Health.MaxValue);

            //// remove stats
            stats.Health.MaxValue /= maxHealthIncrease;
            stats.Armour.Value /= defenceIncrease;
            stats.CritChance.Value /= critChanceIncrease;
            stats.CritDamage.Value /= critDamageIncrease;
        }

        // Don't forget to call end ability to send event out
        base.EndAbility(actor);
    }
    #endregion
}
