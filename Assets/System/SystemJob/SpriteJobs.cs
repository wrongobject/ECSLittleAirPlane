using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public struct RenderData
{
    public float3 position;
}

public struct SpriteSortJobs : IJob
{
    public void Execute()
    {
        
    }
}

public struct SpriteCullJob : IJobChunk
{
    public float top;
    public float bottom;
    public NativeQueue<RenderData> nativeQueue;
    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    {
        var componentArray = chunk.GetNativeArray(new ArchetypeChunkComponentType<Translation>());
        var itor = componentArray.GetEnumerator();
        while (itor.MoveNext())
        {
            if (itor.Current.Value.y >= bottom && itor.Current.Value.y < top)
            {
                nativeQueue.Enqueue(new RenderData()
                {
                    position = itor.Current.Value,
                });
            }
        }
        itor.Dispose();
    }
}