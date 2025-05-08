using UnityEngine;

public class PlantInfo : MonoBehaviour
{
    public PlantCoordinate currentCoordinate;
}

public struct PlantCoordinate
{
    public int x;
    public int y;

    public PlantCoordinate(int x, int y) { this.x = x; this.y = y; }
}
