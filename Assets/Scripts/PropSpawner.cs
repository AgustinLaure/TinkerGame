using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private UnityPool[] possibleDropPools;

    [Header("Config")]
    [SerializeField] private GameObject[] possibleDrops;
    [SerializeField] private bool isRandomDrop;
    [SerializeField] private bool isRandomSpawnPoint;

    private Dictionary<GameObject, UnityPool> prefabToPool = new Dictionary<GameObject, UnityPool>();

    int dropIter = 0;
    int spawnPointIter = 0;

    protected virtual void Awake()
    {
        for (int i = 0; i < possibleDrops.Length; i++)
        {
            prefabToPool.Add(possibleDrops[i], possibleDropPools[i]);
        }
    }

    protected void Spawn(Quaternion rotation)
    {
        if (spawnPoints.Length > 0 && possibleDropPools.Length > 0 && possibleDropPools.Length == possibleDrops.Length && possibleDrops.Length > 0)
        {
            if (isRandomDrop)
            {
                SpawnProp(rotation, Random.Range(0, possibleDrops.Length), isRandomSpawnPoint);
            }
            else
            {
                dropIter++;

                if (dropIter >= possibleDrops.Length)
                {
                    dropIter = 0;
                }

                SpawnProp(rotation, dropIter, isRandomSpawnPoint);
            }
        }
    }

    private void SpawnProp(Quaternion rotation, int drop, bool isRandomPoint)
    {
        if (prefabToPool.TryGetValue(possibleDrops[drop], out UnityPool pool))
        {
            Vector3 spawnPoint;

            if (isRandomPoint)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            }
            else
            {
                spawnPointIter++;

                if (spawnPointIter >= spawnPoints.Length)
                {
                    spawnPointIter = 0;
                }

                spawnPoint = spawnPoints[spawnPointIter].position;
            }

            pool.GetItem(spawnPoint, rotation, transform);
        }
    }
}
