using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public struct RenderData
{
    public float3 position;
}

public struct NativeQueue2ArrayJob : IJob
{
    public NativeQueue<RenderData> nativeQueue;
    public NativeArray<RenderData> nativeArray;
    public void Execute()
    {
        int index = 0;
        RenderData data;
        while (nativeQueue.TryDequeue(out data))
        {
            nativeArray[index] = data;
            index++;
        }
    }
}
[BurstCompile]
public struct SpriteSortJobs : IJob
{
    public NativeArray<RenderData> renderDatas;
    public void Execute()
    {
        for (int i = 0; i < renderDatas.Length; i++)
        {
            for (int j = i; j < renderDatas.Length - 1; j++)
            {
                if (renderDatas[j].position.y < renderDatas[j + 1].position.y)
                {
                    RenderData temp = renderDatas[j];
                    renderDatas[j] = renderDatas[j + 1];
                    renderDatas[j + 1] = temp;
                }
            }
        }
    }
}
[BurstCompile]
public struct SpriteCullJob : IJobChunk
{  
    public float left;
    public float right;
    public float top;
    public float bottom;
    [ReadOnly] public ArchetypeChunkComponentType<Translation> typeTranslation;
    [ReadOnly] public ArchetypeChunkEntityType entityType;

    public NativeQueue<RenderData>.ParallelWriter nativeQueue0;
    public NativeQueue<RenderData>.ParallelWriter nativeQueue1;
    public NativeQueue<RenderData>.ParallelWriter nativeQueue2;
    public NativeQueue<RenderData>.ParallelWriter nativeQueue3;
    public NativeQueue<Entity>.ParallelWriter needRemoveEntity;
    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    {
        int index = 0;
        var componentArray = chunk.GetNativeArray(typeTranslation);
        var entityArray = chunk.GetNativeArray(entityType);
        var itor = componentArray.GetEnumerator();
        while (itor.MoveNext())
        {
            float3 pos = itor.Current.Value;
            if (pos.x < 0 && pos.x >= left && pos.y >= 0 && pos.y < top)
            {
                nativeQueue0.Enqueue(new RenderData()
                {
                    position = itor.Current.Value,
                });
            }
            else if (pos.x >= 0 && pos.x < right && pos.y >= 0 && pos.y < top)
            {
                nativeQueue1.Enqueue(new RenderData()
                {
                    position = itor.Current.Value,
                });
            }
            else if (pos.x < 0 && pos.x >= left && pos.y < 0 && pos.y >= bottom)
            {
                nativeQueue2.Enqueue(new RenderData()
                {
                    position = itor.Current.Value,
                });
            }
            else if (pos.x >= 0 && pos.x < right && pos.y < 0 && pos.y >= bottom)
            {
                nativeQueue3.Enqueue(new RenderData()
                {
                    position = itor.Current.Value,
                });
            }
            else
            {
                needRemoveEntity.Enqueue(entityArray[index]);
            }
            index++;
        }
        itor.Dispose();
    }
}