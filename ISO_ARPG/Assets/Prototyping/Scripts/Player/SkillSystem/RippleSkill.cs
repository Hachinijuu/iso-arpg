using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleSkill : Skill
{
    // ideally, system is setup so values can be assigned in editor with ease and skill can be constructed easily - only functionality of the skills will need to be made in script.
    // note: this implementation is temporary.
    [SerializeField]
    GameObject ripplePrefab;

    [SerializeField]
    Vector3 rippleOffset;

    // initalize this skill's attributes
    private void Awake()
    {
        skName = "Ripple";
        skDescription = "Testing ripples";

        cooldown = 2.5f;
        activeTime = 0.5f;
    }

    protected override void SkillAction()
    {
        Debug.Log("Used the skill");
        StartCoroutine(RippleBlocks());
    }

    IEnumerator RippleBlocks()
    {
        if (ripplePrefab != null)
        {
            Vector3 pos = transform.position + rippleOffset;
            GameObject go = GameObject.Instantiate(ripplePrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(activeTime);
            GameObject.Destroy(go);
        }
    }
}
