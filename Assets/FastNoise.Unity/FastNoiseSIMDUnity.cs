using UnityEngine;

[AddComponentMenu("FastNoise/FastNoiseSIMD Unity", 2)]
public class FastNoiseSIMDUnity : MonoBehaviour
{
    // Fields
    public FastNoiseSIMD fastNoiseSIMD = new FastNoiseSIMD(0x539);
    public string noiseName = "Default Noise";
    public int seed = 0x539;
    public float frequency = 0.01f;
    public FastNoiseSIMD.NoiseType noiseType = FastNoiseSIMD.NoiseType.Simplex;
    public Vector3 axisScales = Vector3.one;
    public int octaves = 3;
    public float lacunarity = 2f;
    public float gain = 0.5f;
    public FastNoiseSIMD.FractalType fractalType;
    public FastNoiseSIMD.CellularDistanceFunction cellularDistanceFunction;
    public FastNoiseSIMD.CellularReturnType cellularReturnType = FastNoiseSIMD.CellularReturnType.Distance;
    public FastNoiseSIMD.NoiseType cellularNoiseLookupType = FastNoiseSIMD.NoiseType.Simplex;
    public float cellularNoiseLookupFrequency = 0.2f;
    public int cellularDistanceIndex0;
    public int cellularDistanceIndex1 = 1;
    public float cellularJitter = 0.45f;
    public FastNoiseSIMD.PerturbType perturbType;
    public float perturbAmp = 1f;
    public float perturbFrequency = 0.5f;
    public float perturbNormaliseLength = 1f;
    public int perturbOctaves = 3;
    public float perturbLacunarity = 2f;
    public float perturbGain = 0.5f;

    private void Awake()
    {
        this.InitNoise(fastNoiseSIMD);
    }

    public void InitNoise(FastNoiseSIMD noise)
    {
        noise.SetSeed(this.seed);
        noise.SetFrequency(this.frequency);
        noise.SetNoiseType(this.noiseType);
        noise.SetAxisScales(this.axisScales.x, this.axisScales.y, this.axisScales.z);
        noise.SetFractalOctaves(this.octaves);
        noise.SetFractalLacunarity(this.lacunarity);
        noise.SetFractalGain(this.gain);
        noise.SetFractalType(this.fractalType);
        noise.SetCellularDistanceFunction(this.cellularDistanceFunction);
        noise.SetCellularReturnType(this.cellularReturnType);
        noise.SetCellularNoiseLookupType(this.cellularNoiseLookupType);
        noise.SetCellularNoiseLookupFrequency(this.cellularNoiseLookupFrequency);
        noise.SetCellularDistance2Indicies(this.cellularDistanceIndex0, this.cellularDistanceIndex1);
        noise.SetCellularJitter(this.cellularJitter);
        noise.SetPerturbType(this.perturbType);
        noise.SetPerturbFrequency(this.perturbFrequency);
        noise.SetPerturbAmp(this.perturbAmp);
        noise.SetPerturbFractalOctaves(this.perturbOctaves);
        noise.SetPerturbFractalLacunarity(this.perturbLacunarity);
        noise.SetPerturbFractalGain(this.perturbGain);
        noise.SetPerturbNormaliseLength(this.perturbNormaliseLength);
    }
}

