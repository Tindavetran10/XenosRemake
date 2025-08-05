using UnityEngine;

namespace Scripts
{
    public class Enemy : Entity
    {
        public EnemyIdleState IdleState;
        public EnemyMoveState MoveState;
        public EnemyAttackState AttackState;
        public EnemyBattleState BattleState;
        public EnemyDeathState DeathState;

        [Header("Movement Details")] 
        public float idleTime;
        public float moveSpeed;
        [Range(0,2)] public float moveAnimSpeedMultiplier;

        [Header("Battle Details")]
        public float battleMoveSpeed = 5f;
        public float attackDistance = 2f;
        public float battleTimeDuration = 5f;
        public float minRetreatDistance = 1f;
        public Vector2 retreatVelocity;
        
        [Header("Player detection")]
        [SerializeField] private LayerMask whatIsPlayer;
        [SerializeField] private Transform playerCheck;
        [SerializeField] private float playerCheckDistance = 10f;
        
        [Header("Death Details")]
        public float deathDuration = 2f;
        public Transform PlayerTransform { get; private set; }

        public override void EntityDeath()
        {
            base.EntityDeath();
            StateMachine.ChangeState(DeathState);
        }

        public void TryEnterBattleState(Transform player)
        {
            if (StateMachine.currentState == BattleState || StateMachine.currentState == AttackState) 
                return;
            
            this.PlayerTransform = player;
            StateMachine.ChangeState(BattleState);
        }
        
        public Transform GetPlayerReference()
        {
            if (PlayerTransform == null)
                PlayerTransform = PlayerDetected().transform;
            return PlayerTransform;
        }

        public RaycastHit2D PlayerDetected()
        {
            var hit = Physics2D.Raycast(playerCheck.position, Vector2.right * FacingDirection, 
                playerCheckDistance, whatIsPlayer | groundLayer);

            if (hit.collider is null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                return default;
            return hit;
        }
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + FacingDirection * 
                playerCheckDistance, playerCheck.position.y));
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + FacingDirection * 
                attackDistance, playerCheck.position.y));
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + FacingDirection * 
                minRetreatDistance, playerCheck.position.y));
        }
    }
}