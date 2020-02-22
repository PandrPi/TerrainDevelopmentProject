using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct GenerationDataComponent : IComponentData
{
    public float4x4 ChunkToPlanet;
    public float4x4 PlanetToChunk;
    public int xStart;
    public int yStart;
    public int zStart;

    public GenerationDataComponent(Matrix4x4 chunkMatrix, Matrix4x4 worldMatrix, int xStart, int yStart, int zStart)
    {
        float4x4 chunk = new float4x4(chunkMatrix.GetColumn(0), chunkMatrix.GetColumn(1), chunkMatrix.GetColumn(2), chunkMatrix.GetColumn(3));
        float4x4 world = new float4x4(worldMatrix.GetColumn(0), worldMatrix.GetColumn(1), worldMatrix.GetColumn(2), worldMatrix.GetColumn(3));
        this.ChunkToPlanet = math.mul(math.inverse(world), chunk);
        this.PlanetToChunk = math.mul(math.inverse(chunk), world);
        this.xStart = xStart;
        this.yStart = yStart;
        this.zStart = zStart;
    }
}
