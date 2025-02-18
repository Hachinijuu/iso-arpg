using UnityEngine;

[CreateAssetMenu(fileName = "PhoenixSwiftness", menuName = "sykcorSystems/Abilities/Hunter/PhoenixSwiftness", order = 4)]
public class PhoenixSwiftness : PassiveAbility
{
    #region VARIABLES
    public float movespeedIncrease = 1f;
    public int dodgeIncrease = 1;
    public int projectileIncrease = 1;

    AudioSource source;
    PlayerStats stats;

    PlayerStats defaultCopy;
    #endregion
    #region FUNCTIONALITY
    public override void InitAbility(Ability ab, GameObject actor)
    {
        stats = actor.GetComponent<PlayerStats>();
        source = actor.GetComponent<AudioSource>();

        defaultCopy = new PlayerStats();
    }

    protected override void Fire(Ability ab, GameObject actor)
    {
        if (stats != null)
        {
            if (abilityActivated)
                source.PlayOneShot(abilityActivated);

            // create a copy of the stats, to revert back to
            defaultCopy.CopyFromStats(stats);

            // add the increased stats
            stats.MoveSpeed.Value *= movespeedIncrease;
            stats.Dodge.Value += dodgeIncrease;
            stats.Projectiles.Value += projectileIncrease;
        }
    }

    public override void EndAbility()
    {
        stats.CopyFromStats(defaultCopy);
    }
    #endregion
}
