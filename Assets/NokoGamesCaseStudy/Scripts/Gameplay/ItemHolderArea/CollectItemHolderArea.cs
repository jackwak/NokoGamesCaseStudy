using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItemHolderArea : BaseItemHolderArea
{
    public override Item RemoveItem()
    {
        _transformerMachineController?.StartTransform();
        return base.RemoveItem();
    }
}
