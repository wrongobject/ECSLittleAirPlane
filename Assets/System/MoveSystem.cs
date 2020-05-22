using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class MoveSystem : ComponentSystem
{
    float3 zAxis = new float3(0, 0, -1);
    protected override void OnUpdate()
    {
        EntityQueryBuilder queryBuilder = Entities.WithAll(typeof(MoveComponent),typeof(Translation),typeof(EnemyComponent));
       
        queryBuilder.ForEach((ref MoveComponent move, ref Translation translation, ref EnemyComponent enemyComponent) => {
            //move.speed += move.acc * Time.DeltaTime;
            translation.Value += new float3( move.speed * Time.DeltaTime,0);
        });
      
        queryBuilder = Entities.WithAll(typeof(MoveComponent), typeof(Translation), typeof(Rotation));
        queryBuilder.ForEach((ref MoveComponent move, ref Translation translation,ref Rotation rotation) =>
        {
            
            move.acc = move.dragDir * math.lerp(0,GameSetting.Instance.maxAccDirStep , move.dragDirLenth) * GameSetting.Instance.playerAccParam;

            move.speed += move.acc * Time.DeltaTime;
            move.speed = math.clamp(move.speed,-move.maxSpeed, move.maxSpeed);
         
            if (!move.speed.Equals(float2.zero))
            {
                float2 nspeed = math.normalize(move.speed);
                float angle = nspeed.x > 0 ? math.acos(nspeed.y) : -math.acos(nspeed.y);               
                rotation.Value = quaternion.AxisAngle(zAxis, angle );               
            }            
            translation.Value += new float3(move.speed * Time.DeltaTime,0);
        });

    }
}
