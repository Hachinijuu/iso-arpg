using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyControllerV2
{
    [SerializeField] GameObject shotPrefab;
    [SerializeField] Transform shootLocation;
    public override void Attack()
    {
        if (shotPrefab == null) { Debug.Log("Ranged Enemy Missing Shot Prefab"); return; } 
        if (canAttack)
        {
            // Perform the attack (play the animation)
            canAttack = false;

            GameObject firedProjectile = GameObject.Instantiate(shotPrefab);
            Projectile p = firedProjectile.GetComponent<Projectile>();
            if (firedProjectile != null && p != null)
            {
                firedProjectile.transform.position = shootLocation.transform.position;
                firedProjectile.transform.rotation = shootLocation.transform.rotation;
                p.FireProjectile();
            }
            //Debug.Log("Performed Attack");
            animator.SetTrigger(animId);

            //hitbox.AttackForTime(hitboxUptime);
            //hitbox.AllowDamageForTime(hitboxUptime);
            //Invoke(nameof(ResetAttack), attackCooldown);
            StartCoroutine(AttackTimer());
        }
        base.Attack();
    }
}
