using System;
using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.HomeKeeper
{
    public class ItemManipulationTest : MonoBehaviour
    {
        private Entity m_PlayerActionEntity;

        
        private EntityManager EM => World.DefaultGameObjectInjectionWorld.EntityManager;
        
        private void Start()
        {
            m_PlayerActionEntity = EM.CreateEntity();
            EM.AddComponentData(m_PlayerActionEntity, new PlayerAction());
        }

        private void Update()
        {
            var camera = Camera.main;

            var playerAction = EM.GetComponentData<PlayerAction>(m_PlayerActionEntity);
            playerAction.Grab = false;
            playerAction.Drop = false;
            playerAction.CameraPosition = camera.transform.position;
            playerAction.CameraForward = camera.transform.forward;
            playerAction.MouseDirection = camera.ScreenPointToRay(Input.mousePosition).direction;
            playerAction.GrabDistance = 10f;
            
            if (Input.GetMouseButtonDown(0))
            {
                playerAction.Grab = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                playerAction.Drop = true;
            }
            
            EM.SetComponentData(m_PlayerActionEntity, playerAction);
        }
    }
}