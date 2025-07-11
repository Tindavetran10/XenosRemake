﻿using UnityEngine;

namespace Scripts
{
    public class Enemy : Entity
    {
        public EnemyIdleState IdleState;
        public EnemyMoveState MoveState;

        [Header("Movement Details")] 
        public float idleTime;
        public float moveSpeed;
        [Range(0,2)] public float moveAnimSpeedMultiplier;
        
    }
}