using UnityEngine;

namespace DefaultNamespace
{
    public abstract class EntityState
    {
        protected StateMachine StateMachine;    // Reference to the state machine
        protected string AnimBoolName;          // Animation boolean parameter name
    
        protected Animator Anim;    // Cached animator reference
        protected Rigidbody2D Rb;

        protected float StateTimer;
        protected bool TriggerCalled;

        protected EntityState(StateMachine stateMachine, string animBoolName)
        {
            StateMachine = stateMachine;
            AnimBoolName = animBoolName;
        }
        
        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void Enter()
        {
            TriggerCalled = false;
            
            // Activate this state's animation
            Anim.SetBool(AnimBoolName, true);
        }

        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        public virtual void Update()
        {
            StateTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Called when exiting this state
        /// </summary>
        public virtual void Exit() => 
            Anim.SetBool(AnimBoolName, false); // Deactivate this state's animation
        
        public void CallAnimationTrigger() => TriggerCalled = true;
    }
}