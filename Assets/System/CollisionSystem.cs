using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public class CollisionSystem : ComponentSystem
{
    public struct CollisionData
    {
        public float3 position;
        public float radius;
        public Entity entity;
    }

    protected override void OnUpdate()
    {        
        EntityQuery enemyQuery = GetEntityQuery(typeof(EnemyComponent),typeof(Translation));
        EntityQuery playerQuery = GetEntityQuery(typeof(PlayerComponent), typeof(Translation));

        NativeArray<Entity> enemies = enemyQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> enemyTrans = enemyQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        NativeArray<CollisionData> collisionDatas = new NativeArray<CollisionData>(enemies.Length, Allocator.TempJob);
        for (int i = 0; i < enemies.Length; i++)
        {
            CollisionData data = new CollisionData();
            collisionDatas[i] = data;
            data.entity = enemies[i];
            data.position = enemyTrans[i].Value;
            data.radius = GameSetting.Instance.enemyRadius + GameSetting.Instance.playerRadius;
        }
        enemies.Dispose();
        enemyTrans.Dispose();

        NativeQueue<Entity> collisionEnemies = new NativeQueue<Entity>(Allocator.TempJob);
        CollisionDetectJob job = new CollisionDetectJob() {
            collisionDatas = collisionDatas,
            translationType = GetArchetypeChunkComponentType<Translation>(),
            entityType = GetArchetypeChunkEntityType(),
            collisionEntity = collisionEnemies.AsParallelWriter(),
        };
        JobHandle handle = job.Schedule(playerQuery);
        handle.Complete();

        NativeArray<Entity> removeEntity = collisionEnemies.ToArray(Allocator.Temp);
        for (int i = 0; i < removeEntity.Length; i++)
        {
            this.EntityManager.DestroyEntity(removeEntity[i]);
        }

        collisionEnemies.Dispose();
        collisionDatas.Dispose();
        removeEntity.Dispose();
    }

    [RequireComponentTag(typeof(PlayerComponent))]
    public struct CollisionDetectJob : IJobChunk
    {
        public NativeArray<CollisionData> collisionDatas;
        public ArchetypeChunkComponentType<Translation> translationType;
        [ReadOnly]public ArchetypeChunkEntityType entityType;
        public NativeQueue<Entity>.ParallelWriter collisionEntity;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Translation> translations = chunk.GetNativeArray(translationType);
            NativeArray<Entity> entityArray = chunk.GetNativeArray(entityType);
            for (int j= 0; j < translations.Length; j++)
            {
                for (int i = 0; i < collisionDatas.Length; i++)
                {
                    float3 delta = collisionDatas[i].position - translations[j].Value;
                    if (math.abs(delta.x) > collisionDatas[i].radius || math.abs(delta.y) > collisionDatas[i].radius)
                        continue;
                    float length = math.length(delta);
                    if (length <= collisionDatas[i].radius)
                    {
                        collisionEntity.Enqueue(entityArray[j]);
                    }
                }
            }

            

        }
    }
}


