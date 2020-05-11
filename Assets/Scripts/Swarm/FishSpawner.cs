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
    [SerializeField] private Transform fishesLookAt;

    [SerializeField] private Color[] fishColors;

    [Header("Split Settings")] [SerializeField]
    private Transform splitLeftLocation;

    [SerializeField] private Transform splitLeftLookAt;
    [SerializeField] private Transform splitRightLocation;
    [SerializeField] private Transform splitRightLookAt;

    [Header("Growth")] [SerializeField] private bool exponentialGrowth;
    [SerializeField] [Min(0)] private float growthTick = 1;
    [SerializeField] [Min(0)] private float growthRate = 1;
    private float _localGrowthTick;

    [Header("Spawn Settings")] [SerializeField]
    private int spawnCount = 10;

    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform spawnLocationOutsideLeft;
    [SerializeField] private Transform spawnLocationOutsideRight;
    [SerializeField] private float outsideSpawnYOffset = 4;

    [SerializeField] private int maxFishSpawns;

    [SerializeField] private float spawnRadius = 2;
    [SerializeField] private float fishRadius = 0.1f;

    [Header("Control Stuff")] [SerializeField]
    private bool drawDebugInfo = true;

    [SerializeField] private List<Fish> fishes = new List<Fish>();
    [SerializeField] private LinkedList<Vector2> _spawnLocations = new LinkedList<Vector2>();

    public bool SpawnEnabled = true;

    private GameController gc;

    private void Awake()
    {
        Hub.Register<FishSpawner>(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawDebugInfo)
        {
            return;
        }

        Gizmos.DrawWireSphere(spawnLocation.position, spawnRadius);

        var i = 0;
        foreach (var loc in _spawnLocations)
        {
            var c = new Color(1, 1, 0, 1 - ((float) i / _spawnLocations.Count));
            Gizmos.color = c;
            Gizmos.DrawSphere(spawnLocation.position + (Vector3) loc, fishRadius);
            i++;
        }
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        PrepareSpawnLocations();
        _localGrowthTick = growthTick;

        gc = Hub.Get<GameController>();
    }

    private void Update()
    {
        if (!gc.IsActive())
        {
            return;
        }

        _localGrowthTick -= Time.deltaTime;
        if (_localGrowthTick < 0)
        {
            _localGrowthTick = growthTick;
            if (SpawnEnabled)
            {
                Spawn((int) growthRate, true);
            }
        }
    }

    public int NumberOfFish => fishes.Count;

    public int SpawnCount => spawnCount;

    private void PrepareSpawnLocations()
    {
        var sampler = new PoissonDiscSampler(2 * spawnRadius, 2 * spawnRadius, fishRadius);
        var half = Vector2.one * spawnRadius;
        var sqr = spawnRadius * spawnRadius;
        var locs = sampler.Samples()
            .Select(loc => loc - half)
            .Where(loc => loc.sqrMagnitude < sqr)
            .OrderBy(loc => loc.sqrMagnitude)
            .ToList();
        _spawnLocations = new LinkedList<Vector2>(locs);

        maxFishSpawns = _spawnLocations.Count;
    }

    public void Spawn(int? k = null, bool outside = false)
    {
        var num = k ?? spawnCount;

        var maxNum = Mathf.Min(num, maxFishSpawns - fishes.Count);
        var fishIndex = fishes.Count;
        for (int i = 0; i < maxNum; i++)
        {
            var index = fishIndex + i;

            // Main Fish Spawn
            Vector2 localPos;
            if (_spawnLocations.Count > 0)
            {
                localPos = _spawnLocations.First();
                _spawnLocations.RemoveFirst();
            }
            else
            {
                localPos = Vector2.zero;
            }

            var isLeftFish = localPos.x < 0;

            var fish = Instantiate(fishPrefab, fishContainer);
            var fishScript = fish.GetComponent<Fish>();

            Vector3 effectiveLocation;
            if (outside)
            {
                effectiveLocation = isLeftFish ? spawnLocationOutsideLeft.position : spawnLocationOutsideRight.position;
                effectiveLocation += Vector3.up * Random.Range(-outsideSpawnYOffset, outsideSpawnYOffset);
            }
            else
            {
                effectiveLocation = spawnLocation.position;
            }

            fish.transform.position = effectiveLocation + (Vector3) localPos;
            // var diff = transform.position - fish.transform.position;
            // diff.Normalize();
            // float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            // fish.transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);

            var color = fishColors[Random.Range(0, fishColors.Length)];
            fishScript.index = index;
            fishScript.SetColor(color);


            fishScript.SetTarget(
                spawnLocation,
                fishesLookAt,
                isLeftFish ? splitLeftLocation : splitRightLocation,
                isLeftFish ? splitLeftLookAt : splitRightLookAt,
                localPos
            );

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

    public void RemoveFish(Fish fish)
    {
        fishes.Remove(fish);
        //var pos = _spawnLocations[fish.index];
        //_spawnLocations.RemoveAt(fish.index);
        //_spawnLocations.Add(pos);

        fish.Eaten();
        ReinsertFishSpawnPosition(fish.GetRelativeTargetPosition());
    }

    private void ReinsertFishSpawnPosition(Vector2 fishPos)
    {
        var sqrMagn = fishPos.sqrMagnitude;
        LinkedListNode<Vector2> insertBeforeElement = null;
        var currentNode = _spawnLocations.First;
        while (currentNode != null)
        {
            if (currentNode.Value.sqrMagnitude > sqrMagn)
            {
                insertBeforeElement = currentNode;
                break;
            }

            currentNode = currentNode.Next;
        }


        if (insertBeforeElement != null)
        {
            _spawnLocations.AddBefore(insertBeforeElement, fishPos);
        }
        else
        {
            _spawnLocations.AddLast(fishPos);
        }
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