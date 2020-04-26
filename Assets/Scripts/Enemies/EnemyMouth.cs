using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMouth : MonoBehaviour
{
    [SerializeField] private string biteAudioName;
    [SerializeField] private float audioRate = 0.2f;

    private float localAudioRate;

    private void Start()
    {
        localAudioRate = audioRate;
    }

    private void Update()
    {
        localAudioRate = Mathf.Max(0, localAudioRate - Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SwarmElement"))
        {
            Debug.Log("Swarm!");
        } 
        else if (other.gameObject.layer == LayerMask.NameToLayer("Fish"))
        {
            var spawner = Hub.Get<FishSpawner>();
            spawner.RemoveFish(other.GetComponent<Fish>());
            if (localAudioRate <= 0.005f)
            {
                AudioController.Instance.PlaySound(biteAudioName);
                localAudioRate = audioRate;
            }
        }
    }
}
