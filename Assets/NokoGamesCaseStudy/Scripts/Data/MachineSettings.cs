using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/Machine_Settings")]
public class MachineSettings : ScriptableObject
{
    [SerializeField] private float _itemCollectInterval = .15f;
    [SerializeField] private float _startCollectDelay = .2f;
    [SerializeField] private float _itemArriveTime = .1f;

    public float ItemCollectInterval => _itemCollectInterval;
    public float StartCollectDelay => _startCollectDelay;
    public float ItemArriveTime => _itemArriveTime;
}
