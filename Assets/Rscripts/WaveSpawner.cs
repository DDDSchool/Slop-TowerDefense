using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float timeBetweenEnemies = 0.5f;
    public float timeBetweenWaves = 5f;
    private int wave = 0;

    void Start() 
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves() 
    {
        while (true) 
        {
            wave += Random.Range(1, 4); //wave add
            for (int i = 0; i < wave; i++)
            {
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(timeBetweenEnemies);
            }
            
            while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                yield return null;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}