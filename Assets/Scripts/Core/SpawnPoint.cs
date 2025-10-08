using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    

    private void OnEnable()
    {
        spawnPoints.Add(this);
    }
    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }
    

    public static Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points available!");
            return Vector3.zero;
        }
        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index].transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
