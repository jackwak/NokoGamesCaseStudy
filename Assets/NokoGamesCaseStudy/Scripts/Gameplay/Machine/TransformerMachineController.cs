using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TransformerMachineController : BaseMachineController
{
    [Header(" References ")]
    [SerializeField] private ItemHolderArea _dropArea;
    [SerializeField] private Transform _entryTransform;

    [Header(" Datas ")]
    private Vector3 _entryPosition;

    protected override void Awake()
    {
        base.Awake();

        _entryPosition = _entryTransform.position;
    }

    public void StartTransform()
    {
        StartCoroutine(StartTransformCoroutine());
    }

    private IEnumerator StartTransformCoroutine()
    {
        yield return _machineSettings.StartCollectDelay;

        while (true)
        {
            GameObject lastItem = _dropArea.RemoveItem();
            if (lastItem == null) yield break;
            lastItem.transform.DOMove(_entryPosition, _machineSettings.ItemArriveTime).OnComplete(() => TransformItem());

            yield return new WaitForSeconds(_machineSettings.ItemCollectInterval);
        }
    }
}
