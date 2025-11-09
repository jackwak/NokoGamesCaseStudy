using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseMachineController : MonoBehaviour
{
    [Header(" Referenes ")]
    [SerializeField] protected ItemType _spawnedItemType;
    [SerializeField] protected BaseItemHolderArea _collectArea;
    [SerializeField] protected Transform _exitTransform;
    [SerializeField] protected Animator _animator;

    [Header(" Settings ")]
    [SerializeField] protected MachineSettings _machineSettings;

    [Header(" Datas ")]
    protected Vector3 _exitPosition;
    protected Transform _collectAreaItemHolder;
    protected bool _isWorking = false;
    private int _isWorkingHash;

    protected virtual void Awake()
    {
        _exitPosition = _exitTransform.position;
        _collectAreaItemHolder = _collectArea.transform.GetChild(1);
        _isWorkingHash = Animator.StringToHash("IsWorking");
    }

    protected Vector3? HasAvaiblePosition()
    {
        return _collectArea.GetAvaiblePosition();
    }

    protected void TransformItem(Vector3? itemDropPosition)
    {
        GameObject itemObj = ItemPoolManager.Instance.GetItem(_spawnedItemType);
        Item transformedItem = itemObj.GetComponent<Item>();
        Transform itemTransform = itemObj.transform;

        itemTransform.position = _exitPosition;
        itemTransform.SetParent(_collectAreaItemHolder);
        itemTransform.DORotate(Vector3.zero, _machineSettings.ItemArriveTime);
        itemTransform.DOJump((Vector3)itemDropPosition, 1f, 1, _machineSettings.ItemArriveTime);
        _collectArea.AddItem(transformedItem);
    }

    protected void SetMachineState(bool isWorking)
    {
        if (_isWorking == isWorking) return;

        _isWorking = isWorking;
        _animator?.SetBool(_isWorkingHash, _isWorking);
    }
    
    protected void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
