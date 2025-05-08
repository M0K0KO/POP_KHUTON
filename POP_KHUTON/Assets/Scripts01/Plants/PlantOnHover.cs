using System;
using UnityEngine;

public class PlantOnHover : MonoBehaviour
{
    private Plant plant;
    private bool isHovering = false;


    private void Awake()
    {
        plant = GetComponent<Plant>();
    }

    private void Update()
    {
        Debug.Log(isHovering);
    }

    private void OnMouseEnter()
    {
        isHovering = true;
        plant.plantController.currentOutline.enabled = true;
    }
    
    private void OnMouseExit()
    {
        isHovering = false;
        plant.plantController.currentOutline.enabled = false;
    }
}
