using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

[UpdateAfter(typeof(SortSystem))]
public class SpriteAnimSystem : ComponentSystem
{
    MaterialPropertyBlock block = new MaterialPropertyBlock();
    List<Matrix4x4> matrices = new List<Matrix4x4>();
    //protected override void OnUpdate()
    //{
    //    matrices.Clear();
    //    block.Clear();
    //    EntityQuery entityQuery = GetEntityQuery(typeof(Translation),typeof(EnemyComponent));
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
    //    }      

    //    Graphics.DrawMeshInstanced(GameSetting.Instance.entityMesh, 0, GameSetting.Instance.enemyMat, matrices, block);

    //    EntityQueryBuilder queryBuilder = Entities.WithNone(typeof(EnemyComponent)).WithAll(typeof(Translation));
    //    queryBuilder.ForEach((ref Translation translation,ref Rotation rotation) =>
    //    {
    //        Graphics.DrawMesh(GameSetting.Instance.entityMesh,translation.Value, rotation.Value, GameSetting.Instance.playerMat,LayerMask.GetMask("Default"));
    //    });


    //}

    protected override void OnUpdate()
    {
        matrices.Clear();
        block.Clear();
        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(EnemyComponent));


        NativeQueue<RenderData> renderQueue = new NativeQueue<RenderData>();
        Camera camera = Camera.main;
        float ctop = camera.transform.position.y + camera.orthographicSize;
        float cbottom = camera.transform.position.y - camera.orthographicSize;
        SpriteCullJob cullJob = new SpriteCullJob()
        {
            top = ctop,
            bottom = cbottom,
            nativeQueue = renderQueue,
        };

        JobHandle handle = cullJob.Schedule(entityQuery);
        handle.Complete();
        
    }
}

