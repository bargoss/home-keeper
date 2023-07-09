using System;
using System.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
/*
public static class WorldCreator{
    public static void Create(string worldName,string subScenePath,CoroutineRunner coroutineRunner,float timeoutSeconds,Action<Result<World>> onCreated)
    {
        coroutineRunner.StartCoroutineFromOutside(CreateWorldCoroutine(worldName, subScenePath, timeoutSeconds, onCreated));
    }
        
    private static World CreateWorld(string name)
    {
        var world = new World(name);
            
        // add default system groups
        var initializationSystemGroup = world.GetOrCreateSystem<InitializationSystemGroup>();
        world.GetOrCreateSystem<SimulationSystemGroup>();
        world.GetOrCreateSystem<PresentationSystemGroup>();
        
        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
        
        initializationSystemGroup.Update(world.Unmanaged);
            
        return world;
    }
        
        
    // create a coroutine that creates a world
    private static IEnumerator CreateWorldCoroutine(string worldName, string subScenePath, float timeoutSeconds, Action<Result<World>> onCreated)
    {
        var world = CreateWorld(worldName);

        // loadscene
        var guid = AssetDatabase.GUIDFromAssetPath(subScenePath);
        var subSceneEntity = SceneSystem.LoadSceneAsync(world.Unmanaged, guid,
            new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.LoadAdditive });
            
        var loadingStarted = Time.realtimeSinceStartup;
        var loadingTimeout = loadingStarted + timeoutSeconds;
            
        // wait for scene to load
        while (Time.realtimeSinceStartup < loadingTimeout)
        {
            world.Update();
            
            if (SceneSystem.IsSceneLoaded(world.Unmanaged, subSceneEntity))
            {
                onCreated?.Invoke(new Result<World>.Success(world));
                // reset time (hopefully)
                world.SetTime(new TimeData(0,0));
                yield break;
            }
            yield return null;
        }
            
        // timeout
        onCreated?.Invoke(new Result<World>.Error(new Exception("scene load timed out")));
        SceneSystem.UnloadScene(world.Unmanaged, subSceneEntity);
        world.Dispose();
    }
}
*/