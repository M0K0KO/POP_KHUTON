using System;
using UnityEngine;

public class PlantInfo : MonoBehaviour
{
    public PlantCoordinate currentCoordinate;
    public PlantType plantType;
    public PlantLevel plantLevel;

    private void Awake()
    {
        currentCoordinate = new PlantCoordinate(0, 0);
    }
}

public struct PlantCoordinate
{
    public int x;
    public int y;

    public PlantCoordinate(int x, int y) { this.x = x; this.y = y; }
}
