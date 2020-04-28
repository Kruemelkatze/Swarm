using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvercomeDynamicBatching : MonoBehaviour
{
    private Material mat;
    private static readonly int Origin = Shader.PropertyToID("Origin");
    private static readonly int UseFixedOrigin = Shader.PropertyToID("UseFixedOrigin");

    private void Awake()
    {
        var renderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
        if (renderer)
        {
            mat = renderer.material;
            mat.SetFloat(UseFixedOrigin, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mat)
        {
            mat.SetVector(Origin, transform.position);
        }
    }
}
