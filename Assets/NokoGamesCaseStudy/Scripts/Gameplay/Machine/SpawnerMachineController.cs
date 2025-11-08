using DG.Tweening;
using UnityEngine;

public class SpawnerMachineController : BaseMachineController
{
    [Header(" Settings ")]
    [SerializeField] private float _spawnInterval;

    void Start()
    {
        InvokeRepeating(nameof(TransformItem), _spawnInterval, _spawnInterval);
    }
}