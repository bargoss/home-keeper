using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeItemGridAuthoring : MonoBehaviour
    {
        public int width = 5;
        public int height = 5;
        public float gridLength = 2;
        
        public class DeItemGridBaker : Baker<DeItemGridAuthoring>
        {
            public override void Bake(DeItemGridAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                //AddComponentObject(entity, new DeItemGrid(authoring.width, authoring.height, authoring.gridLength));
                AddComponentObject(entity, new DeItemGrid());
            }
        }
    }
}