using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public UnityAction AddJob;

    public void OnAddJob()
    {
        AddJob?.Invoke();
    }
}
