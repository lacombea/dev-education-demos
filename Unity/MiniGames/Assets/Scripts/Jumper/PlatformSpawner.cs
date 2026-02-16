using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public int initialPlatforms = 10;
    public float levelWidth = 6f;
    public float minY = 0.8f;
    public float maxY = 2.2f;

    public Transform player;

    List<GameObject> pool = new List<GameObject>();
    float highestPlatformY = -Mathf.Infinity;

    void Start()
    {
        // initial spawn
        float spawnY = -1f;
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnAtRandomX(spawnY);
            spawnY += Random.Range(minY, maxY);
        }
    }

    void Update()
    {
        // spawn ahead while player climbs
        while (highestPlatformY < player.position.y + 10f)
        {
            SpawnAtRandomX(highestPlatformY + Random.Range(minY, maxY));
        }

        // optional: recycle platforms that fall far below camera
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].transform.position.y < Camera.main.transform.position.y - 12f)
            {
                pool[i].transform.position = new Vector3(1000, 1000, 0); // hide / or reuse immediately in spawn
            }
        }
    }

    void SpawnAtRandomX(float y)
    {
        float x = Random.Range(-levelWidth, levelWidth);
        GameObject p = Instantiate(platformPrefab, new Vector3(x, y, 0), Quaternion.identity);
        pool.Add(p);
        if (y > highestPlatformY) highestPlatformY = y;
    }
}