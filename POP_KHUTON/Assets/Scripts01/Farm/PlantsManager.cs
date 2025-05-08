using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using InvalidOperationException = System.InvalidOperationException;

public class PlantsManager : MonoBehaviour
{
    private int rowSize = 0;
    private int colSize = 0;
    private Plant[,] plantList;
    private Farm farm;
    
    private float cellSize = 2f;

    public GameObject plantPrefab;
    
    private void Awake()
    {
        farm = GetComponent<Farm>();
    }

    private void Start()
    {
        MakePlantsList();
    }

    ////////////////////////////
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject instantiatedPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
        }
    }
    ////////////////////////////

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
        // 이후 받은 정보로 초기화 할 예정
        plant.plantController.ChangeType(PlantType.Cabbage);
        plant.plantController.ChangeLevel(PlantLevel.Lv3);
        AddPlant(plant);
        // 이후 받은 정보로 초기화 할 예정
    }

    private void MakePlantsList()
    {
        rowSize = (int)(farm.farmWidth / cellSize);
        colSize = (int)(farm.farmBreadth / cellSize);
        plantList = new Plant[rowSize, colSize];
    }
    
    private Vector3 plantPosition(int x, int z)
    {
        Vector3 topLeftCorner = new Vector3(
            transform.position.x - (farm.farmWidth / 2f), 
            transform.position.y,
            transform.position.z - (farm.farmBreadth / 2f));

        float normalizedX = (topLeftCorner.x + ((cellSize / 2f) + (x * cellSize))) / farm.farmWidth;
        float normalizedZ = (topLeftCorner.z + ((cellSize / 2f) + (z * cellSize))) / farm.farmBreadth;

        Debug.Log(topLeftCorner);
        
        return new Vector3(normalizedX, transform.position.y + 0.5f, normalizedZ);
    }

    
    
    // 이후 받는 정보 기반으로 배치할 예정
    public void AddPlant(Plant plant)
    {
        for (int x = 0; x < rowSize; x++)
        {
            for (int y = 0; y < colSize; y++)
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

        return;
    }
}
