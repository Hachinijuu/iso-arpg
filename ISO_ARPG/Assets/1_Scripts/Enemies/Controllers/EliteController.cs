using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteController : EnemyControllerV2
{
    [SerializeField] GameObject shotPrefab;
    [SerializeField] Transform shootLocation;

    protected static string RangedTrigger = "RangedAttack";
    protected static string SpecialTrigger = "SpecialAttack";

    protected int rangedID = Animator.StringToHash(RangedTrigger);
    protected int specialID = Animator.StringToHash(SpecialTrigger);


    [SerializeField] GameObject specialShot;
    [SerializeField] float attackDelay = 0.5f;

    public virtual void RangedAttack()
    {
        // Standard, ranged attack
        if (shotPrefab == null) { Debug.Log("Ranged Enemy Missing Shot Prefab"); return; }
        if (canAttack)
        {
            canAttack = false;
            GameObject firedProjectile = GameObject.Instantiate(shotPrefab);
            Projectile p = firedProjectile.GetComponent<Projectile>();
            if (firedProjectile != null && p != null)
            {
                firedProjectile.transform.position = shootLocation.transform.position;
                firedProjectile.transform.position = shootLocation.transform.position;
                p.FireProjectile();
            }

            animator.SetTrigger(rangedID);
            StartCoroutine(AttackTimer());
        }
    }
    public virtual void SpecialAttack(Transform launchLocation)
    {
        // Special, volley launcher, red indicated attack
        if (canAttack)
        {
            canAttack = false;
            // Circle creation
            // Build the damage volume at this location after the time has passed

            animator.SetTrigger(specialID);
            StartCoroutine(AttackTimer());
        }
    }

    IEnumerator DelayedAttack()
    { 
        yield return new WaitForSeconds(attackDelay);
        // Standard, ranged attack
        if (specialShot == null) { Debug.Log("Ranged Enemy Missing Shot Prefab"); yield break; }
        GameObject firedProjectile = GameObject.Instantiate(specialShot);
        // Tell the hitbox
        Hitbox hb = firedProjectile.GetComponent<Hitbox>();
        hb.AllowDamageForTime(hitboxUptime);
    }
}
