using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DepthController : MonoBehaviour
{
    public float unitToMeterRatio = 1;
    public bool invertDepth = true;

    [SerializeField] private float zeroDepthDepth = 0;
    [SerializeField] private Transform zeroDepthReference;

    [Header("Calculated Depths")] //
    [SerializeField]
    private float maxSwarmDepthUnits;

    [SerializeField] private float maxSwarmDepthMeters;
    [SerializeField] private float swarmDepthMeters;
    [SerializeField] private float swarmDepthUnits;
    [SerializeField] private float cameraDepthUnits;

    [Header("UI")] //
    [Range(0, 1)]
    public float uiUpdateInterval = 0.5f;

    private float _uiUpdateTimer;
    [SerializeField] private TextMeshProUGUI depthTextField;
    [SerializeField] private TextMeshProUGUI maxDepthTextField;
    
    
    void Awake()
    {
        Hub.Register(this);

        if (zeroDepthReference)
        {
            zeroDepthDepth = zeroDepthReference.position.y;
        }

        _uiUpdateTimer = uiUpdateInterval;
    }

    // Update is called once per frame
    void Update()
    {
        var swarm = Hub.Get<Swarm>();
        if (!swarm)
        {
            return;
        }

        swarmDepthUnits = GetDepthOfElement(swarm.transform);
        maxSwarmDepthUnits = Mathf.Max(maxSwarmDepthUnits, swarmDepthUnits);
        swarmDepthMeters = UnitDepthToMeterDepth(swarmDepthUnits);
        maxSwarmDepthMeters = UnitDepthToMeterDepth(maxSwarmDepthUnits);

        cameraDepthUnits = GetDepthOfElement(Camera.main.transform);

        // UI Updates (depoll!
        _uiUpdateTimer -= Time.deltaTime;

        if (_uiUpdateTimer > 0)
        {
            return;
        }

        _uiUpdateTimer = uiUpdateInterval;

        if (depthTextField)
        {
            depthTextField.text = $"{swarmDepthMeters:0.0}" + "m";
        }

        if (maxDepthTextField)
        {
            maxDepthTextField.text = $"max: {maxSwarmDepthMeters:0.0}" + "m";
        }
    }

    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public float GetDepthOfElement(Transform t)
    {
        var d = t.position.y - zeroDepthDepth;
        return invertDepth ? -d : d;
    }

    public float UnitDepthToMeterDepth(float unitDepth)
    {
        return unitDepth * unitToMeterRatio;
    }

    public float GetDepthOfElement(Vector2 t)
    {
        return t.y - zeroDepthDepth;
    }

    public float GetMaxSwarmDepthUnits() => maxSwarmDepthUnits;
    public float GetMaxSwarmDepthMeters() => maxSwarmDepthMeters;
    public float GetSwarmDepthMeters() => swarmDepthMeters;
    public float GetSwarmDepthUnits() => swarmDepthUnits;
    public float GetCameraDepthUnits() => swarmDepthUnits;
}