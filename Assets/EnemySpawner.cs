using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public TextMeshProUGUI wavesText;

    public GameObject Enemy;
    public int[] enemiesPerWave = { 2, 4, 5 };
    public float startDelay = 1f;
    public float spawnInterval = 1.3f;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    //IEnumerator SpawnWaves()
    //{
    //    yield return new WaitForSeconds(startDelay);

    //    for (int wave = 0; wave < enemiesPerWave.Length; wave++)
    //    {
    //        wavesText.text = $"Wave {wave}";

    //        activeEnemies.Clear();

    //        for (int i = 0; i < enemiesPerWave[wave]; i++)
    //        {
    //            GameObject enemy = Instantiate(Enemy, transform.position, Quaternion.identity);
    //            //enemy.GetComponent<EnemyMovement>().speed = 13;
    //            activeEnemies.Add(enemy);
    //            yield return new WaitForSeconds(spawnInterval);
    //        }

    //        // Wait until all enemies are destroyed
    //        yield return new WaitUntil(() => AllEnemiesDefeated());

    //        wavesText.text = $"Wave {wave}";
    //    }

    //    Debug.Log("All waves complete!");
    //}
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startDelay);

        int wave = 1;

        while (true)
        {
            wavesText.text = $"Wave {wave}";

            activeEnemies.Clear();

            int enemiesToSpawn = Mathf.RoundToInt(2 + Mathf.Log(wave + 1) * wave); // Example scaling formula

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                GameObject enemy = Instantiate(Enemy, transform.position, Quaternion.identity);
                activeEnemies.Add(enemy);
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitUntil(() => AllEnemiesDefeated());

            wave++;
        }
    }


    bool AllEnemiesDefeated()
    {
        // Clean up null entries (destroyed enemies)
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count == 0;
    }
}
