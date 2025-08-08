using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private float _deathTimer;
    private bool _deathEffectsPlayed;

    public EnemyDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        StateMachine.SwitchOffStateMachine();

        // Stop all movement
        Enemy.SetVelocityY(0, 0);

        // Play death effects
        PlayDeathEffects();

        // Set a timer for cleanup
        _deathTimer = Enemy.deathDuration; // Add this field to Enemy
    }

    public override void Update()
    {
        base.Update();

        _deathTimer -= Time.deltaTime;

        if (_deathTimer <= 0)
        {
            HandleDeathCleanup();
        }
    }

    private void PlayDeathEffects()
    {
        if (_deathEffectsPlayed) return;

        // Play death VFX, sound, spawn loot, etc.
        // Enemy.PlayDeathVFX();
        // Enemy.SpawnLoot();
        // GameManager.Instance.AddScore(Enemy.scoreValue);

        _deathEffectsPlayed = true;
    }

    private void HandleDeathCleanup()
    {
        // Destroy the enemy or move to the object pool
        Object.Destroy(Enemy.gameObject);
    }
}
