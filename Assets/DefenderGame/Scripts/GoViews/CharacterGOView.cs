using System;
using System.Collections.Generic;
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
        
        [SerializeField] [HideInInspector] private List<Rigidbody> m_Limbs = new List<Rigidbody>();
        

        public void HandleFixedUpdate(Vector3 position, Vector3 movementVelocity, Vector3 lookDirection, bool grounded, bool attacked)
        {
            var moving = movementVelocity.sqrMagnitude > 0.5f;
            
            m_CharacterParent.transform.rotation = Quaternion.LookRotation(lookDirection - lookDirection.y * Vector3.up);
            transform.position = position;

            try
            {
                // set animator params
                m_Animator.SetBool(AnimatorParamMoving, moving);
                if (attacked)
                {
                    m_Animator.SetTrigger(AnimatorParamAttack);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("error setting animator params: " + e);
            }
            
            
        }
        

        public void Restore()
        {
            m_Animator.SetBool(AnimatorParamMoving, false);
            m_Animator.SetBool(AnimatorParamJump, true);
        }

        private void OnValidate()
        {
            m_Limbs = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
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