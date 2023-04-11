using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Vector3 outBoxPos;
    [SerializeField] private Vector3 outBoxSize;

    RaycastHit hit;

    private void Awake()
    {
        
    }

    void Update()
    {
        RaycastHit[] hits = Physics.BoxCastAll(outBoxPos, outBoxSize, Vector3.zero, Quaternion.identity, 0);

        Debug.Log(hits.Length);

        foreach (RaycastHit hit in hits)
        {
            Destroy(hit.collider.gameObject);
            if (hit.collider.tag == "Enemy")
            {
                Debug.Log("ÄÆ");
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(outBoxPos, outBoxSize);
    }
}
