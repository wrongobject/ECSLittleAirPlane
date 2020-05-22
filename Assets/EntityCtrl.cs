using System;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using Unity.Rendering;
using System.Collections.Generic;
public class EntityCtrl : IDisposable
{
    World _world;   
    EntityArchetype _enemyType;
    EntityArchetype _playerType;
    public void Start()
    {
        _world = World.All[0];       
        _enemyType = _world.EntityManager.CreateArchetype(
            typeof(Translation),                                    
            typeof(EnemyComponent),
            typeof(MoveComponent),
            
            typeof(SpriteAnimComponent)
            );

        _playerType = _world.EntityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            
            typeof(PlayerComponent),
            typeof(MoveComponent),
            typeof(SpriteAnimComponent)
            );

    }

    public void CreateEnemy(int count)
    {
        NativeArray<Entity> entities = _world.EntityManager.CreateEntity(_enemyType, count, Allocator.Temp);
        foreach (var item in entities)
        {           
            _world.EntityManager.SetComponentData(item,new Translation() { Value = new float3(UnityEngine.Random.insideUnitCircle, 0) * GameSetting.Instance.enemySpawnRange});
            _world.EntityManager.SetComponentData(item, new SpriteAnimComponent() );
            _world.EntityManager.SetComponentData(item, new EnemyComponent());
            _world.EntityManager.SetComponentData(item, new MoveComponent() {
                acc = 0,
                speed = UnityEngine.Random.Range(0.1f, GameSetting.Instance.enemySpeedRange),
            });
        }
        entities.Dispose();
    }

    public void CreatePlayer()
    {
        Entity entity = _world.EntityManager.CreateEntity(_playerType);
        _world.EntityManager.SetComponentData(entity, new Translation() { Value = float3.zero });
        _world.EntityManager.SetComponentData(entity, new PlayerComponent());
        _world.EntityManager.SetComponentData(entity, new MoveComponent() {
            
            maxSpeed = GameSetting.Instance.playerMaxSpeed,
        });
        _world.EntityManager.SetComponentData(entity, new Rotation() { Value =  quaternion.identity });
    }

    public void Dispose()
    {        
        _world = null;
        _world.EntityManager.DestroyEntity(_world.EntityManager.UniversalQuery);
    }
}

