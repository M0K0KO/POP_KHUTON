using System;
using UnityEngine;

public class PlantOnHover : MonoBehaviour
{
    private Plant plant;
    public Material material;
    private Color originalOutlineColor;
    private bool isHovering = false;


    private void Awake()
    {
        plant = GetComponent<Plant>();
    }

    void Start()
    {
        material = plant.plantController.currentActiveRenderer.material;
        originalOutlineColor = material.GetColor("_OutlineColor");

        Color invisibleOutline = originalOutlineColor;
        invisibleOutline.a = 0;
        material.SetColor("_OutlineColor", invisibleOutline);
    }

    private void OnMouseEnter()
    {
        isHovering = true;
        Color visibleOutline = originalOutlineColor;
        visibleOutline.a = 1;
        material.SetColor("_OutlineColor", visibleOutline);
    }
    
    private void OnMouseExit()
    {
        isHovering = false;
        Color invisibleOutline = originalOutlineColor;
        invisibleOutline.a = 0;
        material.SetColor("_OutlineColor", invisibleOutline);
    }
}
