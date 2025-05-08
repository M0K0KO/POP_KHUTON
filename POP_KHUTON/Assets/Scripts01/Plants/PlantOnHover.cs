using System;
using DG.Tweening;
using UnityEngine;

public class PlantOnHover : MonoBehaviour
{
    private Plant plant;
    private bool isHovering = false;

    private Camera camera;
    private Vector3 prevPos;
    private float prevOrthoSize;
    
    private void Awake()
    {
        plant = GetComponent<Plant>();
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isHovering)
        {
            prevPos = camera.transform.position;
            prevOrthoSize = camera.orthographicSize;
            
            camera.DOOrthoSize(1.4f, 0.5f).SetEase(Ease.OutCubic);
            Vector3 targetPos = new Vector3(transform.position.x - 5, transform.position.y + 5, transform.position.z - 5);
            Vector3 offSet = new Vector3(0.9f, 1.2f, -1.4f);
            targetPos += offSet;
            camera.transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutCubic);
        }

        if (Input.GetMouseButtonDown(1))
        {
            camera.DOOrthoSize(prevOrthoSize, 0.5f).SetEase(Ease.OutCubic);
            camera.transform.DOMove(prevPos, 0.5f).SetEase(Ease.OutCubic);
        }
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
