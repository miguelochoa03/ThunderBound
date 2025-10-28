using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public TextMeshProUGUI wavesText;
    public GameObject Enemy;
    public int[] enemiesPerWave = { 2, 4, 5 };
    public float startDelay = 6f;
    public float spawnCooldown = 1.3f;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        // start handling the spawning in waves
        StartCoroutine(SpawnWaves());
    }
    // logic to spawn enemies and wave system
    IEnumerator SpawnWaves()
    {
        // give time before immediately spawning
        yield return new WaitForSeconds(startDelay);

        int wave = 1;

        while (true)
        {
            wavesText.text = $"Wave {wave}";

            activeEnemies.Clear();

            int enemiesToSpawn = Mathf.RoundToInt(2 + Mathf.Log(wave + 1) * wave);

            // start spawning the amount in the wave
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                GameObject enemy = Instantiate(Enemy, transform.position, Quaternion.identity);
                activeEnemies.Add(enemy);
                yield return new WaitForSeconds(spawnCooldown);
            }

            yield return new WaitUntil(() => AllEnemiesDefeated());

            wave++;
        }
    }
    // checks to know when next wave can be done when numbered enemies are gone
    bool AllEnemiesDefeated()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count == 0;
    }
}
