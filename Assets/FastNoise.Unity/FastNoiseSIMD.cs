using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FastNoiseSIMD
{
    // Fields
    private readonly IntPtr nativePointer;
    private const string NATIVE_LIB = "FastNoiseSIMD_CLib";

    // Methods
    public FastNoiseSIMD(int seed = 0x539)
    {
        this.nativePointer = NewFastNoiseSIMD(seed);
    }

    public void FillNoiseSet(float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, float scaleModifier = 1f)
    {
        NativeFillNoiseSet(this.nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, scaleModifier);
    }

    public void FillNoiseSetVector(float[] noiseSet, VectorSet vectorSet, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
    {
        NativeFillNoiseSetVector(this.nativePointer, noiseSet, vectorSet.nativePointer, xOffset, yOffset, zOffset);
    }

    public void FillSampledNoiseSet(float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, int sampleScale)
    {
        NativeFillSampledNoiseSet(this.nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, sampleScale);
    }

    public void FillSampledNoiseSetVector(float[] noiseSet, VectorSet vectorSet, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
    {
        NativeFillSampledNoiseSetVector(this.nativePointer, noiseSet, vectorSet.nativePointer, xOffset, yOffset, zOffset);
    }

    ~FastNoiseSIMD()
    {
        NativeFree(this.nativePointer);
    }

    public float[] GetEmptyNoiseSet(int xSize, int ySize, int zSize) =>
        new float[(xSize * ySize) * zSize];

    public float[] GetNoiseSet(int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, float scaleModifier = 1f)
    {
        float[] noiseSet = this.GetEmptyNoiseSet(xSize, ySize, zSize);
        NativeFillNoiseSet(this.nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, scaleModifier);
        return noiseSet;
    }

    public float[] GetSampledNoiseSet(int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, int sampleScale)
    {
        float[] noiseSet = this.GetEmptyNoiseSet(xSize, ySize, zSize);
        NativeFillSampledNoiseSet(this.nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, sampleScale);
        return noiseSet;
    }

    public int GetSeed() =>
        NativeGetSeed(this.nativePointer);

    [DllImport(NATIVE_LIB)]
    public static extern int GetSIMDLevel();
    [DllImport(NATIVE_LIB)]
    private static extern void NativeFillNoiseSet(IntPtr nativePointer, float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, float scaleModifier);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeFillNoiseSetVector(IntPtr nativePointer, float[] noiseSet, IntPtr vectorSetPointer, float xOffset, float yOffset, float zOffset);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeFillSampledNoiseSet(IntPtr nativePointer, float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, int sampleScale);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeFillSampledNoiseSetVector(IntPtr nativePointer, float[] noiseSet, IntPtr vectorSetPointer, float xOffset, float yOffset, float zOffset);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeFree(IntPtr nativePointer);
    [DllImport(NATIVE_LIB)]
    private static extern int NativeGetSeed(IntPtr nativePointer);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetAxisScales(IntPtr nativePointer, float xScale, float yScale, float zScale);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetCellularDistance2Indicies(IntPtr nativePointer, int cellularDistanceIndex0, int cellularDistanceIndex1);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetCellularDistanceFunction(IntPtr nativePointer, int distanceFunction);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetCellularJitter(IntPtr nativePointer, float cellularJitter);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetCellularNoiseLookupFrequency(IntPtr nativePointer, float freq);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetCellularNoiseLookupType(IntPtr nativePointer, int noiseType);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetCellularReturnType(IntPtr nativePointer, int returnType);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetFractalGain(IntPtr nativePointer, float gain);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetFractalLacunarity(IntPtr nativePointer, float lacunarity);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetFractalOctaves(IntPtr nativePointer, int octaves);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetFractalType(IntPtr nativePointer, int fractalType);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetFrequency(IntPtr nativePointer, float freq);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetNoiseType(IntPtr nativePointer, int noiseType);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbAmp(IntPtr nativePointer, float perturbAmp);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbFractalGain(IntPtr nativePointer, float perturbFractalGain);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbFractalLacunarity(IntPtr nativePointer, float perturbFractalLacunarity);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbFractalOctaves(IntPtr nativePointer, int perturbOctaves);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbFrequency(IntPtr nativePointer, float perturbFreq);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbNormaliseLength(IntPtr nativePointer, float perturbNormaliseLength);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetPerturbType(IntPtr nativePointer, int perturbType);
    [DllImport(NATIVE_LIB)]
    private static extern void NativeSetSeed(IntPtr nativePointer, int seed);
    [DllImport(NATIVE_LIB)]
    private static extern IntPtr NewFastNoiseSIMD(int seed);
    public void SetAxisScales(float xScale, float yScale, float zScale)
    {
        NativeSetAxisScales(this.nativePointer, xScale, yScale, zScale);
    }

    public void SetCellularDistance2Indicies(int cellularDistanceIndex0, int cellularDistanceIndex1)
    {
        NativeSetCellularDistance2Indicies(this.nativePointer, cellularDistanceIndex0, cellularDistanceIndex1);
    }

    public void SetCellularDistanceFunction(CellularDistanceFunction cellularDistanceFunction)
    {
        NativeSetCellularDistanceFunction(this.nativePointer, (int)cellularDistanceFunction);
    }

    public void SetCellularJitter(float cellularJitter)
    {
        NativeSetCellularJitter(this.nativePointer, cellularJitter);
    }

    public void SetCellularNoiseLookupFrequency(float cellularNoiseLookupFrequency)
    {
        NativeSetCellularNoiseLookupFrequency(this.nativePointer, cellularNoiseLookupFrequency);
    }

    public void SetCellularNoiseLookupType(NoiseType cellularNoiseLookupType)
    {
        NativeSetCellularNoiseLookupType(this.nativePointer, (int)cellularNoiseLookupType);
    }

    public void SetCellularReturnType(CellularReturnType cellularReturnType)
    {
        NativeSetCellularReturnType(this.nativePointer, (int)cellularReturnType);
    }

    public void SetFractalGain(float gain)
    {
        NativeSetFractalGain(this.nativePointer, gain);
    }

    public void SetFractalLacunarity(float lacunarity)
    {
        NativeSetFractalLacunarity(this.nativePointer, lacunarity);
    }

    public void SetFractalOctaves(int octaves)
    {
        NativeSetFractalOctaves(this.nativePointer, octaves);
    }

    public void SetFractalType(FractalType fractalType)
    {
        NativeSetFractalType(this.nativePointer, (int)fractalType);
    }

    public void SetFrequency(float frequency)
    {
        NativeSetFrequency(this.nativePointer, frequency);
    }

    public void SetNoiseType(NoiseType noiseType)
    {
        NativeSetNoiseType(this.nativePointer, (int)noiseType);
    }

    public void SetPerturbAmp(float perturbAmp)
    {
        NativeSetPerturbAmp(this.nativePointer, perturbAmp);
    }

    public void SetPerturbFractalGain(float perturbFractalGain)
    {
        NativeSetPerturbFractalGain(this.nativePointer, perturbFractalGain);
    }

    public void SetPerturbFractalLacunarity(float perturbFractalLacunarity)
    {
        NativeSetPerturbFractalLacunarity(this.nativePointer, perturbFractalLacunarity);
    }

    public void SetPerturbFractalOctaves(int perturbOctaves)
    {
        NativeSetPerturbFractalOctaves(this.nativePointer, perturbOctaves);
    }

    public void SetPerturbFrequency(float perturbFreq)
    {
        NativeSetPerturbFrequency(this.nativePointer, perturbFreq);
    }

    public void SetPerturbNormaliseLength(float perturbNormaliseLength)
    {
        NativeSetPerturbNormaliseLength(this.nativePointer, perturbNormaliseLength);
    }

    public void SetPerturbType(PerturbType perturbType)
    {
        NativeSetPerturbType(this.nativePointer, (int)perturbType);
    }

    public void SetSeed(int seed)
    {
        NativeSetSeed(this.nativePointer, seed);
    }

    [DllImport(NATIVE_LIB)]
    public static extern void SetSIMDLevel(int level);

    // Nested Types
    public enum CellularDistanceFunction
    {
        Euclidean,
        Manhattan,
        Natural
    }

    public enum CellularReturnType
    {
        CellValue,
        Distance,
        Distance2,
        Distance2Add,
        Distance2Sub,
        Distance2Mul,
        Distance2Div,
        NoiseLookup,
        Distance2Cave
    }

    public enum FractalType
    {
        FBM,
        Billow,
        RigidMulti
    }

    public enum NoiseType
    {
        Value,
        ValueFractal,
        Perlin,
        PerlinFractal,
        Simplex,
        SimplexFractal,
        WhiteNoise,
        Cellular
    }

    public enum PerturbType
    {
        None,
        Gradient,
        GradientFractal,
        Normalise,
        Gradient_Normalise,
        GradientFractal_Normalise
    }

    public class VectorSet
    {
        // Fields
        internal readonly IntPtr nativePointer;

        // Methods
        public VectorSet(Vector3[] vectors, int sampleSizeX = -1, int sampleSizeY = -1, int sampleSizeZ = -1, int samplingScale = 0)
        {
            float[] vectorSetArray = new float[vectors.Length * 3];
            for (int i = 0; i < vectors.Length; i++)
            {
				Vector3 currentVector = vectors[i];
                vectorSetArray[i] = currentVector.x;
                vectorSetArray[i + vectors.Length] = currentVector.y;
                vectorSetArray[i + (vectors.Length * 2)] = currentVector.z;
            }
            this.nativePointer = NewVectorSet(vectorSetArray, vectorSetArray.Length, samplingScale, sampleSizeX, sampleSizeY, sampleSizeZ);
        }

        ~VectorSet()
        {
            NativeFreeVectorSet(this.nativePointer);
        }

        [DllImport(NATIVE_LIB)]
        private static extern void NativeFreeVectorSet(IntPtr nativePointer);
        [DllImport(NATIVE_LIB)]
        private static extern IntPtr NewVectorSet(float[] vectorSetArray, int arraySize, int samplingScale, int sampleSizeX, int sampleSizeY, int sampleSizeZ);
    }
}

