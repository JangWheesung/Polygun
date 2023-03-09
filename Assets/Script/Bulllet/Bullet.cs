using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Dev;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    private void OnEnable()
    {
        FAED.InvokeDelay(() => { FAED.Push(gameObject); }, 2);
    }

    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player"))
        {
            Quaternion thisRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            BulletSmokeManager.Instance.CreateBulletSmoke(transform.position, thisRot);
        }

        FAED.Push(gameObject);
    }
}
