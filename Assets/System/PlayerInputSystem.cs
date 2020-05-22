using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
public class PlayerInputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        EntityQueryBuilder queryBuilder = Entities.WithAll(typeof(MoveComponent),typeof(PlayerComponent));
        queryBuilder.ForEach((ref MoveComponent moveComponent,ref PlayerComponent playerComponent) => {
            float2 dir = float2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                dir.y = dir.y + 1;                
            }
            if (Input.GetKey(KeyCode.S))
            {
                dir.y = dir.y - 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = dir.x - 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                dir.x = dir.x + 1;
            }
            if (!dir.Equals(float2.zero))
            {
                moveComponent.dragDir = math.normalize(dir);
                moveComponent.dragDirLenth = math.length(dir);
            }
            else
            {
                moveComponent.dragDir = dir;
                moveComponent.dragDirLenth = 0;
            }
        });
    }
}


