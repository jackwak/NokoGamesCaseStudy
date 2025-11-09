using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolderArea : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private Renderer _areaRenderer;
    [SerializeField] private GameObject _spawnedObject;
    [SerializeField] private Transform _itemHolderTransform;
    [SerializeField] private TransformerMachineController _transformerMachineController;

    [Header(" Settings ")]
    [SerializeField] private ItemHolderType _itemHolderType;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _heightCount;
    [SerializeField] private int _xCount;
    [SerializeField] private int _zCount;
    [SerializeField] private float _xSpacing;
    [SerializeField] private float _zSpacing;

    private List<Vector3> _itemPositions;
    private Stack<Item> _itemStack;

    public int ItemCount => _itemStack.Count;
    public ItemHolderType ItemHolderType => _itemHolderType;
    public ItemType ItemType => _itemType;
    public Transform ItemHolderTransform => _itemHolderTransform;

    private void Awake()
    {
        _itemPositions = new List<Vector3>();
        _itemStack = new Stack<Item>();
    }

    private void Start()
    {
        InitializeItemPositions(_spawnedObject.transform.GetChild(0).GetComponent<Renderer>().bounds.size);
    }

    public void InitializeItemPositions(Vector3 itemSize)
    {
        _itemPositions.Clear();
        _itemStack.Clear();

        float totalWidth = (_xCount - 1) * (itemSize.x + _xSpacing) + itemSize.x;
        float totalDepth = (_zCount - 1) * (itemSize.z + _zSpacing) + itemSize.z;
        Vector3 holderCenter = _areaRenderer.bounds.center;

        Vector3 startPos = new Vector3(
            holderCenter.x - (totalWidth / 2f) + (itemSize.x / 2f),
            _areaRenderer.bounds.min.y,
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

                    _itemPositions.Add(position);
                }
            }
        }
    }

    public bool IsCorrectArea(ItemType itemType)
    {
        if (_itemType == itemType) return true;
        else return false;
    }

    public void AddItem(Item item)
    {
        _itemStack.Push(item);

        _transformerMachineController?.StartTransform();
    }

    public Item RemoveItem()
    {
        if (ItemCount > 0)
        {
            return _itemStack.Pop();
        }

        return null;
    }

    public Vector3? GetAvaiblePosition()
    {
        if (ItemCount >= _itemPositions.Count)
        {
            return null;
        }

        Vector3 nextPos = _itemPositions[_itemStack.Count];
        return nextPos;
    }

    public Item GetLastItem()
    {
        if (ItemCount > 0)
        {
            return _itemStack.Peek();
        }

        return null;
    }
}
