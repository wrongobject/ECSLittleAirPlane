using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using LocalRandom = Unity.Mathematics.Random;
public class AutoInputSystem : ComponentSystem
{
    LocalRandom rand = new LocalRandom((uint)System.DateTime.Now.Millisecond);

    protected override void OnUpdate()
    {
        EntityQueryBuilder queryBuilder = Entities.WithAll(typeof(MoveComponent), typeof(EnemyComponent));
        queryBuilder.ForEach((ref MoveComponent moveComponent,ref EnemyComponent enemyComponent)=> {
            moveComponent.dragDir = rand.NextFloat2(1) - 0.5f;
        });
    }
}

