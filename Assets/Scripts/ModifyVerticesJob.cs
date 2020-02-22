using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ModifyVerticesJob : IJobParallelFor
{
    public ModifyVerticesDataGroup dataGroup;

    private static readonly float3 up = new float3(0, 1, 0);

    public void Execute(int i)
    {
        Vector3 tempVec = dataGroup.Vertices[i];
        float4 tempFloat4 = new float4(tempVec.x, tempVec.y, tempVec.z, 1f);

        float3 ctpPos = math.mul(dataGroup.ChunkToPlanet, tempFloat4).xyz;

        float y = dataGroup.Noise1[i] * 50 + dataGroup.Noise2[i];

        ctpPos += up * y;

        ctpPos = math.mul(dataGroup.PlanetToChunk, new float4(ctpPos, 1f)).xyz;
        tempVec.x = ctpPos.x;
        tempVec.y = ctpPos.y;
        tempVec.z = ctpPos.z;
        dataGroup.Vertices[i] = tempVec;
    }
}

public struct ModifyVerticesDataGroup
{
    public NativeArray<Vector3> Vertices;
    public NativeArray<float> Noise1;
    public NativeArray<float> Noise2;
    public float4x4 ChunkToPlanet;
    public float4x4 PlanetToChunk;

    public ModifyVerticesDataGroup(Vector3[] Vertices, float[] Noise1, float[] Noise2, Matrix4x4 ctp, Matrix4x4 ptc)
    {
        this.Vertices = new NativeArray<Vector3>(Vertices, Allocator.TempJob);
        this.Noise1 = new NativeArray<float>(Noise1, Allocator.TempJob);
        this.Noise2 = new NativeArray<float>(Noise2, Allocator.TempJob);
        this.ChunkToPlanet = new float4x4(ctp.GetColumn(0), ctp.GetColumn(1), ctp.GetColumn(2), ctp.GetColumn(3));
        this.PlanetToChunk = new float4x4(ptc.GetColumn(0), ptc.GetColumn(1), ptc.GetColumn(2), ptc.GetColumn(3));
    }

    public void Dispose()
    {
        Vertices.Dispose();
        Noise1.Dispose();
        Noise2.Dispose();
    }
}
