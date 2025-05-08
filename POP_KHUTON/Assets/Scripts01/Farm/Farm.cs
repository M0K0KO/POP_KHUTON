using System;
using UnityEngine;

public class Farm : MonoBehaviour
{
    public static Farm instance;
    
    public MeshRenderer meshRenderer;
    public PlantsManager plantsManager;
    
    public float farmBreadth;
    public float farmWidth;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        meshRenderer = GetComponent<MeshRenderer>();
        plantsManager = GetComponent<PlantsManager>();
        
        farmWidth = meshRenderer.bounds.size.x;
        farmBreadth = meshRenderer.bounds.size.z;
    }

    private void Start()
    {

    }
}
