using System;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public static event Action<Plant> OnPlantCreated;
    
    
    public PlantInfo plantInfo;
    
    // Plant Coordinate은 PlantsManager에서 관리하는 2차원 plant list에서 관리하는 인덱스.
    
    private void Awake()
    {
        plantInfo = GetComponent<PlantInfo>();
    }

    private void Start()
    {
        OnPlantCreated?.Invoke(this);
    }
}
