using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRaycast : MonoBehaviour
{
    public static GunRaycast Instance;

    private GameObject player;
    public PlayerHead playerHead;

    Ray ray;
    RaycastHit hit;

    private void Awake()
    {
        Instance = this;

        player = transform.parent.parent.parent.gameObject;
        //playerHead = FindObjectOfType<PlayerHead>();
    }

    void Update()
    {
        ray = new Ray(transform.position, transform.forward);
    }

    public void GunRay()
    {
        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Enemy"))//Àû
        {
            hit.transform.GetComponent<EnemyAi>().Behitting();

            Quaternion thisRot = Quaternion.Euler(playerHead.transform.eulerAngles.x - 180, player.transform.eulerAngles.y, 0);
            BulletSmokeManager.Instance.CreateEnemyHitParticle(hit.point, thisRot);
        }
        else if (Physics.Raycast(ray, out hit))//±× ¿Ü
        {
            Quaternion thisRot = Quaternion.Euler(playerHead.transform.eulerAngles.x - 90, player.transform.eulerAngles.y, 180);
            BulletSmokeManager.Instance.CreateBulletSmoke(hit.point, thisRot);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 20);
    }
}
