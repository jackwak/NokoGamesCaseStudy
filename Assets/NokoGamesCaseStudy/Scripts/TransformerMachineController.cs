using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TransformerMachineController : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private ItemHolderArea _dropArea;
    [SerializeField] private ItemHolderArea _collectArea;
    [SerializeField] private GameObject _transformedItem;
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    [Header(" Settings ")]
    [SerializeField] private float _itemCollectInterval = .15f;
    [SerializeField] private float _startCollectDelay = .2f;
    [SerializeField] private float _itemArriveTime = .1f;

    //Properties
    private Vector3 EntryPosition => _entryTransform.position;
    private Vector3 ExitPosition => _exitTransform.position;
    private Transform CollectAreaItemHolder => _collectArea.transform.GetChild(1);

    public void StartTransform()
    {
        StartCoroutine(StartTransformCoroutine());
    }

    private IEnumerator StartTransformCoroutine()
    {
        yield return _startCollectDelay;

        while (true)
        {
            GameObject lastItem = _dropArea.RemoveItem();
            if (lastItem == null) yield break;
            lastItem.transform.DOMove(EntryPosition, _itemArriveTime).OnComplete(() => TransformItem());

            yield return new WaitForSeconds(_itemCollectInterval);
        }
    }

    private void TransformItem()
    {
        Vector3? itemDropPosition = _collectArea.GetAvaiblePosition();
        if (itemDropPosition == null) return;

        GameObject transformedItem = Instantiate(_transformedItem, CollectAreaItemHolder);

        transformedItem.transform.position = ExitPosition;
        transformedItem.transform.DOMove((Vector3)itemDropPosition, _itemArriveTime).OnComplete(() => _collectArea.AddItem(transformedItem));
    }
}
