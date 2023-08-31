using System;
using System.Collections.Generic;
using Unity.NetCode;
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

        private NetworkTick m_LastAttack;
        private NetworkTick m_LastThrow;

        private Vector3 m_PreviousVelocity;
        private Vector3 m_SmoothUpTilt;

        public void HandleFixedUpdate(Vector3 position, Vector3 movementVelocity, Vector3 lookDirection, bool grounded, NetworkTick lastAttacked, NetworkTick lastThrown, float deltaTime)
        {
            var moving = movementVelocity.sqrMagnitude > 0.5f;
            
            var velocityDelta = movementVelocity - m_PreviousVelocity;
            var acceleration = velocityDelta / deltaTime;
            
            var upTilt = Vector3.ProjectOnPlane(acceleration * 0.1f, Vector3.up);
            m_SmoothUpTilt = Vector3.Lerp(m_SmoothUpTilt, upTilt, deltaTime *0.5f);
            //print("m_SmoothUpTilt: " + m_SmoothUpTilt);
            var right = Vector3.Cross(lookDirection, Vector3.up);
            var up = (Vector3.up + m_SmoothUpTilt).normalized;
            var forward = Vector3.Cross(up, right);
            
            
            if (!Dead)
            {
                m_CharacterParent.transform.rotation = Quaternion.LookRotation(forward, up);
                transform.position = position;
            }
            
            
            
            m_PreviousVelocity = movementVelocity;
            
            HandleAnimations(lastAttacked, lastThrown, moving);
        }

        private void HandleAnimations(NetworkTick lastAttacked, NetworkTick lastThrown, bool moving)
        {
            try
            {
                // set animator params
                m_Animator.SetBool(AnimatorParamMoving, moving);

                if (lastAttacked != m_LastAttack)
                {
                    m_LastAttack = lastAttacked;
                    m_Animator.SetTrigger(AnimatorParamAttack);
                }

                if (lastThrown != m_LastThrow)
                {
                    m_LastThrow = lastThrown;
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
            m_PreviousVelocity = Vector3.zero;
            m_SmoothUpTilt = Vector3.zero;
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