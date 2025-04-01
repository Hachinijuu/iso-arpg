using UnityEngine;


[RequireComponent(typeof(EntityStats))]
public class ChestInteract : InteractableObject
{
    [SerializeField] EntityStats stats;

    public void Awake()
    {
        if (stats == null)
            stats = GetComponent<EntityStats>();
    }
    //[SerializeField] Transform dropPosition;
    //public void CreateDroppedItems()
    //{
    //    // Can look to drop system and cycle drop table to create drops
    //    foreach (ItemData item in itemsToDrop)
    //    {
    //        if (dropPosition != null)
    //        {
    //            DropSystem.Instance.CreatedDroppedObject(dropPosition.position, item);
    //        }
    //        else
    //        {
    //            Vector3 placementPos = transform.position + (transform.forward * 0.5f);
    //            DropSystem.Instance.CreatedDroppedObject(placementPos, item);
    //        }
    //    }
    //}

    protected override void InteractAction()
    {
        stats.Health.Value -= 1;    // Kill the chest, will allow the drop system to handle the drop
    }
    //protected override void InteractAction()
    //{
    //    CreateDroppedItems();
    //    // Destroy this object
    //
    //    // Later set active to false once contained in a pool
    //    Destroy(gameObject);
    //}
}
