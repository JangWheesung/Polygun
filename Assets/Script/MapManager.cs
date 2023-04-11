using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static UnityEditor.PlayerSettings;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemys;

    //[Header("Range")]
    [SerializeField] float minX, maxX, minZ, maxZ;

    RaycastHit hit;

    void Awake()
    {
        StartCoroutine(SpawnSpin(3));
    }

    void Update()
    {
        //StartCoroutine(SpawnSpin(3));
        //if(Time.time % 5 == 0)
        //    SpawnEnemy(enemys[Random.Range(0, enemys.Count)]);
    }

    void SpawnEnemy(GameObject spawnEnemy)
    {
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(minX, maxX), 3, UnityEngine.Random.Range(minZ, maxZ));

        Collider[] col = Physics.OverlapSphere(randomPos, 1f);

        bool floor = Physics.Raycast(randomPos + new Vector3(0, 2, 0), Vector3.up * 30, out hit);

        if (col.Length < 2 && (hit.transform == null || hit.transform.tag != "Building"))
        {
            Instantiate(spawnEnemy, randomPos, Quaternion.identity);
        }
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
                //continue;
            }

            yield return new WaitForSeconds(time);
        }
    }
}
