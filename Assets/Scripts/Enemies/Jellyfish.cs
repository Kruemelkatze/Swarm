using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;

public class Jellyfish : MonoBehaviour
{
    [SerializeField] private bool playedSound;
    [SerializeField] private string soundName = "jellyfish";
    [SerializeField] private bool randomSound = true;

    [SerializeField] private Light2D light;

    [SerializeField] private Color[] availableColors;

    private void Start()
    {
        if (light && availableColors.Length > 0)
        {
            light.color = availableColors.RandomElement();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playedSound && other.gameObject.layer == LayerMask.NameToLayer("Fish"))
        {
            playedSound = true;
            if (randomSound)
            {
                AudioController.Instance.PlayRandomSound(soundName);
            }
            else
            {
                AudioController.Instance.PlaySound(soundName);
            }
        }
    }
}
