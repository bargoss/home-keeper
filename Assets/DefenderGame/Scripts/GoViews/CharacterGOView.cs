using System;
using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class CharacterGOView : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Transform m_CharacterParent;
        
        private static readonly int StateIdle = Animator.StringToHash("Idle");
        private static readonly int StateRun = Animator.StringToHash("Run");
        private static readonly int StateAttack = Animator.StringToHash("Attack");

        public bool TestMode = false;
        public bool TestAttacked = false;
        public Vector3 TestMovementVelocity = Vector3.zero;
        public Vector3 TestLookDirection = Vector3.forward;
        
        public void FixedUpdate()
        {
            HandleFixedUpdate(TestMovementVelocity, TestLookDirection, true, TestAttacked);
            
            TestAttacked = false;
        }


        public void HandleFixedUpdate(Vector3 movementVelocity, Vector3 lookDirection, bool grounded, bool attacked)
        {
            var animatorState = m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            
            if (attacked)
            {
                m_Animator.Play(StateAttack);
            }
            else if (movementVelocity.sqrMagnitude > 0.5f && animatorState == StateIdle)
            {
                m_Animator.Play(StateRun);
            }
            else if (movementVelocity.sqrMagnitude < 0.5f && animatorState == StateRun)
            {
                m_Animator.Play(StateIdle);
            }
            
            m_CharacterParent.transform.rotation = Quaternion.LookRotation(lookDirection - lookDirection.y * Vector3.up);
        }
        

        public void Restore()
        {
            m_Animator.Play(StateIdle);
        }

        public void ActivateRagdoll()
        {
            throw new System.NotImplementedException();
        }
        
        public void ApplyExplosionForceToRagdoll(Vector3 explosionPosition, float explosionForce)
        {
            throw new System.NotImplementedException();
        }
    }
}