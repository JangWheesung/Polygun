using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Dev;

public class BulletSmokeManager : MonoBehaviour
{
    public static BulletSmokeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateBulletSmoke(Vector3 bulletPos, Quaternion bulletRot)
    {
        FAED.Pop("BulletSmoke", bulletPos, bulletRot);
    }

    public void CreateEnemyHitParticle(Vector3 bulletPos, Quaternion bulletRot)
    {
        FAED.Pop("EnemyHitParticle", bulletPos, bulletRot);
    }
}
