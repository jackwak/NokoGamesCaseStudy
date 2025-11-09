using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
    public static ItemPoolManager Instance { get; private set; }

    [System.Serializable]
    public class ItemPoolData
    {
        [Header(" Pool Settings ")]
        public ItemType ItemType;
        public GameObject Prefab;
        public int InitialPoolSize = 10;
        public bool CanExpand = true;
        
        [HideInInspector] public Queue<GameObject> AvailableObjects = new Queue<GameObject>();
        [HideInInspector] public List<GameObject> AllObjects = new List<GameObject>();
    }

    [Header(" References ")]
    [SerializeField] private Transform _poolParent;

    [Header(" Pool Data ")]
    [SerializeField] private List<ItemPoolData> _itemPools = new List<ItemPoolData>();

    [Header(" Datas ")]
    private Dictionary<ItemType, ItemPoolData> _poolDictionary;

    void Awake()
    {
        Instance = this;

        _poolDictionary = new Dictionary<ItemType, ItemPoolData>();
        InitializeAllPools();
    }

    private void InitializeAllPools()
    {
        foreach (ItemPoolData poolData in _itemPools)
        {
            _poolDictionary.Add(poolData.ItemType, poolData);

            for (int i = 0; i < poolData.InitialPoolSize; i++)
            {
                CreateNewObject(poolData);
            }
        }
    }

    private GameObject CreateNewObject(ItemPoolData poolData)
    {
        GameObject obj = Instantiate(poolData.Prefab, _poolParent);
        obj.name = $"{poolData.ItemType}_pooled_{poolData.AllObjects.Count}";
        obj.SetActive(false);
        
        poolData.AvailableObjects.Enqueue(obj);
        poolData.AllObjects.Add(obj);
        
        return obj;
    }

    public GameObject GetItem(ItemType itemType)
    {
        if (!_poolDictionary.ContainsKey(itemType))
        {
            Debug.LogError($"Pool for ItemType {itemType} not found!");
            return null;
        }

        ItemPoolData poolData = _poolDictionary[itemType];

        if (poolData.AvailableObjects.Count == 0)
        {
            if (poolData.CanExpand)
            {
                CreateNewObject(poolData);
            }
            else
            {
                Debug.LogWarning($"Pool for {itemType} is empty and cannot expand!");
                return null;
            }
        }

        GameObject obj = poolData.AvailableObjects.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnItem(GameObject obj)
    {
        if (obj == null) return;

        Item item = obj.GetComponent<Item>();
        if (item == null)
        {
            Debug.LogWarning("Object doesn't have Item component!");
            return;
        }

        ItemType itemType = item.ItemType;

        if (!_poolDictionary.ContainsKey(itemType))
        {
            Debug.LogWarning($"Trying to return item of type {itemType} but pool not found!");
            Destroy(obj);
            return;
        }

        ItemPoolData poolData = _poolDictionary[itemType];

        obj.SetActive(false);
        obj.transform.SetParent(_poolParent);
        poolData.AvailableObjects.Enqueue(obj);
    }
}