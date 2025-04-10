using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    public GameObject filler;
    public float attackWait;

    public void OnEnable()
    {
        if (attackWait <= 0) { return; }
        StartFill();
    }
    public void StartFill()
    {
        StartCoroutine(HandleFill());
    }
    IEnumerator HandleFill()
    {
        float timer = 0.0f;
        Vector3 start = filler.transform.localScale;
        Vector3 goal = transform.localScale;
        while (timer < attackWait)
        {
            float offset = timer / attackWait;
            transform.localScale = Vector3.Lerp(start, goal, offset);
            timer += Time.deltaTime;
            yield return null;
        }
        filler.transform.localScale = goal;
        Destroy(gameObject);
    }

    //     IEnumerator ApplyKnockback(Vector3 dir)
    // {
    //     float timer = 0.0f;
    //     Vector3 start = transform.position;
    //     Vector3 end = start + dir * knockbackForce;
    //     while (timer < attackWait)
    //     {
    //         float offset = timer / attackWait;
    //         transform.position = Vector3.Lerp(start, end, offset);
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //     transform.position = end;
    //     //applyKnockback = true;
    // }
}
