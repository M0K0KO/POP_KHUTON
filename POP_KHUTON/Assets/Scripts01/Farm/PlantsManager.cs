using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using InvalidOperationException = System.InvalidOperationException;

public class PlantsManager : MonoBehaviour
{
    private int rowSize = 0;
    private int colSize = 0;
    public Plant[,] plantList;
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

    }
    ////////////////////////////

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

    
    
    //Instantiate 될 때 자동으로 호출
    public void AddPlant(int row, int col)
    {
        GameObject instantiatedPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
        Plant plant = instantiatedPlant.GetComponent<Plant>();
        
        if (plantList[row, col] == null)
        {
            plantList[row, col] = plant;
            Vector3 localPosition = plantPosition(row, col);
                    
            plant.transform.SetParent(transform);
            plant.transform.localPosition = localPosition;
                    
            plant.plantInfo.currentCoordinate = new PlantCoordinate(row, col);

            return;
        }
        return;
    }

    //
    public void UpdatePlant(Plant plant, int row, int col, PlantType plantType, PlantLevel plantLevel)
    {
        if (plantList[row, col] != null)
        {
            plantList[row, col].plantController.ChangeType(plantType);
            plantList[row, col].plantController.ChangeLevel(plantLevel);

            plant.plantController.currentOutline.enabled = false;
        }
    }
}
