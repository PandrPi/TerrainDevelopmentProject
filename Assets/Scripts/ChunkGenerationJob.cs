using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct ChunkGenerationJob : IJob
{
    public GenerationDataComponent data;
    public int pointsNumber;
    public NoiseGroup noiseGroup;
    public Vector3[] Vertices;
    public float[] noise1;
    public float[] noise2;

    public void Execute()
    {
        noise1 = noiseGroup.fastNoise1.GetEmptyNoiseSet(pointsNumber, pointsNumber, 1);
        noise2 = noiseGroup.fastNoise2.GetEmptyNoiseSet(pointsNumber, pointsNumber, 1);
        noiseGroup.fastNoise1.FillSampledNoiseSetVector(noise1, new FastNoiseSIMD.VectorSet(Vertices), data.xStart, data.yStart, data.zStart);
        noiseGroup.fastNoise2.FillSampledNoiseSetVector(noise2, new FastNoiseSIMD.VectorSet(Vertices), data.xStart, data.yStart, data.zStart);
    }
}
