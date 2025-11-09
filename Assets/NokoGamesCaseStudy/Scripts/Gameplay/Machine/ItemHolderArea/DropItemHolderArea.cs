using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemHolderArea : BaseItemHolderArea
{
    [SerializeField] private TransformerMachineController _transformerMachineController;

    public override void AddItem(Item item)
    {
        base.AddItem(item);
        _transformerMachineController?.StartTransform();
    }
}
