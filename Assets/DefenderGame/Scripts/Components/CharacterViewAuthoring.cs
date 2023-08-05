using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class CharacterViewAuthoring : MonoBehaviour
    {
        public class CharacterViewBaker : Baker<CharacterViewAuthoring>
        {
            public override void Bake(CharacterViewAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CharacterView());
            }
        }
    }
}