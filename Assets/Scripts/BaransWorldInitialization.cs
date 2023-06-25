using System;
using System.Collections.Generic;
using Unity.Entities;
using System.Linq;



    /// <summary>
    /// Utilities to help initialize the default ECS <see cref="World"/>.
    /// </summary>
    public static class BaransWorldInitialization
    { 
        private static void AddSystemToRootLevelSystemGroupsBaran(World world, IEnumerable<Type> allSystems)
        {
            foreach (var systemType in allSystems)
            {
                var updateInGroupAttributes = TypeManager.GetSystemAttributes(systemType, typeof(UpdateInGroupAttribute));
                if (updateInGroupAttributes.Length == 0)
                    continue;
                
                var updateInGroupAttribute = (UpdateInGroupAttribute)updateInGroupAttributes[0];
                var system = world.CreateSystem(systemType);
                var systemGroup = world.GetOrCreateSystemManaged(updateInGroupAttribute.GroupType) as ComponentSystemGroup;
                systemGroup.AddSystemToUpdateList(system);
            }
        } 
        public static World Initialize(string defaultWorldName, bool editorWorld = false)
        {
            var world = new World(defaultWorldName, editorWorld ? WorldFlags.Editor : WorldFlags.Game);
            
            var initializationSystemGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationSystemGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationSystemGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            
            var allSystems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
            AddSystemToRootLevelSystemGroupsBaran(world, allSystems);
            
            initializationSystemGroup.SortSystems();
            simulationSystemGroup.SortSystems();
            presentationSystemGroup.SortSystems();
            
            return world;
        }
    }

