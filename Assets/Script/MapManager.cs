using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemys;

    RaycastHit hit;

    void Awake()
    {
        StartCoroutine(SpawnSpin(3));
    }

    void SpawnEnemy(GameObject spawnEnemy)
    {
        Instantiate(spawnEnemy, RandomPos(30), Quaternion.identity);
    }

    private Vector3 RandomPos(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
        return hit.position;
    }

    IEnumerator SpawnSpin(float time)
    {
        while (true)
        {
            try
            {
                SpawnEnemy(enemys[UnityEngine.Random.Range(0, enemys.Count)]);
            }
            catch (Exception exp)
            {
                
            }

            yield return new WaitForSeconds(time);
        }
    }
}
