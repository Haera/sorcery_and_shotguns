using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Shoot,
    Melee,
    Boom,
    Flee
}

public enum ProjectileType {
    Default,
    Cannon,
    Bullet
}