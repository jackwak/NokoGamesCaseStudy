using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TransformerMachineController : BaseMachineController
{
    [Header(" References ")]
    [SerializeField] private BaseItemHolderArea _dropArea;
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
        if (_isWorking) return;
        StartCoroutine(StartTransformCoroutine());
    }

    private IEnumerator StartTransformCoroutine()
    {
        yield return _machineSettings.StartCollectDelay;

        SetMachineState(true);

        while (true)
        {
            Vector3? itemDropPosition = HasAvaiblePosition();
            Item lastItem = _dropArea.GetLastItem();

            if (lastItem == null || itemDropPosition == null)
            {
                SetMachineState(false);
                yield break;
            }

            lastItem = _dropArea.RemoveItem();
            lastItem.transform.SetParent(_collectArea.ItemHolderTransform);
            lastItem.transform.DOLocalRotate(Vector3.zero, _machineSettings.ItemArriveTime);
            lastItem.transform.DOJump(_entryPosition, 1f, 1, _machineSettings.ItemArriveTime).OnComplete(() =>
            {
                TransformItem(itemDropPosition);
                lastItem.ReturnToPool();
            });
            yield return new WaitForSeconds(_machineSettings.ItemCollectInterval);
        }

    }
}
