using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] blocs;
    public Vector3 position;

    public float spawnRate;
    private float timer = 0;

    private void Awake()
    {
        spawnRate = 4.25f;
    }
    void Spawn()
    {
        Instantiate(blocs[Random.Range(0, blocs.Length)],
        position,
        Quaternion.identity);
    }
    public void Clear()
    {
        var instances = GameObject.FindGameObjectsWithTag("Bloc");
        foreach (var bloc in instances)
        {
            Destroy(bloc);
        }
    }
    public void IncreaseSpawnRate()
    {
        spawnRate -= 0.25f;
    }

    void FixedUpdate()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Spawn();
            timer = 0;
        }
    }
}
