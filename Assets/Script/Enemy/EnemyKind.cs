using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Enemy/EnemyStat")]
public class EnemyKind : ScriptableObject
{
    public float hp;
    public float roundPerMinute;
    public float radiusRange;
    public float boundaryTime;
}
