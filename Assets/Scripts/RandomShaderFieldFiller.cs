using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShaderFieldFiller : MonoBehaviour
{
    // Start is called before the first frame update
    private static readonly int RandomShaderField = Shader.PropertyToID("Vector1_1F7C514A");

    void Start()
    {
        var rend = GetComponent<Renderer>();
        rend = rend != null ? rend : GetComponentInChildren<Renderer>();
        if (rend && rend.material)
        {
            rend.material.SetFloat(RandomShaderField, Random.value);
        }

    }
}
