using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public class CollisionSystem : JobComponentSystem
{   

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery enemyQuery = GetEntityQuery(typeof(EnemyComponent));
        EntityQuery playerQuery = GetEntityQuery(typeof(PlayerComponent));

        //CollissionDetectJob detectJob = new CollissionDetectJob() {
        //    enemy = enemyQuery,
        //    player = playerQuery,
        //};
        //return detectJob.Schedule(inputDeps);
        JobHandle handle = Entities.ForEach((ref Entity entity,ref Translation translation) =>
        {

        }).Schedule(inputDeps);
        return handle;
    }
}

public struct CollissionDetectJob : IJobChunk
{
    public EntityQuery enemy;
    public EntityQuery player;
    public void Execute()
    {
        
    }

    void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    {
        
    }
}

