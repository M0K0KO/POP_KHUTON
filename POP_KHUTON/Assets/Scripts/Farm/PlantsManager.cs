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

        plantPosition(0, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject testPlant = Instantiate(plantPrefab, Vector3.zero, Quaternion.identity);
            AddPlant(testPlant.GetComponent<Plant>());
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
        Vector2Int position = AddPlant(plant);
    }

    // Farm Breadth, Width 값 기반으로 한 2차원 배열 PlantList생성
    private void MakePlantsList()
    {
        plantList = new Plant[(int)farm.farmBreadth, (int)farm.farmWidth];
    }
    
    private Vector3 plantPosition(int x, int y)
    {
        float cellSize = 1f;
        
        Vector3 topLeftCorner = new Vector3(
            transform.position.x - (farm.farmBreadth / 2f), 
            transform.position.y,
            transform.position.z - (farm.farmWidth / 2f));
        
        float normalizedX = (topLeftCorner.x + ((cellSize / 2f) * (1 + (x * cellSize))));
        float normalizedZ = (topLeftCorner.z + ((cellSize / 2f) * (1 + (y * cellSize))));

        Renderer renderer = GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;

        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minZ = bounds.min.z;
        float maxZ = bounds.max.z;

        float resultX = ((normalizedX - minX) / (maxX - minX)) - 0.5f;
        float resultZ = ((normalizedZ - minZ) / (maxZ - minZ)) - 0.5f;
        
        return new Vector3(resultX, transform.position.y + 0.5f, resultZ);
    }

    
    public Vector2Int AddPlant(Plant plant)
    {
        for (int y = 0; y < farm.farmBreadth; y++)
        {
            for (int x = 0; x < farm.farmWidth; x++)
            {
                if (plantList[y, x] == null)
                {
                    plantList[y, x] = plant;
                
                    Vector3 localPosition = plantPosition(x, y);
                
                    if (plant.transform.parent != transform)
                    {
                        Vector3 worldPos = plant.transform.position;
                        plant.transform.SetParent(transform);
                    
                        plant.transform.localPosition = localPosition;
                    }
                    else
                    {
                        plant.transform.localPosition = localPosition;
                    }
                
                    if (plant.TryGetComponent(out PlantInfo plantInfo))
                    {
                        plantInfo.currentCoordinate = new PlantCoordinate(x, y);
                    }
                
                    Debug.Log($"식물이 ({x}, {y}) 위치에 추가되었습니다. 로컬 위치: {localPosition}");
                    return new Vector2Int(x, y); 
                }
            }
        }
    
        // 빈 공간이 없는 경우
        throw new InvalidOperationException("모든 그리드 공간이 채워져 있습니다. 더 이상 식물을 추가할 수 없습니다.");
    }
}
