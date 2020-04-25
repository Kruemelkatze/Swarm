using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish goes *happy fish noises*")] [SerializeField]
    private GameObject fishPrefab;

    [SerializeField] private Transform fishContainer;
    [SerializeField] private Color[] fishColors;

    [Header("Spawn Settings")] [SerializeField]
    private int spawnCount = 10;

    [SerializeField] private int maxFishSpawns;

    [SerializeField] private float spawnRadius = 2;
    [SerializeField] private float fishRadius = 0.1f;

    [Header("Control Stuff")] [SerializeField]
    private bool drawDebugInfo = true;

    [SerializeField] private List<Fish> fishes = new List<Fish>();
    [SerializeField] private List<Vector2> _spawnLocations = new List<Vector2>() {Vector2.up};


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawDebugInfo)
        {
            return;
        }


        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        for (int i = 0; i < _spawnLocations.Count; i++)
        {
            var loc = _spawnLocations[i];
            var c = new Color(1, 1, 0, 1 - ((float) i / _spawnLocations.Count));
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position + (Vector3) loc, fishRadius);
        }
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        PrepareSpawnLocations();
    }

    private void PrepareSpawnLocations()
    {
        var sampler = new PoissonDiscSampler(2 * spawnRadius, 2 * spawnRadius, fishRadius);
        var half = Vector2.one * spawnRadius;
        var sqr = spawnRadius * spawnRadius;
        _spawnLocations = sampler.Samples()
            .Select(loc => loc - half)
            .Where(loc => loc.sqrMagnitude < sqr)
            .OrderBy(loc => loc.sqrMagnitude)
            .ToList();

        maxFishSpawns = _spawnLocations.Count;
    }

    private void Spawn(int? k = null)
    {
        var num = k ?? spawnCount;

        var maxNum = Mathf.Min(num, maxFishSpawns - fishes.Count);
        var fishIndex = fishes.Count;
        for (int i = 0; i < maxNum; i++)
        {
            var index = fishIndex + i;
            // Main Fish Spawn
            var fish = Instantiate(fishPrefab, fishContainer);
            var fishScript = fish.GetComponent<Fish>();

            fish.transform.position = transform.position + (Vector3) _spawnLocations[index];
            var color = fishColors[Random.Range(0, fishColors.Length)];
            fishScript.SetColor(color);
            fishScript.SetTargetPosition(fish.transform.position);
            
            fishes.Add(fishScript);
        }
    }

    private void SpawnAll()
    {
        foreach (var f in fishes)
        {
            f.SimplyRemove();
        }

        fishes.Clear();

        Spawn(maxFishSpawns);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FishSpawner))]
    public class FishSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var spawner = target as FishSpawner;
            if (spawner == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Prepare Locations"))
            {
                spawner.PrepareSpawnLocations();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Spawn 1"))
            {
                spawner.Spawn(1);
            }
            if (GUILayout.Button("Spawn K"))
            {
                spawner.Spawn();
            }

            if (GUILayout.Button("Spawn All"))
            {
                spawner.SpawnAll();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}