using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SpawnerMachineController : BaseMachineController
{
    [Header(" Settings ")]
    [SerializeField] private float _spawnInterval;

    void Start()
    {
        StartCoroutine(TransformItem());
    }

    private IEnumerator TransformItem()
    {
        do
        {
            Vector3? position = HasAvaiblePosition();
            if (position != null)
            {
                TransformItem(position);
                SetMachineState(true);
            }
            else
            {
                SetMachineState(false);
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
        while (true);
    }
}