using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    [SerializeField] private bool playedSound;
    [SerializeField] private string soundName = "jellyfish";
    [SerializeField] private bool randomSound = true;
    

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
