using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemys;

    //[Header("Range")]
    [SerializeField] float minX, maxX, minZ, maxZ;

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
        Vector3 randomPos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        Collider[] col = Physics.OverlapSphere(randomPos, 1f);

        if (col.Length < 2) Instantiate(spawnEnemy, randomPos, Quaternion.identity);
    }

    IEnumerator SpawnSpin(float time)
    {
        while (true)
        {
            Debug.Log(69);
            SpawnEnemy(enemys[Random.Range(0, enemys.Count)]);
            yield return new WaitForSeconds(time);
        }
    }
}
