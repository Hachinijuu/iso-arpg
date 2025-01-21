public class DestructableObject : Hurtbox
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        // Turn off object
        gameObject.SetActive(false);
        // Play destruction emission effect
        // Drop Loot
        // Return to Object Pool


    }
}
