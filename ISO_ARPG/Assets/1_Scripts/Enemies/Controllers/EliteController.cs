using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EliteController : EnemyControllerV2
{
    [SerializeField] GameObject shotPrefab;
    [SerializeField] Transform shootLocation;

    protected static string RangedTrigger = "RangedAttack";
    protected static string SpecialTrigger = "SpecialAttack";

    protected int rangedID = Animator.StringToHash(RangedTrigger);
    protected int specialID = Animator.StringToHash(SpecialTrigger);


    [SerializeField] AttackIndicator attackIndicator;
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

            //     // Perform the attack (play the animation)
            // canAttack = false;

            // GameObject firedProjectile = GameObject.Instantiate(shotPrefab);
            // Projectile p = firedProjectile.GetComponent<Projectile>();
            // if (firedProjectile != null && p != null)
            // {
            //     firedProjectile.transform.position = shootLocation.transform.position;
            //     firedProjectile.transform.rotation = shootLocation.transform.rotation;
            //     p.FireProjectile();
            // }
            // //Debug.Log("Performed Attack");
            // animator.SetTrigger(animId);

            // //hitbox.AttackForTime(hitboxUptime);
            // //hitbox.AllowDamageForTime(hitboxUptime);
            // //Invoke(nameof(ResetAttack), attackCooldown);
            // StartCoroutine(AttackTimer());
    public virtual void SpecialAttack(Transform launchLocation)
    {
        // Special, volley launcher, red indicated attack
        if (canAttack)
        {
            canAttack = false;
            // Circle creation
            // Build the damage volume at this location after the time has passed
            animator.SetTrigger(specialID);
            Vector3 pos = launchLocation.position;
            pos.y = 0f;
            StartCoroutine(DelayedAttack(pos));
            StartCoroutine(AttackTimer());
        }
    }

    IEnumerator DelayedAttack(Vector3 pos)
    { 
        GameObject indicator = GameObject.Instantiate(attackIndicator.gameObject);
        attackIndicator.attackWait = attackDelay;
        indicator.transform.position = pos;
        yield return new WaitForSeconds(attackDelay);
        // Standard, ranged attack
        if (specialShot == null) { Debug.Log("Ranged Enemy Missing Shot Prefab"); yield break; }
        GameObject firedProjectile = GameObject.Instantiate(specialShot);
        firedProjectile.transform.position = pos;
        // Tell the hitbox
        Hitbox hb = firedProjectile.GetComponent<Hitbox>();
        hb.AllowDamageForTime(hitboxUptime);
        Destroy(firedProjectile, hitboxUptime);
    }
}
