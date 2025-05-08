using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using InvalidOperationException = System.InvalidOperationException;

public class PlantsManager : MonoBehaviour
{
    private Plant[,] plantList;
    private Farm farm;

    public GameObject plantPrefab;
    
    private void Awake()
    {
        farm = GetComponent<Farm>();
    }

    private void Start()
    {
        MakePlantsList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject instantiatedPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnEnable()
    {
        Plant.OnPlantCreated += HandlePlantCreated;
    }

    private void OnDisable()
    {
        Plant.OnPlantCreated -= HandlePlantCreated;
    }

    private void HandlePlantCreated(Plant plant)
    {
        AddPlant(plant);
    }

    // Farm Breadth, Width 값 기반으로 한 2차원 배열 PlantList생성
    private void MakePlantsList()
    {
        plantList = new Plant[(int)farm.farmWidth, (int)farm.farmBreadth];
    }
    
    private Vector3 plantPosition(int x, int z)
    {
        float cellSize = 1f;
        
        Vector3 topLeftCorner = new Vector3(
            transform.position.x - (farm.farmWidth / 2f), 
            transform.position.y,
            transform.position.z - (farm.farmBreadth / 2f));

        float normalizedX = (topLeftCorner.x + ((cellSize / 2f) + (x * cellSize))) / farm.farmWidth;
        float normalizedZ = (topLeftCorner.z + ((cellSize / 2f) + (z * cellSize))) / farm.farmBreadth;

        Debug.Log(topLeftCorner);
        
        return new Vector3(normalizedX, transform.position.y + 0.5f, normalizedZ);
    }

    
    public void AddPlant(Plant plant)
    {
        for (int x = 0; x < farm.farmWidth; x++)
        {
            for (int y = 0; y < farm.farmBreadth; y++)
            {
                if (plantList[x, y] == null)
                {
                    plantList[x, y] = plant;
                    Vector3 localPosition = plantPosition(x, y);
                    
                    plant.transform.SetParent(transform);
                    plant.transform.localPosition = localPosition;
                    
                    plant.plantInfo.currentCoordinate = new PlantCoordinate(x, y);
                    Debug.Log($"식물이 ({x}, {y}) 위치에 추가되었습니다. 로컬 위치: {localPosition}");

                    return;
                }
            }
        }
    }
}
