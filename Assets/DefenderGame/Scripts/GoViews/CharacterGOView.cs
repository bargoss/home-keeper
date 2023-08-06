using System;
using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class CharacterGOView : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Transform m_CharacterParent;
        
        // animator params:
        private static readonly int AnimatorParamAttack = Animator.StringToHash("Attack");
        private static readonly int AnimatorParamMoving = Animator.StringToHash("Moving");
        private static readonly int AnimatorParamJump = Animator.StringToHash("Jump");
        
        
        

        public void HandleFixedUpdate(Vector3 movementVelocity, Vector3 lookDirection, bool grounded, bool attacked)
        {
            var moving = movementVelocity.sqrMagnitude > 0.5f;
            
            // set animator params
            m_Animator.SetBool(AnimatorParamMoving, moving);
            m_Animator.SetBool(AnimatorParamJump, !grounded);
            if (attacked)
            {
                m_Animator.SetTrigger(AnimatorParamAttack);
            }
            
            m_CharacterParent.transform.rotation = Quaternion.LookRotation(lookDirection - lookDirection.y * Vector3.up);
        }
        

        public void Restore()
        {
            m_Animator.SetBool(AnimatorParamMoving, false);
            m_Animator.SetBool(AnimatorParamJump, true);
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