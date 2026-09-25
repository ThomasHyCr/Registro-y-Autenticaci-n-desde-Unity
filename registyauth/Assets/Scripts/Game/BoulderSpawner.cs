using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour
{
    [Header("Prefab y puntos de spawn (arrastra los 3 marcadores arriba de cada carril)")]
    public GameObject boulderPrefab;
    public Transform[] laneSpawnPoints = new Transform[3];

    [Header("Ritmo de aparición")]
    public float minSpawnInterval = 0.8f;
    public float maxSpawnInterval = 1.6f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        int laneCount = laneSpawnPoints.Length;
        if (laneCount < 2 || boulderPrefab == null) return;

        // Elegimos cuántos carriles se llenan esta oleada: entre 1 y (laneCount - 1)
        // así siempre queda al menos 1 carril libre para esquivar.
        int lanesToFill = Random.Range(1, laneCount); // Random.Range(int,int) es exclusivo en el máximo

        // Barajamos los índices de carril y tomamos los primeros "lanesToFill"
        List<int> indices = new List<int>();
        for (int i = 0; i < laneCount; i++) indices.Add(i);
        Shuffle(indices);

        for (int i = 0; i < lanesToFill; i++)
        {
            Transform spawnPoint = laneSpawnPoints[indices[i]];
            if (spawnPoint != null)
            {
                Vector3 spawnPosition = spawnPoint.position;
                spawnPosition.z = -1f;
                Instantiate(boulderPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
