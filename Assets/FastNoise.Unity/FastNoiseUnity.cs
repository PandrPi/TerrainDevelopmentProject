using UnityEngine;

[AddComponentMenu("FastNoise/FastNoise Unity", 1)]
public class FastNoiseUnity : MonoBehaviour
{
    // Fields
    public FastNoise fastNoise = new FastNoise(0x539);
    public string noiseName = "Default Noise";
    public int seed = 0x539;
    public float frequency = 0.01f;
    public FastNoise.Interp interp = FastNoise.Interp.Quintic;
    public FastNoise.NoiseType noiseType = FastNoise.NoiseType.Simplex;
    public int octaves = 3;
    public float lacunarity = 2f;
    public float gain = 0.5f;
    public FastNoise.FractalType fractalType;
    public FastNoise.CellularDistanceFunction cellularDistanceFunction;
    public FastNoise.CellularReturnType cellularReturnType;
    public FastNoiseUnity cellularNoiseLookup;
    public int cellularDistanceIndex0;
    public int cellularDistanceIndex1 = 1;
    public float cellularJitter = 0.45f;
    public float gradientPerturbAmp = 1f;

    // Methods
    private void Awake()
    {
        this.SaveSettings();
    }

    public void SaveSettings()
    {
        this.fastNoise.SetSeed(this.seed);
        this.fastNoise.SetFrequency(this.frequency);
        this.fastNoise.SetInterp(this.interp);
        this.fastNoise.SetNoiseType(this.noiseType);
        this.fastNoise.SetFractalOctaves(this.octaves);
        this.fastNoise.SetFractalLacunarity(this.lacunarity);
        this.fastNoise.SetFractalGain(this.gain);
        this.fastNoise.SetFractalType(this.fractalType);
        this.fastNoise.SetCellularDistanceFunction(this.cellularDistanceFunction);
        this.fastNoise.SetCellularReturnType(this.cellularReturnType);
        this.fastNoise.SetCellularJitter(this.cellularJitter);
        this.fastNoise.SetCellularDistance2Indicies(this.cellularDistanceIndex0, this.cellularDistanceIndex1);
        if (this.cellularNoiseLookup)
        {
            this.fastNoise.SetCellularNoiseLookup(this.cellularNoiseLookup.fastNoise);
        }
        this.fastNoise.SetGradientPerturbAmp(this.gradientPerturbAmp);
    }
}

