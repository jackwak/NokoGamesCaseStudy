using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private ItemType _itemType;

    [Header(" Settings ")]
    [SerializeField] private float _carryYRotation = 0;

    [Header(" Datas ")]
    private float _rendererYSize;

    public ItemType ItemType => _itemType;
    public float RendererYSize => _rendererYSize;
    public float CarryYRotation => _carryYRotation;

    void Awake()
    {
        _rendererYSize = transform.GetChild(0).GetComponent<Renderer>().bounds.size.y;
    }

    public void ReturnToPool()
    {
        ItemPoolManager.Instance.ReturnItem(gameObject);
    }
}
