using UnityEngine;

public class DestructibleObject : Hurtbox
{
    // Add this script to destructible objects, this is a hurtbox that has semi-extended functionality
    // Only use this so that there is not unnecessary listening outside of it

    // This class can play it's own effects instead of having an external class handle it

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip destroySound;
    //[SerializeField] Animator anim;
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        // Turn off object

        // Include a random volume / pitch to modify the sound on destruction
        if (aSource != null && destroySound != null)
            aSource.PlayOneShot(destroySound);


        gameObject.SetActive(false);
        // Play destruction emission effect

        // Loot is dropped by drop manager

        // Drop Loot
        // Return to Object Pool
    }
}
