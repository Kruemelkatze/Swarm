using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMouth : MonoBehaviour
{
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
        }
    }
}
