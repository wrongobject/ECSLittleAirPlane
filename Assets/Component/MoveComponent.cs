using Unity.Entities;
using Unity.Transforms;

using UnityEngine;
using Unity.Mathematics;
public struct MoveComponent : IComponentData
{
    public float2 dragDir;
    public float dragDirLenth;
    public float2 speed;

    public float2 acc;
    public float negAcc;        //减速
    public float maxSpeed;      //最大速度
}

