using System;
using UnityEngine;

public class Farm : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public PlantsManager plantsManager;
    
    public float farmBreadth;
    public float farmWidth;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        plantsManager = GetComponent<PlantsManager>();
        
        farmBreadth = meshRenderer.bounds.size.x;
        farmWidth = meshRenderer.bounds.size.z;
    }

    private void Start()
    {

    }
}
