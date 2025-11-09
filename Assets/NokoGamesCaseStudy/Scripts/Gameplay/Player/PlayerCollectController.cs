using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.IO.Compression;

public class PlayerCollectController : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private Transform _itemHolderTransform;

    [Header(" Settings ")]
    [SerializeField] private float _itemCollectInterval = .15f;
    [SerializeField] private float _itemArriveTime = .1f;
    [SerializeField] private int _maxItemCollectCount = 10;

    [Header(" Datas ")]
    private Stack<Item> _items;


    public int ItemCount => _items.Count;
    public Stack<Item> Items => _items;
    public int MaxItemCollectCount => _maxItemCollectCount;
    public ItemType? CurrentItemType
    {
        get
        {
            if (_items.Count > 0)
            {

                return _items.Peek().ItemType;
            }

            return null;
        }
    }

    public Item GetTopItem()
    {
        if (_items.Count > 0)
            return _items.Peek();
        return null;
    }

    void Awake()
    {
        _items = new Stack<Item>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseItemHolderArea itemHolderArea))
        {
            if (itemHolderArea.ItemHolderType == ItemHolderType.Collect)
            {
                StartCoroutine(StartCollect(itemHolderArea));
            }
            else if (itemHolderArea.ItemHolderType == ItemHolderType.Drop)
            {
                StartCoroutine(StartDrop(itemHolderArea));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
    }

    private IEnumerator StartCollect(BaseItemHolderArea itemHolderArea)
    {
        do
        {
            yield return new WaitForSeconds(_itemCollectInterval);
            AddItem(itemHolderArea);
        }
        while (true);
    }

    private IEnumerator StartDrop(BaseItemHolderArea itemHolderArea)
    {
        do
        {
            yield return new WaitForSeconds(_itemCollectInterval);
            RemoveItem(itemHolderArea);
        }
        while (true);
    }

    private void AddItem(BaseItemHolderArea itemHolderArea)
    {
        Item item = itemHolderArea.GetLastItem();
        if (item == null || _maxItemCollectCount <= ItemCount || ((ItemCount > 0) && (CurrentItemType != itemHolderArea.ItemType))) return;

        float itemPosY = ItemCount * item.RendererYSize;
        item.transform.SetParent(_itemHolderTransform);
        Vector3 itemPosition = Vector3.up * itemPosY;
        Debug.Log(itemPosition);

        itemHolderArea.RemoveItem();
        _items.Push(item);

        item.transform.DOLocalRotate(Vector3.up * item.CarryYRotation, _itemArriveTime);
        item.transform.DOLocalJump(itemPosition, 1f, 1, _itemArriveTime);
    }

    public void RemoveItem(BaseItemHolderArea itemHolderArea)
    {
        Vector3? targetPosition = itemHolderArea.GetAvaiblePosition();
        if (_items.Count <= 0 || targetPosition == null) return;

        Item lastItem = _items.Peek();
        if (!itemHolderArea.IsCorrectArea(lastItem.ItemType)) return;

        _items.Pop();

        lastItem.transform.SetParent(itemHolderArea.ItemHolderTransform);
        lastItem.transform.DOJump((Vector3)targetPosition, 1f, 1, _itemArriveTime).OnComplete(()=> itemHolderArea.AddItem(lastItem));
        lastItem.transform.DOLocalRotate(Vector3.zero, _itemArriveTime);
    }
}
