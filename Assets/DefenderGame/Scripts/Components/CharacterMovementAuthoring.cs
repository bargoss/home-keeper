using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class CharacterMovementAuthoring : MonoBehaviour
    {
        public float MaxSpeed;
        public float MaxAcceleration;

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