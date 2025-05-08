using System;
using UnityEngine;

public class WorldSingleton : MonoBehaviour
{
    public static WorldSingleton instance;

    public GameObject plantDetailWindow;
    
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }
}
