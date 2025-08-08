using UnityEngine;

namespace Scripts.AnimationTrigger
{
    public class EnemyAnimationTrigger : EntityAnimationTrigger
    {
        private Enemy _enemy;
        private EnemyVFX _enemyVFX;

        public override void Awake()
        {
            base.Awake();
            _enemy = GetComponentInParent<Enemy>();
            _enemyVFX = GetComponentInParent<EnemyVFX>();
        }
        
        private void EnableCounterWindow()
        {
            _enemy.EnableCounterWindow(true);
            _enemyVFX.EnableAttackAlert(true);
        }

        private void DisableCounterWindow()
        {
            _enemy.EnableCounterWindow(false);  
            _enemyVFX.EnableAttackAlert(false);
        }
    }
}