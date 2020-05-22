using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class SpriteAnimSystem : ComponentSystem
{
    const int  blockSize = 1023;

    MaterialPropertyBlock block = new MaterialPropertyBlock();
    List<Matrix4x4> matrices = new List<Matrix4x4>();
    //protected override void OnUpdate()
    //{
    //    matrices.Clear();
    //    block.Clear();
    //    EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(EnemyComponent));
    //    NativeArray<Translation> nativeArray = entityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
    //    for (int i = 0; i < nativeArray.Length; i++)
    //    {
    //        for (int j = i; j < nativeArray.Length - 1; j++)
    //        {
    //            if (nativeArray[j].Value.y < nativeArray[j + 1].Value.y)
    //            {
    //                Translation temp = nativeArray[j];
    //                nativeArray[j] = nativeArray[j + 1];
    //                nativeArray[j + 1] = temp;
    //            }
    //        }
    //    }

    //    foreach (var item in nativeArray)
    //    {
    //        matrices.Add(Matrix4x4.TRS(item.Value, Quaternion.identity, Vector3.one));
    //        if (matrices.Count >= 1023)
    //        {
    //            Graphics.DrawMeshInstanced(GameSetting.Instance.entityMesh, 0, GameSetting.Instance.enemyMat, matrices, block);
    //            matrices.Clear();
    //        }
    //    }

    //    Graphics.DrawMeshInstanced(GameSetting.Instance.entityMesh, 0, GameSetting.Instance.enemyMat, matrices, block);

    //    EntityQueryBuilder queryBuilder = Entities.WithNone(typeof(EnemyComponent)).WithAll(typeof(Translation));
    //    queryBuilder.ForEach((ref Translation translation, ref Rotation rotation) =>
    //    {
    //        Graphics.DrawMesh(GameSetting.Instance.entityMesh, translation.Value, rotation.Value, GameSetting.Instance.playerMat, LayerMask.GetMask("Default"));
    //    });


    //}

    protected override void OnUpdate()
    {
        matrices.Clear();
        block.Clear();
        int jobHandleindex = 0;
        NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(4, Allocator.Temp);
        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(EnemyComponent));


        NativeQueue<RenderData> renderQueue0 = new NativeQueue<RenderData>(Allocator.TempJob);
        NativeQueue<RenderData> renderQueue1 = new NativeQueue<RenderData>(Allocator.TempJob);
        NativeQueue<RenderData> renderQueue2 = new NativeQueue<RenderData>(Allocator.TempJob);
        NativeQueue<RenderData> renderQueue3 = new NativeQueue<RenderData>(Allocator.TempJob);
        NativeQueue<Entity> toRemoveEntity = new NativeQueue<Entity>(Allocator.TempJob);
        Camera camera = Camera.main;

        NativeQueue<RenderData>.ParallelWriter pw0 = renderQueue0.AsParallelWriter();
        NativeQueue<RenderData>.ParallelWriter pw1 = renderQueue1.AsParallelWriter();
        NativeQueue<RenderData>.ParallelWriter pw2 = renderQueue2.AsParallelWriter();
        NativeQueue<RenderData>.ParallelWriter pw3 = renderQueue3.AsParallelWriter();

        NativeQueue<Entity>.ParallelWriter tre = toRemoveEntity.AsParallelWriter();
        float halfW = camera.orthographicSize / Screen.currentResolution.height * Screen.currentResolution.width;
        SpriteCullJob cullJob = new SpriteCullJob()
        {
            top = camera.transform.position.y + camera.orthographicSize,
            bottom = camera.transform.position.y - camera.orthographicSize,
            left = camera.transform.position.x - halfW,
            right = camera.transform.position.x + halfW,
            nativeQueue0 = pw0,
            nativeQueue1 = pw1,
            nativeQueue2 = pw2,
            nativeQueue3 = pw3,
            typeTranslation = GetArchetypeChunkComponentType<Translation>(),
            entityType = GetArchetypeChunkEntityType(),
            needRemoveEntity = tre,
        };

        JobHandle handle = cullJob.Schedule(entityQuery);
        handle.Complete();

        NativeArray<JobHandle> switchHandles = new NativeArray<JobHandle>(4, Allocator.TempJob);
        int switchJobIndex = 0;
        NativeArray<RenderData> rd0 = new NativeArray<RenderData>(renderQueue0.Count, Allocator.TempJob),
            rd1 = new NativeArray<RenderData>(renderQueue1.Count, Allocator.TempJob),
            rd2 = new NativeArray<RenderData>(renderQueue2.Count, Allocator.TempJob),
            rd3 = new NativeArray<RenderData>(renderQueue3.Count, Allocator.TempJob);
        NativeQueue2ArrayJob nativeQueue2Array0 = new NativeQueue2ArrayJob()
        {
            nativeQueue = renderQueue0,
            nativeArray = rd0
        };
        switchHandles[switchJobIndex++] = nativeQueue2Array0.Schedule();
        NativeQueue2ArrayJob nativeQueue2Array1 = new NativeQueue2ArrayJob()
        {
            nativeQueue = renderQueue1,
            nativeArray = rd1
        };
        switchHandles[switchJobIndex++] = nativeQueue2Array1.Schedule();
        NativeQueue2ArrayJob nativeQueue2Array2 = new NativeQueue2ArrayJob()
        {
            nativeQueue = renderQueue2,
            nativeArray = rd2
        };
        switchHandles[switchJobIndex++] = nativeQueue2Array2.Schedule();
        NativeQueue2ArrayJob nativeQueue2Array3 = new NativeQueue2ArrayJob()
        {
            nativeQueue = renderQueue3,
            nativeArray = rd3
        };
        switchHandles[switchJobIndex++] = nativeQueue2Array3.Schedule();

        JobHandle.CompleteAll(switchHandles);
        switchHandles.Dispose();

        renderQueue0.Dispose();
        renderQueue1.Dispose();
        renderQueue2.Dispose();
        renderQueue3.Dispose();
        SpriteSortJobs sortJobs0 = new SpriteSortJobs()
        {
            renderDatas = rd0,
        };
        jobHandles[jobHandleindex++] = sortJobs0.Schedule();

        SpriteSortJobs sortJobs1 = new SpriteSortJobs()
        {
            renderDatas = rd1,
        };
        jobHandles[jobHandleindex++] = sortJobs1.Schedule();

        SpriteSortJobs sortJobs2 = new SpriteSortJobs()
        {
            renderDatas = rd2,
        };
        jobHandles[jobHandleindex++] = sortJobs2.Schedule();

        SpriteSortJobs sortJobs3 = new SpriteSortJobs()
        {
            renderDatas = rd3,
        };
        jobHandles[jobHandleindex++] = sortJobs3.Schedule();

        JobHandle.CompleteAll(jobHandles);
        foreach (var item in rd0)
        {
            AddData(item);
        }
        foreach (var item in rd1)
        {
            AddData(item);

        }
        foreach (var item in rd2)
        {
            AddData(item);
        }
        foreach (var item in rd3)
        {
            AddData(item);
        }
        if (matrices.Count > 0)
        {
            Graphics.DrawMeshInstanced(GameSetting.Instance.entityMesh, 0, GameSetting.Instance.enemyMat, matrices, block);
        }
        rd0.Dispose();
        rd1.Dispose();
        rd2.Dispose();
        rd3.Dispose();
        jobHandles.Dispose();

        NativeArray<Entity> toRemoveEntityArray = toRemoveEntity.ToArray(Allocator.Temp);
        foreach (var item in toRemoveEntityArray)
        {
            this.EntityManager.DestroyEntity(item);
        }
        toRemoveEntity.Dispose();
        toRemoveEntityArray.Dispose();


        EntityQueryBuilder queryBuilder = Entities.WithNone(typeof(EnemyComponent)).WithAll(typeof(Translation));
        queryBuilder.ForEach((ref Translation translation, ref Rotation rotation) =>
        {
            Graphics.DrawMesh(GameSetting.Instance.entityMesh, translation.Value, rotation.Value, GameSetting.Instance.playerMat, LayerMask.GetMask("Default"));
        });
    }

    private void AddData(RenderData data)
    {
        matrices.Add(Matrix4x4.TRS(data.position, Quaternion.identity, Vector3.one));
        if (matrices.Count >= blockSize)
        {
            Graphics.DrawMeshInstanced(GameSetting.Instance.entityMesh, 0, GameSetting.Instance.enemyMat, matrices, block);
            matrices.Clear();
        }
    }
}

