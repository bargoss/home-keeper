using Unity.Entities;
using Unity.Mathematics;

namespace _OnlyOneGame.Scripts.Components.Data
{
    public struct OnGoingActionData{
        public float Float0;
        public float Float1;
        public float Float2;
        public int Int0;
        public int Int1;
        
        public byte TypeIndex;
        
        public static implicit operator OnGoingActionData(ActionMeleeAttacking actionMeleeAttacking) {
            return new OnGoingActionData {
                Float0 = actionMeleeAttacking.Direction.x,
                Float1 = actionMeleeAttacking.Direction.y,
                Float2 = actionMeleeAttacking.Direction.z,
                TypeIndex = 0
            };
        }
        
        public static implicit operator OnGoingActionData(ActionDismantling actionDismantling) {
            return new OnGoingActionData {
                Int0 = actionDismantling.Target.Index,
                Int1 = actionDismantling.Target.Version,
                TypeIndex = 1
            };
        }
        
        public bool TryGet(out ActionMeleeAttacking meleeAttacking) {
            if (TypeIndex == 0) {
                meleeAttacking = new ActionMeleeAttacking(new float3(Float0, Float1, Float2));
                return true;
            }
            meleeAttacking = default;
            return false;
        }
        
        public bool TryGet(out ActionDismantling dismantling) {
            if (TypeIndex == 1) {
                dismantling = new ActionDismantling {
                    Target = new Entity {
                        Index = Int0,
                        Version = Int1
                    }
                };
                return true;
            }
            dismantling = default;
            return false;
        }
    }
    
    public struct ActionMeleeAttacking
    {
        public float3 Direction;
        
        public ActionMeleeAttacking(float3 direction)
        {
            Direction = direction;
        }
    }

    public struct ActionDismantling
    {
        public Entity Target;

        public ActionDismantling(Entity target)
        {
            Target = target;
        }
    }
}