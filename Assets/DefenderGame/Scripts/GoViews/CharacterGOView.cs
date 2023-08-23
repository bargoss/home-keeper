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
        private static readonly int AnimatorParamThrow = Animator.StringToHash("Throw");
        
        [SerializeField] [HideInInspector] private List<Rigidbody> m_Limbs = new List<Rigidbody>();
        
        public bool Dead { get; private set; }

        public void HandleFixedUpdate(Vector3 position, Vector3 movementVelocity, Vector3 lookDirection, bool grounded, bool attacked, bool itemThrown)
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

                if (itemThrown)
                {
                    m_Animator.SetTrigger(AnimatorParamThrow);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("error setting animator params: " + e);
            }
        }
        

        public void Restore()
        {
            SetDead(false);
            m_Animator.SetBool(AnimatorParamMoving, false);
            m_Animator.SetBool(AnimatorParamJump, true);
            
            m_Animator.enabled = true;
            foreach (var limb in m_Limbs)
            {
                limb.isKinematic = true;
            }
        }

        private void Awake()
        {
            m_Limbs = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        }

        /// <summary>
        /// goes ragdoll
        /// </summary>
        public void SetDead(bool dead)
        {
            if(Dead == dead) return;
            Dead = dead;
            m_Animator.enabled = !dead;
            foreach (var limb in m_Limbs)
            {
                limb.isKinematic = !dead;
            }
        }
        
        public void ApplyExplosionForceToRagdoll(Vector3 explosionPosition, float explosionForce, float explosionRadius)
        {
            foreach (var limb in m_Limbs)
            {
                limb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }
}