using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class CharacterMovementAuthoring : MonoBehaviour
    {
        [GhostField] public float MaxSpeed;
        [GhostField] public float MaxAcceleration;

        public class CharacterMovementBaker : Baker<CharacterMovementAuthoring>
        {
            public override void Bake(CharacterMovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new CharacterMovement
                    {
                        MaxSpeed = authoring.MaxSpeed, MaxAcceleration = authoring.MaxAcceleration
                    });
            }
        }
    }
}