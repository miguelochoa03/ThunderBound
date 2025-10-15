using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemy;
    //GameObject bloodEffectCopy = Instantiate(bloodEffect, transform.position, Quaternion.identity);
    float cooldown = 0f;
    float nextSpawn = 2f;


    // Update is called once per frame
    void Update()
    {
        //Instantiate(Enemy, transform.position, Quaternion.identity);
        if (Time.time > cooldown)
        {
            Instantiate(Enemy, transform.position, Quaternion.identity);
            cooldown = Time.time + nextSpawn;
        }
    }
}
