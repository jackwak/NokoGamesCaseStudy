using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolderArea : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private Renderer _holderArea;
    [SerializeField] private GameObject _areaHolderObject;


    [Header(" Settings ")]
    [SerializeField] private int _heightCount;
    [SerializeField] private int _xCount;
    [SerializeField] private int _zCount;

    [SerializeField] private float _xSpacing;
    [SerializeField] private float _zSpacing;

    [Header(" Datas ")]
    private List<ItemData> _itemDatas;

    public class ItemData
    {
        public Vector3 ItemPosition;
        public bool PositionOccupied;
        public GameObject ItemObject;
    }

    private void Awake()
    {
        _itemDatas = new List<ItemData>();
    }

    void Start()
    {
        InitializeItemPositions(_areaHolderObject.transform.GetChild(0).GetComponent<Renderer>().bounds.size);
    }

    public void InitializeItemPositions(Vector3 itemSize)
    {
        _itemDatas.Clear();

        float totalWidth = (_xCount - 1) * (itemSize.x + _xSpacing) + itemSize.x;
        float totalDepth = (_zCount - 1) * (itemSize.z + _zSpacing) + itemSize.z;

        Vector3 holderCenter = _holderArea.bounds.center;

        Vector3 startPos = new Vector3(
            holderCenter.x - (totalWidth / 2f) + (itemSize.x / 2f),
            _holderArea.bounds.min.y,
            holderCenter.z - (totalDepth / 2f) + (itemSize.z / 2f)
        );

        for (int y = 0; y < _heightCount; y++)
        {
            for (int z = 0; z < _zCount; z++)
            {
                for (int x = 0; x < _xCount; x++)
                {
                    Vector3 position = new Vector3(
                        startPos.x + (x * (itemSize.x + _xSpacing)),
                        startPos.y + (y * itemSize.y),
                        startPos.z + (z * (itemSize.z + _zSpacing))
                    );

                    ItemData data = new ItemData
                    {
                        ItemPosition = position,
                        PositionOccupied = false,
                        ItemObject = null
                    };

                    _itemDatas.Add(data);
                }
            }
        }
    }

    public Vector3? GetAvailablePosition()
    {
        for (int i = 0; i < _itemDatas.Count; i++)
        {
            if (!_itemDatas[i].PositionOccupied)
            {
                _itemDatas[i].PositionOccupied = true;
                return _itemDatas[i].ItemPosition;
            }
        }

        return null;
    }

    public Vector3? AddItem(GameObject item)
    {
        for (int i = 0; i < _itemDatas.Count; i++)
        {
            if (!_itemDatas[i].PositionOccupied)
            {
                _itemDatas[i].PositionOccupied = true;
                _itemDatas[i].ItemObject = item;
                return _itemDatas[i].ItemPosition;
            }
        }

        return null;
    }

    public GameObject RemoveLastItem()
    {
        for (int i = _itemDatas.Count - 1; i >= 0; i--)
        {
            if (_itemDatas[i].PositionOccupied && _itemDatas[i].ItemObject != null)
            {
                GameObject item = _itemDatas[i].ItemObject;
                
                _itemDatas[i].PositionOccupied = false;
                _itemDatas[i].ItemObject = null;

                return item;
            }
        }

        return null;
    }

    public void ReleasePosition(Vector3 position)
    {
        for (int i = 0; i < _itemDatas.Count; i++)
        {
            if (Vector3.Distance(_itemDatas[i].ItemPosition, position) < 0.01f)
            {
                _itemDatas[i].PositionOccupied = false;
                _itemDatas[i].ItemObject = null;
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_itemDatas == null || _itemDatas.Count == 0) return;

        foreach (var data in _itemDatas)
        {
            Gizmos.color = data.PositionOccupied ? Color.red : Color.green;
            Gizmos.DrawWireSphere(data.ItemPosition, 0.1f);
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Test - Spawn Objects")]
    private void TestSpawn()
    {
        if (_areaHolderObject != null)
        {
            _itemDatas = new List<ItemData>();

            Renderer testRenderer = _areaHolderObject.transform.GetChild(0).GetComponent<Renderer>();
            if (testRenderer != null)
            {
                Vector3 itemSize = testRenderer.bounds.size;
                InitializeItemPositions(itemSize);
                SpawnTestObjects();
            }
        }
    }

    private void SpawnTestObjects()
    {
        foreach (var data in _itemDatas)
        {
            GameObject obj = Instantiate(_areaHolderObject, data.ItemPosition, Quaternion.identity, transform);
            AddItem(obj);
        }
    }

    [ContextMenu("Test - Clear Objects")]
    private void TestClear()
    {
        for (int i = transform.childCount - 1; i >= 1; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        _itemDatas.Clear();
    }

    [ContextMenu("Test - Remove Last Item")]
    private void TestRemoveLastItem()
    {
        GameObject lastItem = RemoveLastItem();
        if (lastItem != null)
        {
            Debug.Log($"Çıkarılan item: {lastItem.name}");
            DestroyImmediate(lastItem);
        }
        else
        {
            Debug.Log("Çıkarılacak item yok!");
        }
    }
#endif
}