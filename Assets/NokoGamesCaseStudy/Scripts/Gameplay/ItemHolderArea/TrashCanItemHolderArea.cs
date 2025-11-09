using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCanItemHolderArea : BaseItemHolderArea
{
    [Header(" Trash Can Settings ")]
    [SerializeField] private float _destroyDelay = 0.3f;
    [SerializeField] private bool _acceptAllItemTypes = true;

    public override void AddItem(Item item)
    {
        base.AddItem(item);
        StartCoroutine(DestroyItemCoroutine(item));
    }

    public override bool IsCorrectArea(ItemType itemType)
    {
        if (_acceptAllItemTypes) return true;
        
        return base.IsCorrectArea(itemType);
    }

    private IEnumerator DestroyItemCoroutine(Item item)
    {
        yield return new WaitForSeconds(_destroyDelay);

        if (item != null && ItemCount > 0)
        {
            _itemStack.Pop();
            item.ReturnToPool();
        }
    }
}