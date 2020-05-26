using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

[UpdateAfter(typeof(MoveSystem))]
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

        NativeArray<CollisionData> enemyCollisionDatas = new NativeArray<CollisionData>(enemies.Length, Allocator.TempJob);
        for (int i = 0; i < enemies.Length; i++)
        {
            CollisionData data = new CollisionData();           
            data.entity = enemies[i];
            data.position = enemyTrans[i].Value;
            data.radius = GameSetting.Instance.enemyRadius + GameSetting.Instance.playerRadius;
            enemyCollisionDatas[i] = data;
        }
        enemies.Dispose();
        enemyTrans.Dispose();

        NativeQueue<Entity> collisionEnemies = new NativeQueue<Entity>(Allocator.TempJob);
        CollisionDetectJob job = new CollisionDetectJob() {
            collisionDatas = enemyCollisionDatas,
            translationType = GetArchetypeChunkComponentType<Translation>(),
            entityType = GetArchetypeChunkEntityType(),
            collisionEntity = collisionEnemies.AsParallelWriter(),
        };
        JobHandle handle = job.Schedule(playerQuery);
        handle.Complete();

        
        if (collisionEnemies.Count > 0)
        {
            NativeArray<Entity> removeEntity = collisionEnemies.ToArray(Allocator.Temp);
            this.EntityManager.DestroyEntity(removeEntity);
            
            
            //foreach (var item in removeEntity)
            //{
            //    Debug.LogFormat("remove count:{0},index:{1},version:{2}" , removeEntity.Length,item.Index,item.Version);
            //}
            removeEntity.Dispose();
        }
        collisionEnemies.Dispose();
        enemyCollisionDatas.Dispose();
       
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
                        collisionEntity.Enqueue(collisionDatas[i].entity);
                        Debug.LogFormat("enqueue,length:{0},radius:{1},index:{2}",length,collisionDatas[i].radius, collisionDatas[i].entity.Index);
                    }
                }
            }

            

        }
    }
}


