using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class ChildForOneFrameAuthoring : MonoBehaviour
    {
        
    }

    public class ChildForOneFrameBaker : Baker<ChildForOneFrameAuthoring>
    {
        public override void Bake(ChildForOneFrameAuthoring authoring)
        {
            var parentGo = authoring.gameObject.transform.parent.gameObject;
            var tr = authoring.gameObject.transform;
            var localTransformMatrix = Matrix4x4.TRS(tr.localPosition, tr.localRotation, tr.localScale);
            
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new ChildForOneFrame
            {
                Parent = GetEntity(parentGo, TransformUsageFlags.Dynamic), 
                LocalTransform = localTransformMatrix
            });
        }
    }
}