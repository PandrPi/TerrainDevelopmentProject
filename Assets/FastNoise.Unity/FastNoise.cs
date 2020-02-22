using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class FastNoise
{
    // Fields
    private const short FN_INLINE = 0x100;
    private const int FN_CELLULAR_INDEX_MAX = 3;
    private int m_seed = 0x539;
    private float m_frequency = 0.01f;
    private Interp m_interp = Interp.Quintic;
    private NoiseType m_noiseType = NoiseType.Simplex;
    private int m_octaves = 3;
    private float m_lacunarity = 2f;
    private float m_gain = 0.5f;
    private FractalType m_fractalType;
    private float m_fractalBounding;
    private CellularDistanceFunction m_cellularDistanceFunction;
    private CellularReturnType m_cellularReturnType;
    private FastNoise m_cellularNoiseLookup;
    private int m_cellularDistanceIndex0;
    private int m_cellularDistanceIndex1 = 1;
    private float m_cellularJitter = 0.45f;
    private float m_gradientPerturbAmp = 1f;
    private static readonly Float2[] GRAD_2D = new Float2[] { new Float2(-1f, -1f), new Float2(1f, -1f), new Float2(-1f, 1f), new Float2(1f, 1f), new Float2(0f, -1f), new Float2(-1f, 0f), new Float2(0f, 1f), new Float2(1f, 0f) };
    private static readonly Float3[] GRAD_3D;
    private static readonly Float2[] CELL_2D;
    private static readonly Float3[] CELL_3D;
    private const int X_PRIME = 0x653;
    private const int Y_PRIME = 0x7a69;
    private const int Z_PRIME = 0x1b3b;
    private const int W_PRIME = 0x3f5;
    private const float F3 = 0.3333333f;
    private const float G3 = 0.1666667f;
    private const float G33 = -0.5f;
    private const float F2 = 0.5f;
    private const float G2 = 0.25f;
    private static readonly byte[] SIMPLEX_4D;
    private const float F4 = 0.309017f;
    private const float G4 = 0.1381966f;
    private const float CUBIC_3D_BOUNDING = 0.2962963f;
    private const float CUBIC_2D_BOUNDING = 0.4444444f;

    // Methods
    static FastNoise()
    {
        Float3[] numArray2 = new Float3[0x10];
        numArray2[0] = new Float3(1f, 1f, 0f);
        numArray2[1] = new Float3(-1f, 1f, 0f);
        numArray2[2] = new Float3(1f, -1f, 0f);
        numArray2[3] = new Float3(-1f, -1f, 0f);
        numArray2[4] = new Float3(1f, 0f, 1f);
        numArray2[5] = new Float3(-1f, 0f, 1f);
        numArray2[6] = new Float3(1f, 0f, -1f);
        numArray2[7] = new Float3(-1f, 0f, -1f);
        numArray2[8] = new Float3(0f, 1f, 1f);
        numArray2[9] = new Float3(0f, -1f, 1f);
        numArray2[10] = new Float3(0f, 1f, -1f);
        numArray2[11] = new Float3(0f, -1f, -1f);
        numArray2[12] = new Float3(1f, 1f, 0f);
        numArray2[13] = new Float3(0f, -1f, 1f);
        numArray2[14] = new Float3(-1f, 1f, 0f);
        numArray2[15] = new Float3(0f, -1f, -1f);
        GRAD_3D = numArray2;
        Float2[] numArray3 = new Float2[0x100];
        numArray3[0] = new Float2(-0.2700222f, -0.9628541f);
        numArray3[1] = new Float2(0.3863093f, -0.9223693f);
        numArray3[2] = new Float2(0.04444859f, -0.9990117f);
        numArray3[3] = new Float2(-0.5992523f, -0.8005602f);
        numArray3[4] = new Float2(-0.781928f, 0.6233687f);
        numArray3[5] = new Float2(0.9464672f, 0.3227999f);
        numArray3[6] = new Float2(-0.6514147f, -0.7587219f);
        numArray3[7] = new Float2(0.9378473f, 0.3470484f);
        numArray3[8] = new Float2(-0.8497876f, -0.5271252f);
        numArray3[9] = new Float2(-0.8790426f, 0.4767433f);
        numArray3[10] = new Float2(-0.8923003f, -0.4514424f);
        numArray3[11] = new Float2(-0.3798444f, -0.9250504f);
        numArray3[12] = new Float2(-0.9951651f, 0.09821638f);
        numArray3[13] = new Float2(0.7724398f, -0.635088f);
        numArray3[14] = new Float2(0.7573283f, -0.6530343f);
        numArray3[15] = new Float2(-0.9928005f, -0.1197801f);
        numArray3[0x10] = new Float2(-0.05326657f, 0.9985803f);
        numArray3[0x11] = new Float2(0.9754254f, -0.2203301f);
        numArray3[0x12] = new Float2(-0.7665018f, 0.6422421f);
        numArray3[0x13] = new Float2(0.9916367f, 0.1290606f);
        numArray3[20] = new Float2(-0.9946969f, 0.1028504f);
        numArray3[0x15] = new Float2(-0.5379205f, -0.8429955f);
        numArray3[0x16] = new Float2(0.5022815f, -0.8647041f);
        numArray3[0x17] = new Float2(0.4559821f, -0.8899889f);
        numArray3[0x18] = new Float2(-0.8659131f, -0.5001944f);
        numArray3[0x19] = new Float2(0.08794584f, -0.9961253f);
        numArray3[0x1a] = new Float2(-0.5051685f, 0.8630207f);
        numArray3[0x1b] = new Float2(0.7753185f, -0.6315704f);
        numArray3[0x1c] = new Float2(-0.6921945f, 0.721711f);
        numArray3[0x1d] = new Float2(-0.5191659f, -0.8546734f);
        numArray3[30] = new Float2(0.8978623f, -0.4402764f);
        numArray3[0x1f] = new Float2(-0.1706774f, 0.9853269f);
        numArray3[0x20] = new Float2(-0.935343f, -0.3537421f);
        numArray3[0x21] = new Float2(-0.9992405f, 0.03896747f);
        numArray3[0x22] = new Float2(-0.2882064f, -0.9575683f);
        numArray3[0x23] = new Float2(-0.9663811f, 0.2571138f);
        numArray3[0x24] = new Float2(-0.8759714f, -0.482363f);
        numArray3[0x25] = new Float2(-0.8303123f, -0.5572984f);
        numArray3[0x26] = new Float2(0.05110134f, -0.9986935f);
        numArray3[0x27] = new Float2(-0.8558373f, -0.5172451f);
        numArray3[40] = new Float2(0.09887026f, 0.9951003f);
        numArray3[0x29] = new Float2(0.9189016f, 0.3944868f);
        numArray3[0x2a] = new Float2(-0.2439376f, -0.9697909f);
        numArray3[0x2b] = new Float2(-0.8121409f, -0.5834613f);
        numArray3[0x2c] = new Float2(-0.9910432f, 0.1335421f);
        numArray3[0x2d] = new Float2(0.8492424f, -0.5280032f);
        numArray3[0x2e] = new Float2(-0.9717839f, -0.235873f);
        numArray3[0x2f] = new Float2(0.9949457f, 0.1004142f);
        numArray3[0x30] = new Float2(0.6241065f, -0.7813392f);
        numArray3[0x31] = new Float2(0.6629103f, 0.7486988f);
        numArray3[50] = new Float2(-0.7197418f, 0.6942418f);
        numArray3[0x33] = new Float2(-0.8143371f, -0.5803922f);
        numArray3[0x34] = new Float2(0.1045211f, -0.9945227f);
        numArray3[0x35] = new Float2(-0.1065926f, -0.9943027f);
        numArray3[0x36] = new Float2(0.4457997f, -0.8951328f);
        numArray3[0x37] = new Float2(0.1055474f, 0.9944143f);
        numArray3[0x38] = new Float2(-0.9927903f, 0.1198644f);
        numArray3[0x39] = new Float2(-0.8334367f, 0.552615f);
        numArray3[0x3a] = new Float2(0.9115562f, -0.4111756f);
        numArray3[0x3b] = new Float2(0.8285545f, -0.5599084f);
        numArray3[60] = new Float2(0.7217098f, -0.6921958f);
        numArray3[0x3d] = new Float2(0.4940493f, -0.8694339f);
        numArray3[0x3e] = new Float2(-0.3652321f, -0.9309165f);
        numArray3[0x3f] = new Float2(-0.9696607f, 0.2444548f);
        numArray3[0x40] = new Float2(0.08925509f, -0.9960088f);
        numArray3[0x41] = new Float2(0.5354071f, -0.8445941f);
        numArray3[0x42] = new Float2(-0.1053576f, 0.9944344f);
        numArray3[0x43] = new Float2(-0.9890285f, 0.1477251f);
        numArray3[0x44] = new Float2(0.004856105f, 0.9999882f);
        numArray3[0x45] = new Float2(0.9885598f, 0.1508291f);
        numArray3[70] = new Float2(0.9286129f, -0.3710498f);
        numArray3[0x47] = new Float2(-0.5832394f, -0.8123003f);
        numArray3[0x48] = new Float2(0.3015208f, 0.9534596f);
        numArray3[0x49] = new Float2(-0.9575111f, 0.2883966f);
        numArray3[0x4a] = new Float2(0.9715802f, -0.2367105f);
        numArray3[0x4b] = new Float2(0.2299818f, 0.973195f);
        numArray3[0x4c] = new Float2(0.9557638f, -0.2941352f);
        numArray3[0x4d] = new Float2(0.7409561f, 0.6715534f);
        numArray3[0x4e] = new Float2(-0.9971514f, -0.07542631f);
        numArray3[0x4f] = new Float2(0.6905711f, -0.7232645f);
        numArray3[80] = new Float2(-0.2907137f, -0.9568101f);
        numArray3[0x51] = new Float2(0.5912778f, -0.806468f);
        numArray3[0x52] = new Float2(-0.9454592f, -0.3257405f);
        numArray3[0x53] = new Float2(0.6664456f, 0.7455537f);
        numArray3[0x54] = new Float2(0.6236135f, 0.7817329f);
        numArray3[0x55] = new Float2(0.9126994f, -0.4086317f);
        numArray3[0x56] = new Float2(-0.8191762f, 0.5735419f);
        numArray3[0x57] = new Float2(-0.8812746f, -0.4726046f);
        numArray3[0x58] = new Float2(0.9953313f, 0.09651673f);
        numArray3[0x59] = new Float2(0.9855651f, -0.169297f);
        numArray3[90] = new Float2(-0.8495981f, 0.5274307f);
        numArray3[0x5b] = new Float2(0.6174854f, -0.7865824f);
        numArray3[0x5c] = new Float2(0.8508157f, 0.5254643f);
        numArray3[0x5d] = new Float2(0.9985033f, -0.0546925f);
        numArray3[0x5e] = new Float2(0.1971372f, -0.9803759f);
        numArray3[0x5f] = new Float2(0.6607856f, -0.7505747f);
        numArray3[0x60] = new Float2(-0.03097494f, 0.9995202f);
        numArray3[0x61] = new Float2(-0.6731661f, 0.7394913f);
        numArray3[0x62] = new Float2(-0.7195019f, -0.6944906f);
        numArray3[0x63] = new Float2(0.9727511f, 0.2318516f);
        numArray3[100] = new Float2(0.9997059f, -0.02425069f);
        numArray3[0x65] = new Float2(0.4421788f, -0.8969269f);
        numArray3[0x66] = new Float2(0.9981351f, -0.06104367f);
        numArray3[0x67] = new Float2(-0.9173661f, -0.3980446f);
        numArray3[0x68] = new Float2(-0.8150057f, -0.579453f);
        numArray3[0x69] = new Float2(-0.8789331f, 0.476945f);
        numArray3[0x6a] = new Float2(0.01586058f, 0.9998742f);
        numArray3[0x6b] = new Float2(-0.8095465f, 0.5870558f);
        numArray3[0x6c] = new Float2(-0.9165899f, -0.3998287f);
        numArray3[0x6d] = new Float2(-0.8023543f, 0.5968481f);
        numArray3[110] = new Float2(-0.5176738f, 0.8555781f);
        numArray3[0x6f] = new Float2(-0.8154407f, -0.5788406f);
        numArray3[0x70] = new Float2(0.402201f, -0.9155514f);
        numArray3[0x71] = new Float2(-0.9052557f, -0.4248672f);
        numArray3[0x72] = new Float2(0.7317446f, 0.681579f);
        numArray3[0x73] = new Float2(-0.5647632f, -0.825253f);
        numArray3[0x74] = new Float2(-0.8403276f, -0.5420789f);
        numArray3[0x75] = new Float2(-0.9314281f, 0.3639252f);
        numArray3[0x76] = new Float2(0.5238199f, 0.8518291f);
        numArray3[0x77] = new Float2(0.7432804f, -0.66898f);
        numArray3[120] = new Float2(-0.9853716f, -0.1704197f);
        numArray3[0x79] = new Float2(0.4601469f, 0.8878428f);
        numArray3[0x7a] = new Float2(0.8258554f, 0.5638819f);
        numArray3[0x7b] = new Float2(0.6182366f, 0.785992f);
        numArray3[0x7c] = new Float2(0.8331503f, -0.5530466f);
        numArray3[0x7d] = new Float2(0.1500307f, 0.9886813f);
        numArray3[0x7e] = new Float2(-0.6623304f, -0.7492119f);
        numArray3[0x7f] = new Float2(-0.6685987f, 0.7436234f);
        numArray3[0x80] = new Float2(0.7025606f, 0.7116239f);
        numArray3[0x81] = new Float2(-0.541939f, -0.8404179f);
        numArray3[130] = new Float2(-0.3388616f, 0.9408362f);
        numArray3[0x83] = new Float2(0.833153f, 0.5530425f);
        numArray3[0x84] = new Float2(-0.2989721f, -0.9542618f);
        numArray3[0x85] = new Float2(0.2638523f, 0.9645631f);
        numArray3[0x86] = new Float2(0.1241087f, -0.9922686f);
        numArray3[0x87] = new Float2(-0.7282649f, -0.6852957f);
        numArray3[0x88] = new Float2(0.69625f, 0.7177994f);
        numArray3[0x89] = new Float2(-0.9183536f, 0.395761f);
        numArray3[0x8a] = new Float2(-0.6326102f, -0.7744703f);
        numArray3[0x8b] = new Float2(-0.9331892f, -0.3593855f);
        numArray3[140] = new Float2(-0.1153779f, -0.9933217f);
        numArray3[0x8d] = new Float2(0.9514975f, -0.3076566f);
        numArray3[0x8e] = new Float2(-0.08987977f, -0.9959526f);
        numArray3[0x8f] = new Float2(0.6678497f, 0.7442962f);
        numArray3[0x90] = new Float2(0.79524f, -0.6062947f);
        numArray3[0x91] = new Float2(-0.6462007f, -0.7631675f);
        numArray3[0x92] = new Float2(-0.2733599f, 0.9619119f);
        numArray3[0x93] = new Float2(0.966959f, -0.2549318f);
        numArray3[0x94] = new Float2(-0.9792895f, 0.2024652f);
        numArray3[0x95] = new Float2(-0.5369503f, -0.8436139f);
        numArray3[150] = new Float2(-0.2700365f, -0.9628501f);
        numArray3[0x97] = new Float2(-0.6400277f, 0.7683519f);
        numArray3[0x98] = new Float2(-0.7854537f, -0.6189204f);
        numArray3[0x99] = new Float2(0.06005906f, -0.9981948f);
        numArray3[0x9a] = new Float2(-0.0245577f, 0.9996984f);
        numArray3[0x9b] = new Float2(-0.6598362f, 0.7514095f);
        numArray3[0x9c] = new Float2(-0.6253895f, -0.7803128f);
        numArray3[0x9d] = new Float2(-0.6210409f, -0.7837782f);
        numArray3[0x9e] = new Float2(0.8348889f, 0.5504186f);
        numArray3[0x9f] = new Float2(-0.1592275f, 0.9872419f);
        numArray3[160] = new Float2(0.8367622f, 0.5475664f);
        numArray3[0xa1] = new Float2(-0.8675754f, -0.4973057f);
        numArray3[0xa2] = new Float2(-0.2022663f, -0.9793305f);
        numArray3[0xa3] = new Float2(0.939919f, 0.3413976f);
        numArray3[0xa4] = new Float2(0.9877405f, -0.1561049f);
        numArray3[0xa5] = new Float2(-0.9034455f, 0.4287028f);
        numArray3[0xa6] = new Float2(0.1269804f, -0.9919052f);
        numArray3[0xa7] = new Float2(-0.3819601f, 0.9241788f);
        numArray3[0xa8] = new Float2(0.9754626f, 0.2201653f);
        numArray3[0xa9] = new Float2(-0.3204016f, -0.9472818f);
        numArray3[170] = new Float2(-0.9874761f, 0.1577687f);
        numArray3[0xab] = new Float2(0.02535348f, -0.9996786f);
        numArray3[0xac] = new Float2(0.4835131f, -0.8753371f);
        numArray3[0xad] = new Float2(-0.28508f, -0.9585037f);
        numArray3[0xae] = new Float2(-0.06805516f, -0.9976816f);
        numArray3[0xaf] = new Float2(-0.7885244f, -0.6150035f);
        numArray3[0xb0] = new Float2(0.3185392f, -0.9479097f);
        numArray3[0xb1] = new Float2(0.8880043f, 0.4598351f);
        numArray3[0xb2] = new Float2(0.6476921f, -0.7619022f);
        numArray3[0xb3] = new Float2(0.9820241f, 0.1887554f);
        numArray3[180] = new Float2(0.9357275f, -0.3527237f);
        numArray3[0xb5] = new Float2(-0.8894895f, 0.4569555f);
        numArray3[0xb6] = new Float2(0.7922791f, 0.6101588f);
        numArray3[0xb7] = new Float2(0.7483819f, 0.6632681f);
        numArray3[0xb8] = new Float2(-0.728893f, -0.6846277f);
        numArray3[0xb9] = new Float2(0.8729033f, -0.4878933f);
        numArray3[0xba] = new Float2(0.8288346f, 0.5594937f);
        numArray3[0xbb] = new Float2(0.08074567f, 0.9967347f);
        numArray3[0xbc] = new Float2(0.9799148f, -0.1994165f);
        numArray3[0xbd] = new Float2(-0.5807307f, -0.8140957f);
        numArray3[190] = new Float2(-0.470005f, -0.8826638f);
        numArray3[0xbf] = new Float2(0.2409493f, 0.9705377f);
        numArray3[0xc0] = new Float2(0.9437817f, -0.3305694f);
        numArray3[0xc1] = new Float2(-0.8927999f, -0.4504535f);
        numArray3[0xc2] = new Float2(-0.8069623f, 0.5906031f);
        numArray3[0xc3] = new Float2(0.06258973f, 0.9980394f);
        numArray3[0xc4] = new Float2(-0.9312598f, 0.364356f);
        numArray3[0xc5] = new Float2(0.577745f, 0.8162174f);
        numArray3[0xc6] = new Float2(-0.3360096f, -0.9418586f);
        numArray3[0xc7] = new Float2(0.6979321f, -0.7161639f);
        numArray3[200] = new Float2(-0.002008157f, -0.999998f);
        numArray3[0xc9] = new Float2(-0.1827294f, -0.9831632f);
        numArray3[0xca] = new Float2(-0.6523912f, 0.7578824f);
        numArray3[0xcb] = new Float2(-0.4302627f, -0.9027037f);
        numArray3[0xcc] = new Float2(-0.9985126f, -0.05452091f);
        numArray3[0xcd] = new Float2(-0.01028102f, -0.9999471f);
        numArray3[0xce] = new Float2(-0.4946071f, 0.8691167f);
        numArray3[0xcf] = new Float2(-0.299935f, 0.9539596f);
        numArray3[0xd0] = new Float2(0.8165472f, 0.5772787f);
        numArray3[0xd1] = new Float2(0.269746f, 0.9629315f);
        numArray3[210] = new Float2(-0.7306287f, -0.682775f);
        numArray3[0xd3] = new Float2(-0.7590952f, -0.6509796f);
        numArray3[0xd4] = new Float2(-0.9070538f, 0.4210146f);
        numArray3[0xd5] = new Float2(-0.5104861f, -0.859886f);
        numArray3[0xd6] = new Float2(0.861335f, 0.5080373f);
        numArray3[0xd7] = new Float2(0.5007882f, -0.8655699f);
        numArray3[0xd8] = new Float2(-0.6541582f, 0.7563578f);
        numArray3[0xd9] = new Float2(-0.8382756f, -0.5452468f);
        numArray3[0xda] = new Float2(0.6940071f, 0.7199682f);
        numArray3[0xdb] = new Float2(0.06950936f, 0.9975813f);
        numArray3[220] = new Float2(0.1702942f, -0.9853933f);
        numArray3[0xdd] = new Float2(0.2695973f, 0.9629731f);
        numArray3[0xde] = new Float2(0.5519612f, -0.8338698f);
        numArray3[0xdf] = new Float2(0.2256575f, -0.9742067f);
        numArray3[0xe0] = new Float2(0.4215263f, -0.9068162f);
        numArray3[0xe1] = new Float2(0.4881873f, -0.8727388f);
        numArray3[0xe2] = new Float2(-0.3683855f, -0.9296731f);
        numArray3[0xe3] = new Float2(-0.9825391f, 0.1860565f);
        numArray3[0xe4] = new Float2(0.8125647f, 0.582871f);
        numArray3[0xe5] = new Float2(0.3196461f, -0.947537f);
        numArray3[230] = new Float2(0.9570914f, 0.2897862f);
        numArray3[0xe7] = new Float2(-0.6876655f, -0.7260276f);
        numArray3[0xe8] = new Float2(-0.9988771f, -0.04737673f);
        numArray3[0xe9] = new Float2(-0.1250179f, 0.9921545f);
        numArray3[0xea] = new Float2(-0.8280134f, 0.5607083f);
        numArray3[0xeb] = new Float2(0.9324864f, -0.3612051f);
        numArray3[0xec] = new Float2(0.6394653f, 0.7688199f);
        numArray3[0xed] = new Float2(-0.01623847f, -0.9998682f);
        numArray3[0xee] = new Float2(-0.9955015f, -0.09474614f);
        numArray3[0xef] = new Float2(-0.8145332f, 0.580117f);
        numArray3[240] = new Float2(0.4037328f, -0.9148769f);
        numArray3[0xf1] = new Float2(0.9944263f, 0.1054337f);
        numArray3[0xf2] = new Float2(-0.1624712f, 0.9867133f);
        numArray3[0xf3] = new Float2(-0.9949488f, -0.1003839f);
        numArray3[0xf4] = new Float2(-0.6995302f, 0.714603f);
        numArray3[0xf5] = new Float2(0.5263415f, -0.8502733f);
        numArray3[0xf6] = new Float2(-0.5395222f, 0.8419714f);
        numArray3[0xf7] = new Float2(0.657937f, 0.7530729f);
        numArray3[0xf8] = new Float2(0.01426759f, -0.9998982f);
        numArray3[0xf9] = new Float2(-0.6734384f, 0.7392433f);
        numArray3[250] = new Float2(0.6394121f, -0.7688642f);
        numArray3[0xfb] = new Float2(0.9211571f, 0.3891909f);
        numArray3[0xfc] = new Float2(-0.1466372f, -0.9891903f);
        numArray3[0xfd] = new Float2(-0.7823181f, 0.6228791f);
        numArray3[0xfe] = new Float2(-0.5039611f, -0.8637264f);
        numArray3[0xff] = new Float2(-0.774312f, -0.632804f);
        CELL_2D = numArray3;
        Float3[] numArray4 = new Float3[0x100];
        numArray4[0] = new Float3(-0.7292737f, -0.661844f, 0.1735582f);
        numArray4[1] = new Float3(0.7902921f, -0.5480887f, -0.2739291f);
        numArray4[2] = new Float3(0.7217579f, 0.6226212f, -0.3023381f);
        numArray4[3] = new Float3(0.5656831f, -0.8208298f, -0.07900003f);
        numArray4[4] = new Float3(0.760049f, -0.555598f, -0.3371f);
        numArray4[5] = new Float3(0.3713946f, 0.5011265f, 0.7816254f);
        numArray4[6] = new Float3(-0.1277062f, -0.4254439f, -0.8959289f);
        numArray4[7] = new Float3(-0.2881561f, -0.5815839f, 0.7607406f);
        numArray4[8] = new Float3(0.5849561f, -0.6628202f, -0.4674352f);
        numArray4[9] = new Float3(0.3307171f, 0.03916537f, 0.9429169f);
        numArray4[10] = new Float3(0.8712122f, -0.4113374f, -0.2679382f);
        numArray4[11] = new Float3(0.580981f, 0.7021916f, 0.4115678f);
        numArray4[12] = new Float3(0.5037569f, 0.6330057f, -0.5878204f);
        numArray4[13] = new Float3(0.4493712f, 0.6013902f, 0.6606023f);
        numArray4[14] = new Float3(-0.6878404f, 0.09018891f, -0.7202372f);
        numArray4[15] = new Float3(-0.5958956f, -0.646935f, 0.4757977f);
        numArray4[0x10] = new Float3(-0.5127052f, 0.1946922f, -0.8361987f);
        numArray4[0x11] = new Float3(-0.9911507f, -0.05410276f, -0.1212153f);
        numArray4[0x12] = new Float3(-0.2149721f, 0.9720882f, -0.09397608f);
        numArray4[0x13] = new Float3(-0.7518651f, -0.5428057f, 0.374247f);
        numArray4[20] = new Float3(0.5237069f, 0.8516377f, -0.02107818f);
        numArray4[0x15] = new Float3(0.6333505f, 0.1926167f, -0.7495105f);
        numArray4[0x16] = new Float3(-0.06788242f, 0.3998306f, 0.9140719f);
        numArray4[0x17] = new Float3(-0.5538629f, -0.4729897f, -0.6852129f);
        numArray4[0x18] = new Float3(-0.7261456f, -0.5911991f, 0.3509933f);
        numArray4[0x19] = new Float3(-0.9229275f, -0.1782809f, 0.3412049f);
        numArray4[0x1a] = new Float3(-0.6968815f, 0.6511275f, 0.300648f);
        numArray4[0x1b] = new Float3(0.9608045f, -0.2098363f, -0.1811725f);
        numArray4[0x1c] = new Float3(0.06817146f, -0.9743405f, 0.2145069f);
        numArray4[0x1d] = new Float3(-0.3577285f, -0.6697087f, -0.6507846f);
        numArray4[30] = new Float3(-0.1868621f, 0.7648617f, -0.6164975f);
        numArray4[0x1f] = new Float3(-0.6541697f, 0.3967915f, 0.6439087f);
        numArray4[0x20] = new Float3(0.699334f, -0.6164538f, 0.3618239f);
        numArray4[0x21] = new Float3(-0.1546666f, 0.6291284f, 0.7617583f);
        numArray4[0x22] = new Float3(-0.6841613f, -0.2580482f, -0.6821542f);
        numArray4[0x23] = new Float3(0.5383981f, 0.4258655f, 0.727163f);
        numArray4[0x24] = new Float3(-0.5026988f, -0.7939833f, -0.3418837f);
        numArray4[0x25] = new Float3(0.3202972f, 0.2834415f, 0.9039196f);
        numArray4[0x26] = new Float3(0.8683227f, -0.0003762656f, -0.4959995f);
        numArray4[0x27] = new Float3(0.7911201f, -0.08511046f, 0.6057106f);
        numArray4[40] = new Float3(-0.04011016f, -0.4397249f, 0.8972364f);
        numArray4[0x29] = new Float3(0.914512f, 0.3579346f, -0.1885488f);
        numArray4[0x2a] = new Float3(-0.9612039f, -0.2756484f, 0.01024667f);
        numArray4[0x2b] = new Float3(0.6510361f, -0.2877799f, -0.7023779f);
        numArray4[0x2c] = new Float3(-0.2041786f, 0.7365237f, 0.6448596f);
        numArray4[0x2d] = new Float3(-0.7718264f, 0.3790627f, 0.5104856f);
        numArray4[0x2e] = new Float3(-0.3060083f, -0.7692988f, 0.5608371f);
        numArray4[0x2f] = new Float3(0.4540073f, -0.5024843f, 0.73579f);
        numArray4[0x30] = new Float3(0.4816796f, 0.6021208f, -0.636738f);
        numArray4[0x31] = new Float3(0.696198f, -0.3222197f, 0.6414692f);
        numArray4[50] = new Float3(-0.6532161f, -0.6781149f, 0.3368516f);
        numArray4[0x33] = new Float3(0.5089301f, -0.6154662f, -0.6018234f);
        numArray4[0x34] = new Float3(-0.163592f, -0.9133605f, -0.3728409f);
        numArray4[0x35] = new Float3(0.5240802f, -0.8437664f, 0.1157506f);
        numArray4[0x36] = new Float3(0.5902587f, 0.4983818f, -0.6349884f);
        numArray4[0x37] = new Float3(0.5863228f, 0.4947647f, 0.6414308f);
        numArray4[0x38] = new Float3(0.6779335f, 0.2341345f, 0.6968409f);
        numArray4[0x39] = new Float3(0.7177054f, -0.6858979f, 0.1201786f);
        numArray4[0x3a] = new Float3(-0.532882f, -0.5205125f, 0.6671608f);
        numArray4[0x3b] = new Float3(-0.8654874f, -0.07007271f, -0.4960054f);
        numArray4[60] = new Float3(-0.286181f, 0.7952089f, 0.5345495f);
        numArray4[0x3d] = new Float3(-0.0484953f, 0.9810836f, -0.1874116f);
        numArray4[0x3e] = new Float3(-0.6358522f, 0.6058348f, 0.47818f);
        numArray4[0x3f] = new Float3(0.6254795f, -0.286162f, 0.7258697f);
        numArray4[0x40] = new Float3(-0.258526f, 0.5061949f, -0.8227582f);
        numArray4[0x41] = new Float3(0.02136307f, 0.5064017f, -0.862033f);
        numArray4[0x42] = new Float3(0.2001118f, 0.8599263f, 0.4695551f);
        numArray4[0x43] = new Float3(0.4743561f, 0.6014985f, -0.6427953f);
        numArray4[0x44] = new Float3(0.6622994f, -0.5202475f, -0.539168f);
        numArray4[0x45] = new Float3(0.08084973f, -0.653272f, 0.7527941f);
        numArray4[70] = new Float3(-0.6893687f, 0.05928604f, 0.7219805f);
        numArray4[0x47] = new Float3(-0.1121887f, -0.9673185f, 0.2273953f);
        numArray4[0x48] = new Float3(0.7344116f, 0.5979668f, -0.3210533f);
        numArray4[0x49] = new Float3(0.5789393f, -0.248885f, 0.776457f);
        numArray4[0x4a] = new Float3(0.6988183f, 0.355717f, -0.6205791f);
        numArray4[0x4b] = new Float3(-0.8636845f, -0.2748771f, -0.4224826f);
        numArray4[0x4c] = new Float3(-0.4247028f, -0.4640881f, 0.777335f);
        numArray4[0x4d] = new Float3(0.5257723f, -0.8427017f, 0.115833f);
        numArray4[0x4e] = new Float3(0.934383f, 0.3163025f, -0.1639544f);
        numArray4[0x4f] = new Float3(-0.1016836f, -0.8057303f, -0.5834888f);
        numArray4[80] = new Float3(-0.6529239f, 0.5060213f, -0.5635893f);
        numArray4[0x51] = new Float3(-0.2465286f, -0.9668206f, -0.06694497f);
        numArray4[0x52] = new Float3(-0.9776897f, -0.2099251f, -0.007368825f);
        numArray4[0x53] = new Float3(0.7736893f, 0.5734245f, 0.2694238f);
        numArray4[0x54] = new Float3(-0.6095088f, 0.4995679f, 0.6155737f);
        numArray4[0x55] = new Float3(0.5794535f, 0.7434547f, 0.3339292f);
        numArray4[0x56] = new Float3(-0.8226211f, 0.08142582f, 0.5627294f);
        numArray4[0x57] = new Float3(-0.5103855f, 0.4703668f, 0.719904f);
        numArray4[0x58] = new Float3(-0.5764972f, -0.07231656f, -0.8138927f);
        numArray4[0x59] = new Float3(0.7250629f, 0.3949971f, -0.5641463f);
        numArray4[90] = new Float3(-0.1525424f, 0.4860841f, -0.8604958f);
        numArray4[0x5b] = new Float3(-0.5550976f, -0.4957821f, 0.6678823f);
        numArray4[0x5c] = new Float3(-0.1883614f, 0.914587f, 0.3578417f);
        numArray4[0x5d] = new Float3(0.7625557f, -0.5414408f, -0.354049f);
        numArray4[0x5e] = new Float3(-0.5870232f, -0.3226498f, -0.7424964f);
        numArray4[0x5f] = new Float3(0.3051124f, 0.2262544f, -0.9250488f);
        numArray4[0x60] = new Float3(0.6379576f, 0.5772424f, -0.509707f);
        numArray4[0x61] = new Float3(-0.5966776f, 0.1454852f, -0.7891831f);
        numArray4[0x62] = new Float3(-0.6583306f, 0.6555488f, -0.3699415f);
        numArray4[0x63] = new Float3(0.7434893f, 0.2351085f, 0.6260573f);
        numArray4[100] = new Float3(0.5562114f, 0.826436f, -0.08736329f);
        numArray4[0x65] = new Float3(-0.302894f, -0.8251527f, 0.4768419f);
        numArray4[0x66] = new Float3(0.1129344f, -0.9858884f, -0.1235711f);
        numArray4[0x67] = new Float3(0.5937653f, -0.5896814f, 0.5474657f);
        numArray4[0x68] = new Float3(0.6757964f, -0.5835758f, -0.4502648f);
        numArray4[0x69] = new Float3(0.7242303f, -0.115272f, 0.679855f);
        numArray4[0x6a] = new Float3(-0.9511914f, 0.0753624f, -0.2992581f);
        numArray4[0x6b] = new Float3(0.2539471f, -0.1886339f, 0.9486454f);
        numArray4[0x6c] = new Float3(0.5714336f, -0.1679451f, -0.8032796f);
        numArray4[0x6d] = new Float3(-0.06778235f, 0.3978269f, 0.9149532f);
        numArray4[110] = new Float3(0.6074973f, 0.73306f, -0.3058923f);
        numArray4[0x6f] = new Float3(-0.5435479f, 0.1675822f, 0.8224791f);
        numArray4[0x70] = new Float3(-0.5876678f, -0.3380045f, -0.7351187f);
        numArray4[0x71] = new Float3(-0.7967563f, 0.04097823f, -0.6029099f);
        numArray4[0x72] = new Float3(-0.1996351f, 0.8706295f, 0.4496111f);
        numArray4[0x73] = new Float3(-0.0278766f, -0.9106233f, -0.4122962f);
        numArray4[0x74] = new Float3(-0.7797626f, -0.6257635f, 0.01975776f);
        numArray4[0x75] = new Float3(-0.5211233f, 0.7401645f, -0.4249555f);
        numArray4[0x76] = new Float3(0.8575425f, 0.4053273f, -0.3167502f);
        numArray4[0x77] = new Float3(0.1045223f, 0.8390196f, -0.5339674f);
        numArray4[120] = new Float3(0.3501823f, 0.9242524f, -0.152085f);
        numArray4[0x79] = new Float3(0.198785f, 0.07647613f, 0.9770547f);
        numArray4[0x7a] = new Float3(0.7845997f, 0.6066257f, -0.1280964f);
        numArray4[0x7b] = new Float3(0.09006737f, -0.975099f, -0.2026569f);
        numArray4[0x7c] = new Float3(-0.8274344f, -0.5422996f, 0.1458204f);
        numArray4[0x7d] = new Float3(-0.3485798f, -0.4158023f, 0.8400004f);
        numArray4[0x7e] = new Float3(-0.2471779f, -0.730482f, -0.6366311f);
        numArray4[0x7f] = new Float3(-0.3700155f, 0.8577948f, 0.3567584f);
        numArray4[0x80] = new Float3(0.5913395f, -0.5483119f, -0.5913303f);
        numArray4[0x81] = new Float3(0.1204874f, -0.7626472f, -0.6354935f);
        numArray4[130] = new Float3(0.6169593f, 0.03079648f, 0.7863923f);
        numArray4[0x83] = new Float3(0.1258157f, -0.664083f, -0.7369968f);
        numArray4[0x84] = new Float3(-0.6477565f, -0.1740147f, -0.7417077f);
        numArray4[0x85] = new Float3(0.6217889f, -0.7804431f, -0.06547655f);
        numArray4[0x86] = new Float3(0.6589943f, -0.6096988f, 0.4404474f);
        numArray4[0x87] = new Float3(-0.2689838f, -0.6732403f, -0.6887636f);
        numArray4[0x88] = new Float3(-0.3849775f, 0.5676543f, 0.7277094f);
        numArray4[0x89] = new Float3(0.5754445f, 0.8110471f, -0.1051963f);
        numArray4[0x8a] = new Float3(0.9141594f, 0.3832948f, 0.1319006f);
        numArray4[0x8b] = new Float3(-0.1079253f, 0.9245494f, 0.3654594f);
        numArray4[140] = new Float3(0.3779771f, 0.3043149f, 0.8743716f);
        numArray4[0x8d] = new Float3(-0.2142885f, -0.8259286f, 0.5214617f);
        numArray4[0x8e] = new Float3(0.5802544f, 0.4148099f, -0.7008834f);
        numArray4[0x8f] = new Float3(-0.1982661f, 0.8567162f, -0.4761597f);
        numArray4[0x90] = new Float3(-0.03381554f, 0.3773181f, -0.9254661f);
        numArray4[0x91] = new Float3(-0.6867923f, -0.6656598f, 0.2919134f);
        numArray4[0x92] = new Float3(0.7731743f, -0.2875794f, -0.565243f);
        numArray4[0x93] = new Float3(-0.09655942f, 0.9193708f, -0.3813575f);
        numArray4[0x94] = new Float3(0.2715702f, -0.957791f, -0.09426606f);
        numArray4[0x95] = new Float3(0.2451016f, -0.6917999f, -0.6792188f);
        numArray4[150] = new Float3(0.9777008f, -0.1753855f, 0.1155037f);
        numArray4[0x97] = new Float3(-0.522474f, 0.8521607f, 0.02903616f);
        numArray4[0x98] = new Float3(-0.773488f, -0.5261292f, 0.353418f);
        numArray4[0x99] = new Float3(-0.7134492f, -0.2695473f, 0.6467878f);
        numArray4[0x9a] = new Float3(0.1644037f, 0.5105846f, -0.8439637f);
        numArray4[0x9b] = new Float3(0.6494636f, 0.05585611f, 0.7583384f);
        numArray4[0x9c] = new Float3(-0.4711971f, 0.5017281f, -0.7254256f);
        numArray4[0x9d] = new Float3(-0.6335765f, -0.2381686f, -0.7361091f);
        numArray4[0x9e] = new Float3(-0.9021533f, -0.2709478f, -0.3357182f);
        numArray4[0x9f] = new Float3(-0.3793711f, 0.8722581f, 0.3086152f);
        numArray4[160] = new Float3(-0.6855599f, -0.3250143f, 0.6514394f);
        numArray4[0xa1] = new Float3(0.2900942f, -0.7799058f, -0.5546101f);
        numArray4[0xa2] = new Float3(-0.2098319f, 0.8503707f, 0.4825352f);
        numArray4[0xa3] = new Float3(-0.4592604f, 0.6598504f, -0.5947077f);
        numArray4[0xa4] = new Float3(0.8715945f, 0.09616365f, -0.4807031f);
        numArray4[0xa5] = new Float3(-0.6776666f, 0.7118505f, -0.1844907f);
        numArray4[0xa6] = new Float3(0.7044378f, 0.3124276f, 0.637304f);
        numArray4[0xa7] = new Float3(-0.7052319f, -0.2401093f, -0.6670798f);
        numArray4[0xa8] = new Float3(0.081921f, -0.7207336f, -0.6883546f);
        numArray4[0xa9] = new Float3(-0.6993681f, -0.5875763f, -0.4069869f);
        numArray4[170] = new Float3(-0.1281454f, 0.6419896f, 0.7559286f);
        numArray4[0xab] = new Float3(-0.6337388f, -0.6785471f, -0.3714147f);
        numArray4[0xac] = new Float3(0.5565052f, -0.2168888f, -0.8020357f);
        numArray4[0xad] = new Float3(-0.5791554f, 0.7244372f, -0.3738579f);
        numArray4[0xae] = new Float3(0.1175779f, -0.7096451f, 0.6946793f);
        numArray4[0xaf] = new Float3(-0.613462f, 0.1323631f, 0.7785528f);
        numArray4[0xb0] = new Float3(0.6984636f, -0.02980516f, -0.7150247f);
        numArray4[0xb1] = new Float3(0.8318083f, -0.3930172f, 0.3919598f);
        numArray4[0xb2] = new Float3(0.1469576f, 0.05541652f, -0.9875892f);
        numArray4[0xb3] = new Float3(0.7088686f, -0.2690504f, 0.6520101f);
        numArray4[180] = new Float3(0.2726053f, 0.6736977f, -0.6868899f);
        numArray4[0xb5] = new Float3(-0.6591296f, 0.3035459f, -0.6880466f);
        numArray4[0xb6] = new Float3(0.4815131f, -0.752827f, 0.4487723f);
        numArray4[0xb7] = new Float3(0.943001f, 0.1675647f, -0.2875261f);
        numArray4[0xb8] = new Float3(0.4348029f, 0.7695305f, -0.4677278f);
        numArray4[0xb9] = new Float3(0.3931996f, 0.5944736f, 0.7014236f);
        numArray4[0xba] = new Float3(0.7254336f, -0.6039256f, 0.3301815f);
        numArray4[0xbb] = new Float3(0.7590235f, -0.6506083f, 0.02433313f);
        numArray4[0xbc] = new Float3(-0.8552769f, -0.3430043f, 0.3883936f);
        numArray4[0xbd] = new Float3(-0.6139747f, 0.6981725f, 0.3682258f);
        numArray4[190] = new Float3(-0.7465906f, -0.575201f, 0.3342849f);
        numArray4[0xbf] = new Float3(0.5730066f, 0.8105555f, -0.1210917f);
        numArray4[0xc0] = new Float3(-0.9225878f, -0.3475211f, -0.167514f);
        numArray4[0xc1] = new Float3(-0.7105817f, -0.4719692f, -0.5218417f);
        numArray4[0xc2] = new Float3(-0.0856461f, 0.3583001f, 0.9296697f);
        numArray4[0xc3] = new Float3(-0.8279698f, -0.2043157f, 0.5222271f);
        numArray4[0xc4] = new Float3(0.427944f, 0.278166f, 0.8599346f);
        numArray4[0xc5] = new Float3(0.539908f, -0.7857121f, -0.3019204f);
        numArray4[0xc6] = new Float3(0.5678404f, -0.5495414f, -0.6128308f);
        numArray4[0xc7] = new Float3(-0.9896071f, 0.1365639f, -0.04503419f);
        numArray4[200] = new Float3(-0.6154343f, -0.6440876f, 0.4543037f);
        numArray4[0xc9] = new Float3(0.1074204f, -0.794634f, 0.5975094f);
        numArray4[0xca] = new Float3(-0.359545f, -0.888553f, 0.2849578f);
        numArray4[0xcb] = new Float3(-0.2180405f, 0.1529889f, 0.9638738f);
        numArray4[0xcc] = new Float3(-0.7277432f, -0.6164051f, -0.3007235f);
        numArray4[0xcd] = new Float3(0.7249729f, -0.006697195f, 0.6887448f);
        numArray4[0xce] = new Float3(-0.5553659f, -0.5336586f, 0.6377908f);
        numArray4[0xcf] = new Float3(0.5137558f, 0.7976208f, -0.316f);
        numArray4[0xd0] = new Float3(-0.3794025f, 0.9245608f, -0.03522751f);
        numArray4[0xd1] = new Float3(0.8229249f, 0.2745366f, -0.4974177f);
        numArray4[210] = new Float3(-0.5404114f, 0.6091142f, 0.5804614f);
        numArray4[0xd3] = new Float3(0.8036582f, -0.270303f, 0.5301602f);
        numArray4[0xd4] = new Float3(0.6044319f, 0.6832969f, 0.4095943f);
        numArray4[0xd5] = new Float3(0.06389989f, 0.9658208f, -0.2512108f);
        numArray4[0xd6] = new Float3(0.1087113f, 0.7402471f, -0.6634878f);
        numArray4[0xd7] = new Float3(-0.7134277f, -0.6926784f, 0.1059128f);
        numArray4[0xd8] = new Float3(0.6458898f, -0.5724549f, -0.5050958f);
        numArray4[0xd9] = new Float3(-0.6553931f, 0.7381471f, 0.1599956f);
        numArray4[0xda] = new Float3(0.3910961f, 0.9188871f, -0.05186756f);
        numArray4[0xdb] = new Float3(-0.4879023f, -0.5904377f, 0.6429111f);
        numArray4[220] = new Float3(0.601479f, 0.7707441f, -0.210182f);
        numArray4[0xdd] = new Float3(-0.5677173f, 0.7511361f, 0.3368852f);
        numArray4[0xde] = new Float3(0.7858574f, 0.2266747f, 0.5753667f);
        numArray4[0xdf] = new Float3(-0.4520346f, -0.6042227f, -0.6561857f);
        numArray4[0xe0] = new Float3(0.002272116f, 0.4132844f, -0.9105992f);
        numArray4[0xe1] = new Float3(-0.5815752f, -0.5162926f, 0.6286591f);
        numArray4[0xe2] = new Float3(-0.03703705f, 0.8273786f, 0.5604221f);
        numArray4[0xe3] = new Float3(-0.5119693f, 0.7953544f, -0.324498f);
        numArray4[0xe4] = new Float3(-0.2682417f, -0.957229f, -0.1084388f);
        numArray4[0xe5] = new Float3(-0.2322483f, -0.9679131f, -0.09594243f);
        numArray4[230] = new Float3(0.3554329f, -0.8881506f, 0.2913006f);
        numArray4[0xe7] = new Float3(0.734652f, -0.4371373f, 0.5188423f);
        numArray4[0xe8] = new Float3(0.998512f, 0.04659011f, -0.02833945f);
        numArray4[0xe9] = new Float3(-0.3727688f, -0.9082481f, 0.1900757f);
        numArray4[0xea] = new Float3(0.9173738f, -0.3483642f, 0.1925298f);
        numArray4[0xeb] = new Float3(0.2714911f, 0.414753f, -0.8684887f);
        numArray4[0xec] = new Float3(0.5131763f, -0.7116334f, 0.4798207f);
        numArray4[0xed] = new Float3(-0.8737354f, 0.1888699f, -0.4482351f);
        numArray4[0xee] = new Float3(0.8460044f, -0.3725218f, 0.38145f);
        numArray4[0xef] = new Float3(0.8978727f, -0.1780209f, -0.4026575f);
        numArray4[240] = new Float3(0.2178066f, -0.9698323f, -0.109479f);
        numArray4[0xf1] = new Float3(-0.1518031f, -0.7788918f, -0.6085091f);
        numArray4[0xf2] = new Float3(-0.2600385f, -0.4755398f, -0.840382f);
        numArray4[0xf3] = new Float3(0.5723135f, -0.7474341f, -0.3373418f);
        numArray4[0xf4] = new Float3(-0.7174141f, 0.1699017f, -0.6756111f);
        numArray4[0xf5] = new Float3(-0.6841808f, 0.02145708f, -0.7289968f);
        numArray4[0xf6] = new Float3(-0.2007448f, 0.06555606f, -0.9774477f);
        numArray4[0xf7] = new Float3(-0.1148804f, -0.8044887f, 0.5827524f);
        numArray4[0xf8] = new Float3(-0.787035f, 0.03447489f, 0.6159443f);
        numArray4[0xf9] = new Float3(-0.2015596f, 0.6859872f, 0.6991389f);
        numArray4[250] = new Float3(-0.08581083f, -0.1092084f, -0.990308f);
        numArray4[0xfb] = new Float3(0.5532693f, 0.7325251f, -0.3966108f);
        numArray4[0xfc] = new Float3(-0.1842489f, -0.9777375f, -0.1004077f);
        numArray4[0xfd] = new Float3(0.07754738f, -0.9111506f, 0.404711f);
        numArray4[0xfe] = new Float3(0.1399838f, 0.7601631f, -0.6344734f);
        numArray4[0xff] = new Float3(0.4484419f, -0.8452892f, 0.2904925f);
        CELL_3D = numArray4;
        SIMPLEX_4D = new byte[] {
            0, 1, 2, 3, 0, 1, 3, 2, 0, 0, 0, 0, 0, 2, 3, 1,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 0,
            0, 2, 1, 3, 0, 0, 0, 0, 0, 3, 1, 2, 0, 3, 2, 1,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 0, 3, 0, 0, 0, 0, 1, 3, 0, 2, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 0, 1, 2, 3, 1, 0,
            1, 0, 2, 3, 1, 0, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 2, 0, 3, 1, 0, 0, 0, 0, 2, 1, 3, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            2, 0, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            3, 0, 1, 2, 3, 0, 2, 1, 0, 0, 0, 0, 3, 1, 2, 0,
            2, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            3, 1, 0, 2, 0, 0, 0, 0, 3, 2, 0, 1, 3, 2, 1, 0
        };
    }

    public FastNoise(int seed = 0x539)
    {
        this.m_seed = seed;
        this.CalculateFractalBounding();
    }

    private void CalculateFractalBounding()
    {
        float gain = this.m_gain;
        float num2 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            num2 += gain;
            gain *= this.m_gain;
        }
        this.m_fractalBounding = 1f / num2;
    }

    [MethodImpl(0x100)]
    private static float CubicLerp(float a, float b, float c, float d, float t)
    {
        float num = (d - c) - (a - b);
        return ((((((t * t) * t) * num) + ((t * t) * ((a - b) - num))) + (t * (c - a))) + b);
    }

    [MethodImpl(0x100)]
    private static int FastFloor(float f) =>
        ((f < 0f) ? (((int)f) - 1) : ((int)f));

    [MethodImpl(0x100)]
    private static int FastRound(float f) =>
        ((f < 0f) ? ((int)(f - 0.5f)) : ((int)(f + 0.5f)));

    [MethodImpl(0x100)]
    private int FloatCast2Int(float f)
    {
        long num = BitConverter.DoubleToInt64Bits((double)f);
        return (int)(num ^ (num >> 0x20));
    }

    public float GetCellular(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        switch (this.m_cellularReturnType)
        {
            case CellularReturnType.CellValue:
            case CellularReturnType.NoiseLookup:
            case CellularReturnType.Distance:
                return this.SingleCellular(x, y);
        }
        return this.SingleCellular2Edge(x, y);
    }

    public float GetCellular(float x, float y, float z)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        z *= this.m_frequency;
        switch (this.m_cellularReturnType)
        {
            case CellularReturnType.CellValue:
            case CellularReturnType.NoiseLookup:
            case CellularReturnType.Distance:
                return this.SingleCellular(x, y, z);
        }
        return this.SingleCellular2Edge(x, y, z);
    }

    public float GetCubic(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        return this.SingleCubic(0, x, y);
    }

    public float GetCubic(float x, float y, float z) =>
        this.SingleCubic(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);

    public float GetCubicFractal(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SingleCubicFractalFBM(x, y);

            case FractalType.Billow:
                return this.SingleCubicFractalBillow(x, y);

            case FractalType.RigidMulti:
                return this.SingleCubicFractalRigidMulti(x, y);
        }
        return 0f;
    }

    public float GetCubicFractal(float x, float y, float z)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        z *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SingleCubicFractalFBM(x, y, z);

            case FractalType.Billow:
                return this.SingleCubicFractalBillow(x, y, z);

            case FractalType.RigidMulti:
                return this.SingleCubicFractalRigidMulti(x, y, z);
        }
        return 0f;
    }

    public static float GetDecimalType() =>
        0f;

    public float GetNoise(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        switch (this.m_noiseType)
        {
            case NoiseType.Value:
                return this.SingleValue(this.m_seed, x, y);

            case NoiseType.ValueFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SingleValueFractalFBM(x, y);

                    case FractalType.Billow:
                        return this.SingleValueFractalBillow(x, y);

                    case FractalType.RigidMulti:
                        return this.SingleValueFractalRigidMulti(x, y);
                }
                return 0f;

            case NoiseType.Perlin:
                return this.SinglePerlin(this.m_seed, x, y);

            case NoiseType.PerlinFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SinglePerlinFractalFBM(x, y);

                    case FractalType.Billow:
                        return this.SinglePerlinFractalBillow(x, y);

                    case FractalType.RigidMulti:
                        return this.SinglePerlinFractalRigidMulti(x, y);
                }
                return 0f;

            case NoiseType.Simplex:
                return this.SingleSimplex(this.m_seed, x, y);

            case NoiseType.SimplexFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SingleSimplexFractalFBM(x, y);

                    case FractalType.Billow:
                        return this.SingleSimplexFractalBillow(x, y);

                    case FractalType.RigidMulti:
                        return this.SingleSimplexFractalRigidMulti(x, y);
                }
                return 0f;

            case NoiseType.Cellular:
                switch (this.m_cellularReturnType)
                {
                    case CellularReturnType.CellValue:
                    case CellularReturnType.NoiseLookup:
                    case CellularReturnType.Distance:
                        return this.SingleCellular(x, y);
                }
                return this.SingleCellular2Edge(x, y);

            case NoiseType.WhiteNoise:
                return this.GetWhiteNoise(x, y);

            case NoiseType.Cubic:
                return this.SingleCubic(this.m_seed, x, y);

            case NoiseType.CubicFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SingleCubicFractalFBM(x, y);

                    case FractalType.Billow:
                        return this.SingleCubicFractalBillow(x, y);

                    case FractalType.RigidMulti:
                        return this.SingleCubicFractalRigidMulti(x, y);
                }
                return 0f;
        }
        return 0f;
    }

    public float GetNoise(float x, float y, float z)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        z *= this.m_frequency;
        switch (this.m_noiseType)
        {
            case NoiseType.Value:
                return this.SingleValue(this.m_seed, x, y, z);

            case NoiseType.ValueFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SingleValueFractalFBM(x, y, z);

                    case FractalType.Billow:
                        return this.SingleValueFractalBillow(x, y, z);

                    case FractalType.RigidMulti:
                        return this.SingleValueFractalRigidMulti(x, y, z);
                }
                return 0f;

            case NoiseType.Perlin:
                return this.SinglePerlin(this.m_seed, x, y, z);

            case NoiseType.PerlinFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SinglePerlinFractalFBM(x, y, z);

                    case FractalType.Billow:
                        return this.SinglePerlinFractalBillow(x, y, z);

                    case FractalType.RigidMulti:
                        return this.SinglePerlinFractalRigidMulti(x, y, z);
                }
                return 0f;

            case NoiseType.Simplex:
                return this.SingleSimplex(this.m_seed, x, y, z);

            case NoiseType.SimplexFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SingleSimplexFractalFBM(x, y, z);

                    case FractalType.Billow:
                        return this.SingleSimplexFractalBillow(x, y, z);

                    case FractalType.RigidMulti:
                        return this.SingleSimplexFractalRigidMulti(x, y, z);
                }
                return 0f;

            case NoiseType.Cellular:
                switch (this.m_cellularReturnType)
                {
                    case CellularReturnType.CellValue:
                    case CellularReturnType.NoiseLookup:
                    case CellularReturnType.Distance:
                        return this.SingleCellular(x, y, z);
                }
                return this.SingleCellular2Edge(x, y, z);

            case NoiseType.WhiteNoise:
                return this.GetWhiteNoise(x, y, z);

            case NoiseType.Cubic:
                return this.SingleCubic(this.m_seed, x, y, z);

            case NoiseType.CubicFractal:
                switch (this.m_fractalType)
                {
                    case FractalType.FBM:
                        return this.SingleCubicFractalFBM(x, y, z);

                    case FractalType.Billow:
                        return this.SingleCubicFractalBillow(x, y, z);

                    case FractalType.RigidMulti:
                        return this.SingleCubicFractalRigidMulti(x, y, z);
                }
                return 0f;
        }
        return 0f;
    }

    public float GetPerlin(float x, float y) =>
        this.SinglePerlin(this.m_seed, x * this.m_frequency, y * this.m_frequency);

    public float GetPerlin(float x, float y, float z) =>
        this.SinglePerlin(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);

    public float GetPerlinFractal(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SinglePerlinFractalFBM(x, y);

            case FractalType.Billow:
                return this.SinglePerlinFractalBillow(x, y);

            case FractalType.RigidMulti:
                return this.SinglePerlinFractalRigidMulti(x, y);
        }
        return 0f;
    }

    public float GetPerlinFractal(float x, float y, float z)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        z *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SinglePerlinFractalFBM(x, y, z);

            case FractalType.Billow:
                return this.SinglePerlinFractalBillow(x, y, z);

            case FractalType.RigidMulti:
                return this.SinglePerlinFractalRigidMulti(x, y, z);
        }
        return 0f;
    }

    public int GetSeed() =>
        this.m_seed;

    public float GetSimplex(float x, float y) =>
        this.SingleSimplex(this.m_seed, x * this.m_frequency, y * this.m_frequency);

    public float GetSimplex(float x, float y, float z) =>
        this.SingleSimplex(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);

    public float GetSimplex(float x, float y, float z, float w) =>
        this.SingleSimplex(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency, w * this.m_frequency);

    public float GetSimplexFractal(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SingleSimplexFractalFBM(x, y);

            case FractalType.Billow:
                return this.SingleSimplexFractalBillow(x, y);

            case FractalType.RigidMulti:
                return this.SingleSimplexFractalRigidMulti(x, y);
        }
        return 0f;
    }

    public float GetSimplexFractal(float x, float y, float z)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        z *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SingleSimplexFractalFBM(x, y, z);

            case FractalType.Billow:
                return this.SingleSimplexFractalBillow(x, y, z);

            case FractalType.RigidMulti:
                return this.SingleSimplexFractalRigidMulti(x, y, z);
        }
        return 0f;
    }

    public float GetValue(float x, float y) =>
        this.SingleValue(this.m_seed, x * this.m_frequency, y * this.m_frequency);

    public float GetValue(float x, float y, float z) =>
        this.SingleValue(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);

    public float GetValueFractal(float x, float y)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SingleValueFractalFBM(x, y);

            case FractalType.Billow:
                return this.SingleValueFractalBillow(x, y);

            case FractalType.RigidMulti:
                return this.SingleValueFractalRigidMulti(x, y);
        }
        return 0f;
    }

    public float GetValueFractal(float x, float y, float z)
    {
        x *= this.m_frequency;
        y *= this.m_frequency;
        z *= this.m_frequency;
        switch (this.m_fractalType)
        {
            case FractalType.FBM:
                return this.SingleValueFractalFBM(x, y, z);

            case FractalType.Billow:
                return this.SingleValueFractalBillow(x, y, z);

            case FractalType.RigidMulti:
                return this.SingleValueFractalRigidMulti(x, y, z);
        }
        return 0f;
    }

    public float GetWhiteNoise(float x, float y)
    {
        int num = this.FloatCast2Int(x);
        return ValCoord2D(this.m_seed, num, this.FloatCast2Int(y));
    }

    public float GetWhiteNoise(float x, float y, float z)
    {
        int num = this.FloatCast2Int(x);
        return ValCoord3D(this.m_seed, num, this.FloatCast2Int(y), this.FloatCast2Int(z));
    }

    public float GetWhiteNoise(float x, float y, float z, float w)
    {
        int num = this.FloatCast2Int(x);
        return ValCoord4D(this.m_seed, num, this.FloatCast2Int(y), this.FloatCast2Int(z), this.FloatCast2Int(w));
    }

    public float GetWhiteNoiseInt(int x, int y) =>
        ValCoord2D(this.m_seed, x, y);

    public float GetWhiteNoiseInt(int x, int y, int z) =>
        ValCoord3D(this.m_seed, x, y, z);

    public float GetWhiteNoiseInt(int x, int y, int z, int w) =>
        ValCoord4D(this.m_seed, x, y, z, w);

    [MethodImpl(0x100)]
    private static float GradCoord2D(int seed, int x, int y, float xd, float yd)
    {
        int num = (seed ^ (0x653 * x)) ^ (0x7a69 * y);
        num = ((num * num) * num) * 0xec4d;
        num = (num >> 13) ^ num;
        Float2 num2 = GRAD_2D[num & 7];
        return ((xd * num2.x) + (yd * num2.y));
    }

    [MethodImpl(0x100)]
    private static float GradCoord3D(int seed, int x, int y, int z, float xd, float yd, float zd)
    {
        int num = ((seed ^ (0x653 * x)) ^ (0x7a69 * y)) ^ (0x1b3b * z);
        num = ((num * num) * num) * 0xec4d;
        num = (num >> 13) ^ num;
        Float3 num2 = GRAD_3D[num & 15];
        return (((xd * num2.x) + (yd * num2.y)) + (zd * num2.z));
    }

    [MethodImpl(0x100)]
    private static float GradCoord4D(int seed, int x, int y, int z, int w, float xd, float yd, float zd, float wd)
    {
        int num = (((seed ^ (0x653 * x)) ^ (0x7a69 * y)) ^ (0x1b3b * z)) ^ (0x3f5 * w);
        num = ((num * num) * num) * 0xec4d;
        num = ((num >> 13) ^ num) & 0x1f;
        float num2 = yd;
        float num3 = zd;
        float num4 = wd;
        int num5 = num >> 3;
        if (num5 == 1)
        {
            num2 = wd;
            num3 = xd;
            num4 = yd;
        }
        else if (num5 == 2)
        {
            num2 = zd;
            num3 = wd;
            num4 = xd;
        }
        else if (num5 == 3)
        {
            num2 = yd;
            num3 = zd;
            num4 = wd;
        }
        return (((((num & 4) != 0) ? num2 : -num2) + (((num & 2) != 0) ? num3 : -num3)) + (((num & 1) != 0) ? num4 : -num4));
    }

    public void GradientPerturb(ref float x, ref float y)
    {
        this.SingleGradientPerturb(this.m_seed, this.m_gradientPerturbAmp, this.m_frequency, ref x, ref y);
    }

    public void GradientPerturb(ref float x, ref float y, ref float z)
    {
        this.SingleGradientPerturb(this.m_seed, this.m_gradientPerturbAmp, this.m_frequency, ref x, ref y, ref z);
    }

    public void GradientPerturbFractal(ref float x, ref float y)
    {
        int seed = this.m_seed;
        float perturbAmp = this.m_gradientPerturbAmp * this.m_fractalBounding;
        float frequency = this.m_frequency;
        this.SingleGradientPerturb(seed, perturbAmp, this.m_frequency, ref x, ref y);
        for (int i = 1; i < this.m_octaves; i++)
        {
            frequency *= this.m_lacunarity;
            perturbAmp *= this.m_gain;
            this.SingleGradientPerturb(++seed, perturbAmp, frequency, ref x, ref y);
        }
    }

    public void GradientPerturbFractal(ref float x, ref float y, ref float z)
    {
        int seed = this.m_seed;
        float perturbAmp = this.m_gradientPerturbAmp * this.m_fractalBounding;
        float frequency = this.m_frequency;
        this.SingleGradientPerturb(seed, perturbAmp, this.m_frequency, ref x, ref y, ref z);
        for (int i = 1; i < this.m_octaves; i++)
        {
            frequency *= this.m_lacunarity;
            perturbAmp *= this.m_gain;
            this.SingleGradientPerturb(++seed, perturbAmp, frequency, ref x, ref y, ref z);
        }
    }

    [MethodImpl(0x100)]
    private static int Hash2D(int seed, int x, int y)
    {
        int num = (seed ^ (0x653 * x)) ^ (0x7a69 * y);
        num = ((num * num) * num) * 0xec4d;
        return ((num >> 13) ^ num);
    }

    [MethodImpl(0x100)]
    private static int Hash3D(int seed, int x, int y, int z)
    {
        int num = ((seed ^ (0x653 * x)) ^ (0x7a69 * y)) ^ (0x1b3b * z);
        num = ((num * num) * num) * 0xec4d;
        return ((num >> 13) ^ num);
    }

    [MethodImpl(0x100)]
    private static int Hash4D(int seed, int x, int y, int z, int w)
    {
        int num = (((seed ^ (0x653 * x)) ^ (0x7a69 * y)) ^ (0x1b3b * z)) ^ (0x3f5 * w);
        num = ((num * num) * num) * 0xec4d;
        return ((num >> 13) ^ num);
    }

    [MethodImpl(0x100)]
    private static float InterpHermiteFunc(float t) =>
        ((t * t) * (3f - (2f * t)));

    [MethodImpl(0x100)]
    private static float InterpQuinticFunc(float t) =>
        (((t * t) * t) * ((t * ((t * 6f) - 15f)) + 10f));

    [MethodImpl(0x100)]
    private static float Lerp(float a, float b, float t) =>
        (a + (t * (b - a)));

    public void SetCellularDistance2Indicies(int cellularDistanceIndex0, int cellularDistanceIndex1)
    {
        this.m_cellularDistanceIndex0 = Math.Min(cellularDistanceIndex0, cellularDistanceIndex1);
        this.m_cellularDistanceIndex1 = Math.Max(cellularDistanceIndex0, cellularDistanceIndex1);
        this.m_cellularDistanceIndex0 = Math.Min(Math.Max(this.m_cellularDistanceIndex0, 0), 3);
        this.m_cellularDistanceIndex1 = Math.Min(Math.Max(this.m_cellularDistanceIndex1, 0), 3);
    }

    public void SetCellularDistanceFunction(CellularDistanceFunction cellularDistanceFunction)
    {
        this.m_cellularDistanceFunction = cellularDistanceFunction;
    }

    public void SetCellularJitter(float cellularJitter)
    {
        this.m_cellularJitter = cellularJitter;
    }

    public void SetCellularNoiseLookup(FastNoise noise)
    {
        this.m_cellularNoiseLookup = noise;
    }

    public void SetCellularReturnType(CellularReturnType cellularReturnType)
    {
        this.m_cellularReturnType = cellularReturnType;
    }

    public void SetFractalGain(float gain)
    {
        this.m_gain = gain;
        this.CalculateFractalBounding();
    }

    public void SetFractalLacunarity(float lacunarity)
    {
        this.m_lacunarity = lacunarity;
    }

    public void SetFractalOctaves(int octaves)
    {
        this.m_octaves = octaves;
        this.CalculateFractalBounding();
    }

    public void SetFractalType(FractalType fractalType)
    {
        this.m_fractalType = fractalType;
    }

    public void SetFrequency(float frequency)
    {
        this.m_frequency = frequency;
    }

    public void SetGradientPerturbAmp(float gradientPerturbAmp)
    {
        this.m_gradientPerturbAmp = gradientPerturbAmp;
    }

    public void SetInterp(Interp interp)
    {
        this.m_interp = interp;
    }

    public void SetNoiseType(NoiseType noiseType)
    {
        this.m_noiseType = noiseType;
    }

    public void SetSeed(int seed)
    {
        this.m_seed = seed;
    }

    private float SingleCellular(float x, float y)
    {
        int num = FastRound(x);
        int num2 = FastRound(y);
        float num3 = 0xf_423ff;
        int num4 = 0;
        int num5 = 0;
        switch (this.m_cellularDistanceFunction)
        {
            case CellularDistanceFunction.Manhattan:
                {
                    int num12 = num - 1;
                    while (num12 <= (num + 1))
                    {
                        int num13 = num2 - 1;
                        while (true)
                        {
                            if (num13 > (num2 + 1))
                            {
                                num12++;
                                break;
                            }
                            Float2 num14 = CELL_2D[Hash2D(this.m_seed, num12, num13) & 0xff];
                            float num15 = (num12 - x) + (num14.x * this.m_cellularJitter);
                            float num16 = (num13 - y) + (num14.y * this.m_cellularJitter);
                            float num17 = Math.Abs(num15) + Math.Abs(num16);
                            if (num17 < num3)
                            {
                                num3 = num17;
                                num4 = num12;
                                num5 = num13;
                            }
                            num13++;
                        }
                    }
                    break;
                }
            case CellularDistanceFunction.Natural:
                {
                    int num18 = num - 1;
                    while (num18 <= (num + 1))
                    {
                        int num19 = num2 - 1;
                        while (true)
                        {
                            if (num19 > (num2 + 1))
                            {
                                num18++;
                                break;
                            }
                            Float2 num20 = CELL_2D[Hash2D(this.m_seed, num18, num19) & 0xff];
                            float num21 = (num18 - x) + (num20.x * this.m_cellularJitter);
                            float num22 = (num19 - y) + (num20.y * this.m_cellularJitter);
                            float num23 = (Math.Abs(num21) + Math.Abs(num22)) + ((num21 * num21) + (num22 * num22));
                            if (num23 < num3)
                            {
                                num3 = num23;
                                num4 = num18;
                                num5 = num19;
                            }
                            num19++;
                        }
                    }
                    break;
                }
            default:
                {
                    int num6 = num - 1;
                    while (num6 <= (num + 1))
                    {
                        int num7 = num2 - 1;
                        while (true)
                        {
                            if (num7 > (num2 + 1))
                            {
                                num6++;
                                break;
                            }
                            Float2 num8 = CELL_2D[Hash2D(this.m_seed, num6, num7) & 0xff];
                            float num9 = (num6 - x) + (num8.x * this.m_cellularJitter);
                            float num10 = (num7 - y) + (num8.y * this.m_cellularJitter);
                            float num11 = (num9 * num9) + (num10 * num10);
                            if (num11 < num3)
                            {
                                num3 = num11;
                                num4 = num6;
                                num5 = num7;
                            }
                            num7++;
                        }
                    }
                    break;
                }
        }
        switch (this.m_cellularReturnType)
        {
            case CellularReturnType.CellValue:
                return ValCoord2D(this.m_seed, num4, num5);

            case CellularReturnType.NoiseLookup:
                {
                    Float2 num24 = CELL_2D[Hash2D(this.m_seed, num4, num5) & 0xff];
                    return this.m_cellularNoiseLookup.GetNoise(num4 + (num24.x * this.m_cellularJitter), num5 + (num24.y * this.m_cellularJitter));
                }
            case CellularReturnType.Distance:
                return num3;
        }
        return 0f;
    }

    private float SingleCellular(float x, float y, float z)
    {
        int num = FastRound(x);
        int num2 = FastRound(y);
        int num3 = FastRound(z);
        float num4 = 0xf_423ff;
        int num5 = 0;
        int num6 = 0;
        int num7 = 0;
        CellularDistanceFunction cellularDistanceFunction = this.m_cellularDistanceFunction;
        if (cellularDistanceFunction == CellularDistanceFunction.Euclidean)
        {
            int num8 = num - 1;
            while (num8 <= (num + 1))
            {
                int num9 = num2 - 1;
                while (true)
                {
                    if (num9 > (num2 + 1))
                    {
                        num8++;
                        break;
                    }
                    int num10 = num3 - 1;
                    while (true)
                    {
                        if (num10 > (num3 + 1))
                        {
                            num9++;
                            break;
                        }
                        Float3 num11 = CELL_3D[Hash3D(this.m_seed, num8, num9, num10) & 0xff];
                        float num12 = (num8 - x) + (num11.x * this.m_cellularJitter);
                        float num13 = (num9 - y) + (num11.y * this.m_cellularJitter);
                        float num14 = (num10 - z) + (num11.z * this.m_cellularJitter);
                        float num15 = ((num12 * num12) + (num13 * num13)) + (num14 * num14);
                        if (num15 < num4)
                        {
                            num4 = num15;
                            num5 = num8;
                            num6 = num9;
                            num7 = num10;
                        }
                        num10++;
                    }
                }
            }
        }
        else if (cellularDistanceFunction == CellularDistanceFunction.Manhattan)
        {
            int num16 = num - 1;
            while (num16 <= (num + 1))
            {
                int num17 = num2 - 1;
                while (true)
                {
                    if (num17 > (num2 + 1))
                    {
                        num16++;
                        break;
                    }
                    int num18 = num3 - 1;
                    while (true)
                    {
                        if (num18 > (num3 + 1))
                        {
                            num17++;
                            break;
                        }
                        Float3 num19 = CELL_3D[Hash3D(this.m_seed, num16, num17, num18) & 0xff];
                        float num20 = (num16 - x) + (num19.x * this.m_cellularJitter);
                        float num21 = (num17 - y) + (num19.y * this.m_cellularJitter);
                        float num22 = (num18 - z) + (num19.z * this.m_cellularJitter);
                        float num23 = (Math.Abs(num20) + Math.Abs(num21)) + Math.Abs(num22);
                        if (num23 < num4)
                        {
                            num4 = num23;
                            num5 = num16;
                            num6 = num17;
                            num7 = num18;
                        }
                        num18++;
                    }
                }
            }
        }
        else if (cellularDistanceFunction == CellularDistanceFunction.Natural)
        {
            int num24 = num - 1;
            while (num24 <= (num + 1))
            {
                int num25 = num2 - 1;
                while (true)
                {
                    if (num25 > (num2 + 1))
                    {
                        num24++;
                        break;
                    }
                    int num26 = num3 - 1;
                    while (true)
                    {
                        if (num26 > (num3 + 1))
                        {
                            num25++;
                            break;
                        }
                        Float3 num27 = CELL_3D[Hash3D(this.m_seed, num24, num25, num26) & 0xff];
                        float num28 = (num24 - x) + (num27.x * this.m_cellularJitter);
                        float num29 = (num25 - y) + (num27.y * this.m_cellularJitter);
                        float num30 = (num26 - z) + (num27.z * this.m_cellularJitter);
                        float num31 = ((Math.Abs(num28) + Math.Abs(num29)) + Math.Abs(num30)) + (((num28 * num28) + (num29 * num29)) + (num30 * num30));
                        if (num31 < num4)
                        {
                            num4 = num31;
                            num5 = num24;
                            num6 = num25;
                            num7 = num26;
                        }
                        num26++;
                    }
                }
            }
        }
        switch (this.m_cellularReturnType)
        {
            case CellularReturnType.CellValue:
                return ValCoord3D(this.m_seed, num5, num6, num7);

            case CellularReturnType.NoiseLookup:
                {
                    Float3 num32 = CELL_3D[Hash3D(this.m_seed, num5, num6, num7) & 0xff];
                    return this.m_cellularNoiseLookup.GetNoise(num5 + (num32.x * this.m_cellularJitter), num6 + (num32.y * this.m_cellularJitter), num7 + (num32.z * this.m_cellularJitter));
                }
            case CellularReturnType.Distance:
                return num4;
        }
        return 0f;
    }

    private float SingleCellular2Edge(float x, float y)
    {
        int num = FastRound(x);
        int num2 = FastRound(y);
        float[] numArray = new float[] { 0xf_423ff, 0xf_423ff, 0xf_423ff, 0xf_423ff };
        switch (this.m_cellularDistanceFunction)
        {
            case CellularDistanceFunction.Manhattan:
                {
                    int num10 = num - 1;
                    while (num10 <= (num + 1))
                    {
                        int num11 = num2 - 1;
                        while (true)
                        {
                            if (num11 > (num2 + 1))
                            {
                                num10++;
                                break;
                            }
                            Float2 num12 = CELL_2D[Hash2D(this.m_seed, num10, num11) & 0xff];
                            float num13 = (num10 - x) + (num12.x * this.m_cellularJitter);
                            float num14 = (num11 - y) + (num12.y * this.m_cellularJitter);
                            float num15 = Math.Abs(num13) + Math.Abs(num14);
                            int index = this.m_cellularDistanceIndex1;
                            while (true)
                            {
                                if (index <= 0)
                                {
                                    numArray[0] = Math.Min(numArray[0], num15);
                                    num11++;
                                    break;
                                }
                                numArray[index] = Math.Max(Math.Min(numArray[index], num15), numArray[index - 1]);
                                index--;
                            }
                        }
                    }
                    break;
                }
            case CellularDistanceFunction.Natural:
                {
                    int num17 = num - 1;
                    while (num17 <= (num + 1))
                    {
                        int num18 = num2 - 1;
                        while (true)
                        {
                            if (num18 > (num2 + 1))
                            {
                                num17++;
                                break;
                            }
                            Float2 num19 = CELL_2D[Hash2D(this.m_seed, num17, num18) & 0xff];
                            float num20 = (num17 - x) + (num19.x * this.m_cellularJitter);
                            float num21 = (num18 - y) + (num19.y * this.m_cellularJitter);
                            float num22 = (Math.Abs(num20) + Math.Abs(num21)) + ((num20 * num20) + (num21 * num21));
                            int index = this.m_cellularDistanceIndex1;
                            while (true)
                            {
                                if (index <= 0)
                                {
                                    numArray[0] = Math.Min(numArray[0], num22);
                                    num18++;
                                    break;
                                }
                                numArray[index] = Math.Max(Math.Min(numArray[index], num22), numArray[index - 1]);
                                index--;
                            }
                        }
                    }
                    break;
                }
            default:
                {
                    int num3 = num - 1;
                    while (num3 <= (num + 1))
                    {
                        int num4 = num2 - 1;
                        while (true)
                        {
                            if (num4 > (num2 + 1))
                            {
                                num3++;
                                break;
                            }
                            Float2 num5 = CELL_2D[Hash2D(this.m_seed, num3, num4) & 0xff];
                            float num6 = (num3 - x) + (num5.x * this.m_cellularJitter);
                            float num7 = (num4 - y) + (num5.y * this.m_cellularJitter);
                            float num8 = (num6 * num6) + (num7 * num7);
                            int index = this.m_cellularDistanceIndex1;
                            while (true)
                            {
                                if (index <= 0)
                                {
                                    numArray[0] = Math.Min(numArray[0], num8);
                                    num4++;
                                    break;
                                }
                                numArray[index] = Math.Max(Math.Min(numArray[index], num8), numArray[index - 1]);
                                index--;
                            }
                        }
                    }
                    break;
                }
        }
        switch (this.m_cellularReturnType)
        {
            case CellularReturnType.Distance2:
                return numArray[this.m_cellularDistanceIndex1];

            case CellularReturnType.Distance2Add:
                return (numArray[this.m_cellularDistanceIndex1] + numArray[this.m_cellularDistanceIndex0]);

            case CellularReturnType.Distance2Sub:
                return (numArray[this.m_cellularDistanceIndex1] - numArray[this.m_cellularDistanceIndex0]);

            case CellularReturnType.Distance2Mul:
                return (numArray[this.m_cellularDistanceIndex1] * numArray[this.m_cellularDistanceIndex0]);

            case CellularReturnType.Distance2Div:
                return (numArray[this.m_cellularDistanceIndex0] / numArray[this.m_cellularDistanceIndex1]);
        }
        return 0f;
    }

    private float SingleCellular2Edge(float x, float y, float z)
    {
        int num = FastRound(x);
        int num2 = FastRound(y);
        int num3 = FastRound(z);
        float[] numArray = new float[] { 0xf_423ff, 0xf_423ff, 0xf_423ff, 0xf_423ff };
        switch (this.m_cellularDistanceFunction)
        {
            case CellularDistanceFunction.Euclidean:
                {
                    int num4 = num - 1;
                    while (num4 <= (num + 1))
                    {
                        int num5 = num2 - 1;
                        while (true)
                        {
                            if (num5 > (num2 + 1))
                            {
                                num4++;
                                break;
                            }
                            int num6 = num3 - 1;
                            while (true)
                            {
                                if (num6 > (num3 + 1))
                                {
                                    num5++;
                                    break;
                                }
                                Float3 num7 = CELL_3D[Hash3D(this.m_seed, num4, num5, num6) & 0xff];
                                float num8 = (num4 - x) + (num7.x * this.m_cellularJitter);
                                float num9 = (num5 - y) + (num7.y * this.m_cellularJitter);
                                float num10 = (num6 - z) + (num7.z * this.m_cellularJitter);
                                float num11 = ((num8 * num8) + (num9 * num9)) + (num10 * num10);
                                int index = this.m_cellularDistanceIndex1;
                                while (true)
                                {
                                    if (index <= 0)
                                    {
                                        numArray[0] = Math.Min(numArray[0], num11);
                                        num6++;
                                        break;
                                    }
                                    numArray[index] = Math.Max(Math.Min(numArray[index], num11), numArray[index - 1]);
                                    index--;
                                }
                            }
                        }
                    }
                    break;
                }
            case CellularDistanceFunction.Manhattan:
                {
                    int num13 = num - 1;
                    while (num13 <= (num + 1))
                    {
                        int num14 = num2 - 1;
                        while (true)
                        {
                            if (num14 > (num2 + 1))
                            {
                                num13++;
                                break;
                            }
                            int num15 = num3 - 1;
                            while (true)
                            {
                                if (num15 > (num3 + 1))
                                {
                                    num14++;
                                    break;
                                }
                                Float3 num16 = CELL_3D[Hash3D(this.m_seed, num13, num14, num15) & 0xff];
                                float num17 = (num13 - x) + (num16.x * this.m_cellularJitter);
                                float num18 = (num14 - y) + (num16.y * this.m_cellularJitter);
                                float num19 = (num15 - z) + (num16.z * this.m_cellularJitter);
                                float num20 = (Math.Abs(num17) + Math.Abs(num18)) + Math.Abs(num19);
                                int index = this.m_cellularDistanceIndex1;
                                while (true)
                                {
                                    if (index <= 0)
                                    {
                                        numArray[0] = Math.Min(numArray[0], num20);
                                        num15++;
                                        break;
                                    }
                                    numArray[index] = Math.Max(Math.Min(numArray[index], num20), numArray[index - 1]);
                                    index--;
                                }
                            }
                        }
                    }
                    break;
                }
            case CellularDistanceFunction.Natural:
                {
                    int num22 = num - 1;
                    while (num22 <= (num + 1))
                    {
                        int num23 = num2 - 1;
                        while (true)
                        {
                            if (num23 > (num2 + 1))
                            {
                                num22++;
                                break;
                            }
                            int num24 = num3 - 1;
                            while (true)
                            {
                                if (num24 > (num3 + 1))
                                {
                                    num23++;
                                    break;
                                }
                                Float3 num25 = CELL_3D[Hash3D(this.m_seed, num22, num23, num24) & 0xff];
                                float num26 = (num22 - x) + (num25.x * this.m_cellularJitter);
                                float num27 = (num23 - y) + (num25.y * this.m_cellularJitter);
                                float num28 = (num24 - z) + (num25.z * this.m_cellularJitter);
                                float num29 = ((Math.Abs(num26) + Math.Abs(num27)) + Math.Abs(num28)) + (((num26 * num26) + (num27 * num27)) + (num28 * num28));
                                int index = this.m_cellularDistanceIndex1;
                                while (true)
                                {
                                    if (index <= 0)
                                    {
                                        numArray[0] = Math.Min(numArray[0], num29);
                                        num24++;
                                        break;
                                    }
                                    numArray[index] = Math.Max(Math.Min(numArray[index], num29), numArray[index - 1]);
                                    index--;
                                }
                            }
                        }
                    }
                    break;
                }
            default:
                break;
        }
        switch (this.m_cellularReturnType)
        {
            case CellularReturnType.Distance2:
                return numArray[this.m_cellularDistanceIndex1];

            case CellularReturnType.Distance2Add:
                return (numArray[this.m_cellularDistanceIndex1] + numArray[this.m_cellularDistanceIndex0]);

            case CellularReturnType.Distance2Sub:
                return (numArray[this.m_cellularDistanceIndex1] - numArray[this.m_cellularDistanceIndex0]);

            case CellularReturnType.Distance2Mul:
                return (numArray[this.m_cellularDistanceIndex1] * numArray[this.m_cellularDistanceIndex0]);

            case CellularReturnType.Distance2Div:
                return (numArray[this.m_cellularDistanceIndex0] / numArray[this.m_cellularDistanceIndex1]);
        }
        return 0f;
    }

    private float SingleCubic(int seed, float x, float y)
    {
        int num = FastFloor(x);
        int num2 = FastFloor(y);
        int num3 = num - 1;
        int num4 = num2 - 1;
        int num5 = num + 1;
        int num6 = num2 + 1;
        int num7 = num + 2;
        int num8 = num2 + 2;
        float t = x - num;
        float num10 = y - num2;
        return (CubicLerp(CubicLerp(ValCoord2D(seed, num3, num4), ValCoord2D(seed, num, num4), ValCoord2D(seed, num5, num4), ValCoord2D(seed, num7, num4), t), CubicLerp(ValCoord2D(seed, num3, num2), ValCoord2D(seed, num, num2), ValCoord2D(seed, num5, num2), ValCoord2D(seed, num7, num2), t), CubicLerp(ValCoord2D(seed, num3, num6), ValCoord2D(seed, num, num6), ValCoord2D(seed, num5, num6), ValCoord2D(seed, num7, num6), t), CubicLerp(ValCoord2D(seed, num3, num8), ValCoord2D(seed, num, num8), ValCoord2D(seed, num5, num8), ValCoord2D(seed, num7, num8), t), num10) * 0.4444444f);
    }

    private float SingleCubic(int seed, float x, float y, float z)
    {
        int num = FastFloor(x);
        int num2 = FastFloor(y);
        int num3 = FastFloor(z);
        int num4 = num - 1;
        int num5 = num2 - 1;
        int num6 = num3 - 1;
        int num7 = num + 1;
        int num8 = num2 + 1;
        int num9 = num3 + 1;
        int num10 = num + 2;
        int num11 = num2 + 2;
        int num12 = num3 + 2;
        float t = x - num;
        float num14 = y - num2;
        float num15 = z - num3;
        return (CubicLerp(CubicLerp(CubicLerp(ValCoord3D(seed, num4, num5, num6), ValCoord3D(seed, num, num5, num6), ValCoord3D(seed, num7, num5, num6), ValCoord3D(seed, num10, num5, num6), t), CubicLerp(ValCoord3D(seed, num4, num2, num6), ValCoord3D(seed, num, num2, num6), ValCoord3D(seed, num7, num2, num6), ValCoord3D(seed, num10, num2, num6), t), CubicLerp(ValCoord3D(seed, num4, num8, num6), ValCoord3D(seed, num, num8, num6), ValCoord3D(seed, num7, num8, num6), ValCoord3D(seed, num10, num8, num6), t), CubicLerp(ValCoord3D(seed, num4, num11, num6), ValCoord3D(seed, num, num11, num6), ValCoord3D(seed, num7, num11, num6), ValCoord3D(seed, num10, num11, num6), t), num14), CubicLerp(CubicLerp(ValCoord3D(seed, num4, num5, num3), ValCoord3D(seed, num, num5, num3), ValCoord3D(seed, num7, num5, num3), ValCoord3D(seed, num10, num5, num3), t), CubicLerp(ValCoord3D(seed, num4, num2, num3), ValCoord3D(seed, num, num2, num3), ValCoord3D(seed, num7, num2, num3), ValCoord3D(seed, num10, num2, num3), t), CubicLerp(ValCoord3D(seed, num4, num8, num3), ValCoord3D(seed, num, num8, num3), ValCoord3D(seed, num7, num8, num3), ValCoord3D(seed, num10, num8, num3), t), CubicLerp(ValCoord3D(seed, num4, num11, num3), ValCoord3D(seed, num, num11, num3), ValCoord3D(seed, num7, num11, num3), ValCoord3D(seed, num10, num11, num3), t), num14), CubicLerp(CubicLerp(ValCoord3D(seed, num4, num5, num9), ValCoord3D(seed, num, num5, num9), ValCoord3D(seed, num7, num5, num9), ValCoord3D(seed, num10, num5, num9), t), CubicLerp(ValCoord3D(seed, num4, num2, num9), ValCoord3D(seed, num, num2, num9), ValCoord3D(seed, num7, num2, num9), ValCoord3D(seed, num10, num2, num9), t), CubicLerp(ValCoord3D(seed, num4, num8, num9), ValCoord3D(seed, num, num8, num9), ValCoord3D(seed, num7, num8, num9), ValCoord3D(seed, num10, num8, num9), t), CubicLerp(ValCoord3D(seed, num4, num11, num9), ValCoord3D(seed, num, num11, num9), ValCoord3D(seed, num7, num11, num9), ValCoord3D(seed, num10, num11, num9), t), num14), CubicLerp(CubicLerp(ValCoord3D(seed, num4, num5, num12), ValCoord3D(seed, num, num5, num12), ValCoord3D(seed, num7, num5, num12), ValCoord3D(seed, num10, num5, num12), t), CubicLerp(ValCoord3D(seed, num4, num2, num12), ValCoord3D(seed, num, num2, num12), ValCoord3D(seed, num7, num2, num12), ValCoord3D(seed, num10, num2, num12), t), CubicLerp(ValCoord3D(seed, num4, num8, num12), ValCoord3D(seed, num, num8, num12), ValCoord3D(seed, num7, num8, num12), ValCoord3D(seed, num10, num8, num12), t), CubicLerp(ValCoord3D(seed, num4, num11, num12), ValCoord3D(seed, num, num11, num12), ValCoord3D(seed, num7, num11, num12), ValCoord3D(seed, num10, num11, num12), t), num14), num15) * 0.2962963f);
    }

    private float SingleCubicFractalBillow(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SingleCubic(seed, x, y)) * 2f) - 1f;
        float num3 = 1f;
        int num4 = 0;
        while (++num4 < this.m_octaves)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SingleCubic(++seed, x, y)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleCubicFractalBillow(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SingleCubic(seed, x, y, z)) * 2f) - 1f;
        float num3 = 1f;
        int num4 = 0;
        while (++num4 < this.m_octaves)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SingleCubic(++seed, x, y, z)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleCubicFractalFBM(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = this.SingleCubic(seed, x, y);
        float num3 = 1f;
        int num4 = 0;
        while (++num4 < this.m_octaves)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SingleCubic(++seed, x, y) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleCubicFractalFBM(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = this.SingleCubic(seed, x, y, z);
        float num3 = 1f;
        int num4 = 0;
        while (++num4 < this.m_octaves)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SingleCubic(++seed, x, y, z) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleCubicFractalRigidMulti(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SingleCubic(seed, x, y));
        float num3 = 1f;
        int num4 = 0;
        while (++num4 < this.m_octaves)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SingleCubic(++seed, x, y))) * num3;
        }
        return num2;
    }

    private float SingleCubicFractalRigidMulti(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SingleCubic(seed, x, y, z));
        float num3 = 1f;
        int num4 = 0;
        while (++num4 < this.m_octaves)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SingleCubic(++seed, x, y, z))) * num3;
        }
        return num2;
    }

    private void SingleGradientPerturb(int seed, float perturbAmp, float frequency, ref float x, ref float y)
    {
        float num7;
        float num8;
        float f = x * frequency;
        float num2 = y * frequency;
        int num3 = FastFloor(f);
        int num4 = FastFloor(num2);
        int num5 = num3 + 1;
        int num6 = num4 + 1;
        switch (this.m_interp)
        {
            case Interp.Hermite:
                num7 = InterpHermiteFunc(f - num3);
                num8 = InterpHermiteFunc(num2 - num4);
                break;

            case Interp.Quintic:
                num7 = InterpQuinticFunc(f - num3);
                num8 = InterpQuinticFunc(num2 - num4);
                break;

            default:
                num7 = f - num3;
                num8 = num2 - num4;
                break;
        }
        Float2 num9 = CELL_2D[Hash2D(seed, num3, num4) & 0xff];
        Float2 num10 = CELL_2D[Hash2D(seed, num5, num4) & 0xff];
        float a = Lerp(num9.x, num10.x, num7);
        float num12 = Lerp(num9.y, num10.y, num7);
        num9 = CELL_2D[Hash2D(seed, num3, num6) & 0xff];
        num10 = CELL_2D[Hash2D(seed, num5, num6) & 0xff];
        x += Lerp(a, Lerp(num9.x, num10.x, num7), num8) * perturbAmp;
        y += Lerp(num12, Lerp(num9.y, num10.y, num7), num8) * perturbAmp;
    }

    private void SingleGradientPerturb(int seed, float perturbAmp, float frequency, ref float x, ref float y, ref float z)
    {
        float num10;
        float num11;
        float num12;
        float f = x * frequency;
        float num2 = y * frequency;
        float num3 = z * frequency;
        int num4 = FastFloor(f);
        int num5 = FastFloor(num2);
        int num6 = FastFloor(num3);
        int num7 = num4 + 1;
        int num8 = num5 + 1;
        int num9 = num6 + 1;
        switch (this.m_interp)
        {
            case Interp.Hermite:
                num10 = InterpHermiteFunc(f - num4);
                num11 = InterpHermiteFunc(num2 - num5);
                num12 = InterpHermiteFunc(num3 - num6);
                break;

            case Interp.Quintic:
                num10 = InterpQuinticFunc(f - num4);
                num11 = InterpQuinticFunc(num2 - num5);
                num12 = InterpQuinticFunc(num3 - num6);
                break;

            default:
                num10 = f - num4;
                num11 = num2 - num5;
                num12 = num3 - num6;
                break;
        }
        Float3 num13 = CELL_3D[Hash3D(seed, num4, num5, num6) & 0xff];
        Float3 num14 = CELL_3D[Hash3D(seed, num7, num5, num6) & 0xff];
        num13 = CELL_3D[Hash3D(seed, num4, num8, num6) & 0xff];
        num14 = CELL_3D[Hash3D(seed, num7, num8, num6) & 0xff];
        float a = Lerp(Lerp(num13.x, num14.x, num10), Lerp(num13.x, num14.x, num10), num11);
        float num22 = Lerp(Lerp(num13.y, num14.y, num10), Lerp(num13.y, num14.y, num10), num11);
        float num23 = Lerp(Lerp(num13.z, num14.z, num10), Lerp(num13.z, num14.z, num10), num11);
        num13 = CELL_3D[Hash3D(seed, num4, num5, num9) & 0xff];
        num14 = CELL_3D[Hash3D(seed, num7, num5, num9) & 0xff];
        num13 = CELL_3D[Hash3D(seed, num4, num8, num9) & 0xff];
        num14 = CELL_3D[Hash3D(seed, num7, num8, num9) & 0xff];
        x += Lerp(a, Lerp(Lerp(num13.x, num14.x, num10), Lerp(num13.x, num14.x, num10), num11), num12) * perturbAmp;
        y += Lerp(num22, Lerp(Lerp(num13.y, num14.y, num10), Lerp(num13.y, num14.y, num10), num11), num12) * perturbAmp;
        z += Lerp(num23, Lerp(Lerp(num13.z, num14.z, num10), Lerp(num13.z, num14.z, num10), num11), num12) * perturbAmp;
    }

    private float SinglePerlin(int seed, float x, float y)
    {
        float num5;
        float num6;
        int num = FastFloor(x);
        int num2 = FastFloor(y);
        int num3 = num + 1;
        int num4 = num2 + 1;
        switch (this.m_interp)
        {
            case Interp.Hermite:
                num5 = InterpHermiteFunc(x - num);
                num6 = InterpHermiteFunc(y - num2);
                break;

            case Interp.Quintic:
                num5 = InterpQuinticFunc(x - num);
                num6 = InterpQuinticFunc(y - num2);
                break;

            default:
                num5 = x - num;
                num6 = y - num2;
                break;
        }
        float xd = x - num;
        float yd = y - num2;
        float num9 = xd - 1f;
        float num10 = yd - 1f;
        return Lerp(Lerp(GradCoord2D(seed, num, num2, xd, yd), GradCoord2D(seed, num3, num2, num9, yd), num5), Lerp(GradCoord2D(seed, num, num4, xd, num10), GradCoord2D(seed, num3, num4, num9, num10), num5), num6);
    }

    private float SinglePerlin(int seed, float x, float y, float z)
    {
        float num7;
        float num8;
        float num9;
        int num = FastFloor(x);
        int num2 = FastFloor(y);
        int num3 = FastFloor(z);
        int num4 = num + 1;
        int num5 = num2 + 1;
        int num6 = num3 + 1;
        switch (this.m_interp)
        {
            case Interp.Hermite:
                num7 = InterpHermiteFunc(x - num);
                num8 = InterpHermiteFunc(y - num2);
                num9 = InterpHermiteFunc(z - num3);
                break;

            case Interp.Quintic:
                num7 = InterpQuinticFunc(x - num);
                num8 = InterpQuinticFunc(y - num2);
                num9 = InterpQuinticFunc(z - num3);
                break;

            default:
                num7 = x - num;
                num8 = y - num2;
                num9 = z - num3;
                break;
        }
        float xd = x - num;
        float yd = y - num2;
        float zd = z - num3;
        float num13 = xd - 1f;
        float num14 = yd - 1f;
        float num15 = zd - 1f;
        return Lerp(Lerp(Lerp(GradCoord3D(seed, num, num2, num3, xd, yd, zd), GradCoord3D(seed, num4, num2, num3, num13, yd, zd), num7), Lerp(GradCoord3D(seed, num, num5, num3, xd, num14, zd), GradCoord3D(seed, num4, num5, num3, num13, num14, zd), num7), num8), Lerp(Lerp(GradCoord3D(seed, num, num2, num6, xd, yd, num15), GradCoord3D(seed, num4, num2, num6, num13, yd, num15), num7), Lerp(GradCoord3D(seed, num, num5, num6, xd, num14, num15), GradCoord3D(seed, num4, num5, num6, num13, num14, num15), num7), num8), num9);
    }

    private float SinglePerlinFractalBillow(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SinglePerlin(seed, x, y)) * 2f) - 1f;
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SinglePerlin(++seed, x, y)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SinglePerlinFractalBillow(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SinglePerlin(seed, x, y, z)) * 2f) - 1f;
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SinglePerlin(++seed, x, y, z)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SinglePerlinFractalFBM(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = this.SinglePerlin(seed, x, y);
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SinglePerlin(++seed, x, y) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SinglePerlinFractalFBM(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = this.SinglePerlin(seed, x, y, z);
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SinglePerlin(++seed, x, y, z) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SinglePerlinFractalRigidMulti(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SinglePerlin(seed, x, y));
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SinglePerlin(++seed, x, y))) * num3;
        }
        return num2;
    }

    private float SinglePerlinFractalRigidMulti(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SinglePerlin(seed, x, y, z));
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SinglePerlin(++seed, x, y, z))) * num3;
        }
        return num2;
    }

    private float SingleSimplex(int seed, float x, float y)
    {
        int num8;
        int num9;
        float num14;
        float num15;
        float num16;
        float num = (x + y) * 0.5f;
        int num2 = FastFloor(x + num);
        int num3 = FastFloor(y + num);
        num = (num2 + num3) * 0.25f;
        float num4 = num2 - num;
        float num5 = num3 - num;
        float xd = x - num4;
        float yd = y - num5;
        if (xd > yd)
        {
            num8 = 1;
            num9 = 0;
        }
        else
        {
            num8 = 0;
            num9 = 1;
        }
        float num10 = (xd - num8) + 0.25f;
        float num11 = (yd - num9) + 0.25f;
        float num12 = (xd - 1f) + 0.5f;
        float num13 = (yd - 1f) + 0.5f;
        num = (0.5f - (xd * xd)) - (yd * yd);
        if (num < 0f)
        {
            num14 = 0f;
        }
        else
        {
            num *= num;
            num14 = (num * num) * GradCoord2D(seed, num2, num3, xd, yd);
        }
        num = (0.5f - (num10 * num10)) - (num11 * num11);
        if (num < 0f)
        {
            num15 = 0f;
        }
        else
        {
            num *= num;
            num15 = (num * num) * GradCoord2D(seed, num2 + num8, num3 + num9, num10, num11);
        }
        num = (0.5f - (num12 * num12)) - (num13 * num13);
        if (num < 0f)
        {
            num16 = 0f;
        }
        else
        {
            num *= num;
            num16 = (num * num) * GradCoord2D(seed, num2 + 1, num3 + 1, num12, num13);
        }
        return (50f * ((num14 + num15) + num16));
    }

    private float SingleSimplex(int seed, float x, float y, float z)
    {
        int num8;
        int num9;
        int num10;
        int num11;
        int num12;
        int num13;
        float num23;
        float num24;
        float num25;
        float num26;
        float num = ((x + y) + z) * 0.3333333f;
        int num2 = FastFloor(x + num);
        int num3 = FastFloor(y + num);
        int num4 = FastFloor(z + num);
        num = ((num2 + num3) + num4) * 0.1666667f;
        float xd = x - (num2 - num);
        float yd = y - (num3 - num);
        float zd = z - (num4 - num);
        if (xd >= yd)
        {
            if (yd >= zd)
            {
                num8 = 1;
                num9 = 0;
                num10 = 0;
                num11 = 1;
                num12 = 1;
                num13 = 0;
            }
            else if (xd >= zd)
            {
                num8 = 1;
                num9 = 0;
                num10 = 0;
                num11 = 1;
                num12 = 0;
                num13 = 1;
            }
            else
            {
                num8 = 0;
                num9 = 0;
                num10 = 1;
                num11 = 1;
                num12 = 0;
                num13 = 1;
            }
        }
        else if (yd < zd)
        {
            num8 = 0;
            num9 = 0;
            num10 = 1;
            num11 = 0;
            num12 = 1;
            num13 = 1;
        }
        else if (xd < zd)
        {
            num8 = 0;
            num9 = 1;
            num10 = 0;
            num11 = 0;
            num12 = 1;
            num13 = 1;
        }
        else
        {
            num8 = 0;
            num9 = 1;
            num10 = 0;
            num11 = 1;
            num12 = 1;
            num13 = 0;
        }
        float num14 = (xd - num8) + 0.1666667f;
        float num15 = (yd - num9) + 0.1666667f;
        float num16 = (zd - num10) + 0.1666667f;
        float num17 = (xd - num11) + 0.3333333f;
        float num18 = (yd - num12) + 0.3333333f;
        float num19 = (zd - num13) + 0.3333333f;
        float num20 = xd + -0.5f;
        float num21 = yd + -0.5f;
        float num22 = zd + -0.5f;
        num = ((0.6f - (xd * xd)) - (yd * yd)) - (zd * zd);
        if (num < 0f)
        {
            num23 = 0f;
        }
        else
        {
            num *= num;
            num23 = (num * num) * GradCoord3D(seed, num2, num3, num4, xd, yd, zd);
        }
        num = ((0.6f - (num14 * num14)) - (num15 * num15)) - (num16 * num16);
        if (num < 0f)
        {
            num24 = 0f;
        }
        else
        {
            num *= num;
            num24 = (num * num) * GradCoord3D(seed, num2 + num8, num3 + num9, num4 + num10, num14, num15, num16);
        }
        num = ((0.6f - (num17 * num17)) - (num18 * num18)) - (num19 * num19);
        if (num < 0f)
        {
            num25 = 0f;
        }
        else
        {
            num *= num;
            num25 = (num * num) * GradCoord3D(seed, num2 + num11, num3 + num12, num4 + num13, num17, num18, num19);
        }
        num = ((0.6f - (num20 * num20)) - (num21 * num21)) - (num22 * num22);
        if (num < 0f)
        {
            num26 = 0f;
        }
        else
        {
            num *= num;
            num26 = (num * num) * GradCoord3D(seed, num2 + 1, num3 + 1, num4 + 1, num20, num21, num22);
        }
        return (0x20f * (((num23 + num24) + num25) + num26));
    }

    private float SingleSimplex(int seed, float x, float y, float z, float w)
    {
        float num;
        float num2;
        float num3;
        float num4;
        float num5;
        float num6 = (((x + y) + z) + w) * 0.309017f;
        int num7 = FastFloor(x + num6);
        int num8 = FastFloor(y + num6);
        int num9 = FastFloor(z + num6);
        int num10 = FastFloor(w + num6);
        num6 = (((num7 + num8) + num9) + num10) * 0.1381966f;
        float num11 = num7 - num6;
        float num12 = num8 - num6;
        float num13 = num9 - num6;
        float num14 = num10 - num6;
        float xd = x - num11;
        float yd = y - num12;
        float zd = z - num13;
        float wd = w - num14;
        int index = (((((((xd <= yd) ? 0 : 0x20) + ((xd <= zd) ? 0 : 0x10)) + ((yd <= zd) ? 0 : 8)) + ((xd <= wd) ? 0 : 4)) + ((yd <= wd) ? 0 : 2)) + ((zd <= wd) ? 0 : 1)) << 2;
        int num20 = (SIMPLEX_4D[index] < 3) ? 0 : 1;
        int num21 = (SIMPLEX_4D[index] < 2) ? 0 : 1;
        index++;
        int num22 = (SIMPLEX_4D[index] < 1) ? 0 : 1;
        int num23 = (SIMPLEX_4D[index] < 3) ? 0 : 1;
        int num24 = (SIMPLEX_4D[index] < 2) ? 0 : 1;
        index++;
        int num25 = (SIMPLEX_4D[index] < 1) ? 0 : 1;
        int num26 = (SIMPLEX_4D[index] < 3) ? 0 : 1;
        int num27 = (SIMPLEX_4D[index] < 2) ? 0 : 1;
        index++;
        int num28 = (SIMPLEX_4D[index] < 1) ? 0 : 1;
        int num29 = (SIMPLEX_4D[index] < 3) ? 0 : 1;
        int num30 = (SIMPLEX_4D[index] < 2) ? 0 : 1;
        int num31 = (SIMPLEX_4D[index] < 1) ? 0 : 1;
        float num32 = (xd - num20) + 0.1381966f;
        float num33 = (yd - num23) + 0.1381966f;
        float num34 = (zd - num26) + 0.1381966f;
        float num35 = (wd - num29) + 0.1381966f;
        float num36 = (xd - num21) + 0.2763932f;
        float num37 = (yd - num24) + 0.2763932f;
        float num38 = (zd - num27) + 0.2763932f;
        float num39 = (wd - num30) + 0.2763932f;
        float num40 = (xd - num22) + 0.4145898f;
        float num41 = (yd - num25) + 0.4145898f;
        float num42 = (zd - num28) + 0.4145898f;
        float num43 = (wd - num31) + 0.4145898f;
        float num44 = (xd - 1f) + 0.5527864f;
        float num45 = (yd - 1f) + 0.5527864f;
        float num46 = (zd - 1f) + 0.5527864f;
        float num47 = (wd - 1f) + 0.5527864f;
        num6 = (((0.6f - (xd * xd)) - (yd * yd)) - (zd * zd)) - (wd * wd);
        if (num6 < 0f)
        {
            num = 0f;
        }
        else
        {
            num6 *= num6;
            num = (num6 * num6) * GradCoord4D(seed, num7, num8, num9, num10, xd, yd, zd, wd);
        }
        num6 = (((0.6f - (num32 * num32)) - (num33 * num33)) - (num34 * num34)) - (num35 * num35);
        if (num6 < 0f)
        {
            num2 = 0f;
        }
        else
        {
            num6 *= num6;
            num2 = (num6 * num6) * GradCoord4D(seed, num7 + num20, num8 + num23, num9 + num26, num10 + num29, num32, num33, num34, num35);
        }
        num6 = (((0.6f - (num36 * num36)) - (num37 * num37)) - (num38 * num38)) - (num39 * num39);
        if (num6 < 0f)
        {
            num3 = 0f;
        }
        else
        {
            num6 *= num6;
            num3 = (num6 * num6) * GradCoord4D(seed, num7 + num21, num8 + num24, num9 + num27, num10 + num30, num36, num37, num38, num39);
        }
        num6 = (((0.6f - (num40 * num40)) - (num41 * num41)) - (num42 * num42)) - (num43 * num43);
        if (num6 < 0f)
        {
            num4 = 0f;
        }
        else
        {
            num6 *= num6;
            num4 = (num6 * num6) * GradCoord4D(seed, num7 + num22, num8 + num25, num9 + num28, num10 + num31, num40, num41, num42, num43);
        }
        num6 = (((0.6f - (num44 * num44)) - (num45 * num45)) - (num46 * num46)) - (num47 * num47);
        if (num6 < 0f)
        {
            num5 = 0f;
        }
        else
        {
            num6 *= num6;
            num5 = (num6 * num6) * GradCoord4D(seed, num7 + 1, num8 + 1, num9 + 1, num10 + 1, num44, num45, num46, num47);
        }
        return (0x1bf * ((((num + num2) + num3) + num4) + num5));
    }

    private float SingleSimplexFractalBillow(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SingleSimplex(seed, x, y)) * 2f) - 1f;
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SingleSimplex(++seed, x, y)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleSimplexFractalBillow(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SingleSimplex(seed, x, y, z)) * 2f) - 1f;
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SingleSimplex(++seed, x, y, z)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleSimplexFractalFBM(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = this.SingleSimplex(seed, x, y);
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SingleSimplex(++seed, x, y) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleSimplexFractalFBM(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = this.SingleSimplex(seed, x, y, z);
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SingleSimplex(++seed, x, y, z) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleSimplexFractalRigidMulti(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SingleSimplex(seed, x, y));
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SingleSimplex(++seed, x, y))) * num3;
        }
        return num2;
    }

    private float SingleSimplexFractalRigidMulti(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SingleSimplex(seed, x, y, z));
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SingleSimplex(++seed, x, y, z))) * num3;
        }
        return num2;
    }

    private float SingleValue(int seed, float x, float y)
    {
        float num5;
        float num6;
        int num = FastFloor(x);
        int num2 = FastFloor(y);
        int num3 = num + 1;
        int num4 = num2 + 1;
        switch (this.m_interp)
        {
            case Interp.Hermite:
                num5 = InterpHermiteFunc(x - num);
                num6 = InterpHermiteFunc(y - num2);
                break;

            case Interp.Quintic:
                num5 = InterpQuinticFunc(x - num);
                num6 = InterpQuinticFunc(y - num2);
                break;

            default:
                num5 = x - num;
                num6 = y - num2;
                break;
        }
        return Lerp(Lerp(ValCoord2D(seed, num, num2), ValCoord2D(seed, num3, num2), num5), Lerp(ValCoord2D(seed, num, num4), ValCoord2D(seed, num3, num4), num5), num6);
    }

    private float SingleValue(int seed, float x, float y, float z)
    {
        float num7;
        float num8;
        float num9;
        int num = FastFloor(x);
        int num2 = FastFloor(y);
        int num3 = FastFloor(z);
        int num4 = num + 1;
        int num5 = num2 + 1;
        int num6 = num3 + 1;
        switch (this.m_interp)
        {
            case Interp.Hermite:
                num7 = InterpHermiteFunc(x - num);
                num8 = InterpHermiteFunc(y - num2);
                num9 = InterpHermiteFunc(z - num3);
                break;

            case Interp.Quintic:
                num7 = InterpQuinticFunc(x - num);
                num8 = InterpQuinticFunc(y - num2);
                num9 = InterpQuinticFunc(z - num3);
                break;

            default:
                num7 = x - num;
                num8 = y - num2;
                num9 = z - num3;
                break;
        }
        return Lerp(Lerp(Lerp(ValCoord3D(seed, num, num2, num3), ValCoord3D(seed, num4, num2, num3), num7), Lerp(ValCoord3D(seed, num, num5, num3), ValCoord3D(seed, num4, num5, num3), num7), num8), Lerp(Lerp(ValCoord3D(seed, num, num2, num6), ValCoord3D(seed, num4, num2, num6), num7), Lerp(ValCoord3D(seed, num, num5, num6), ValCoord3D(seed, num4, num5, num6), num7), num8), num9);
    }

    private float SingleValueFractalBillow(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SingleValue(seed, x, y)) * 2f) - 1f;
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SingleValue(++seed, x, y)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleValueFractalBillow(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = (Math.Abs(this.SingleValue(seed, x, y, z)) * 2f) - 1f;
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += ((Math.Abs(this.SingleValue(++seed, x, y, z)) * 2f) - 1f) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleValueFractalFBM(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = this.SingleValue(seed, x, y);
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SingleValue(++seed, x, y) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleValueFractalFBM(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = this.SingleValue(seed, x, y, z);
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 += this.SingleValue(++seed, x, y, z) * num3;
        }
        return (num2 * this.m_fractalBounding);
    }

    private float SingleValueFractalRigidMulti(float x, float y)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SingleValue(seed, x, y));
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SingleValue(++seed, x, y))) * num3;
        }
        return num2;
    }

    private float SingleValueFractalRigidMulti(float x, float y, float z)
    {
        int seed = this.m_seed;
        float num2 = 1f - Math.Abs(this.SingleValue(seed, x, y, z));
        float num3 = 1f;
        for (int i = 1; i < this.m_octaves; i++)
        {
            x *= this.m_lacunarity;
            y *= this.m_lacunarity;
            z *= this.m_lacunarity;
            num3 *= this.m_gain;
            num2 -= (1f - Math.Abs(this.SingleValue(++seed, x, y, z))) * num3;
        }
        return num2;
    }

    [MethodImpl(0x100)]
    private static float ValCoord2D(int seed, int x, int y)
    {
        int num = (seed ^ (0x653 * x)) ^ (0x7a69 * y);
        return (((float)(((num * num) * num) * 0xec4d)) / 0x8000_0000f);
    }

    [MethodImpl(0x100)]
    private static float ValCoord3D(int seed, int x, int y, int z)
    {
        int num = ((seed ^ (0x653 * x)) ^ (0x7a69 * y)) ^ (0x1b3b * z);
        return (((float)(((num * num) * num) * 0xec4d)) / 0x8000_0000f);
    }

    [MethodImpl(0x100)]
    private static float ValCoord4D(int seed, int x, int y, int z, int w)
    {
        int num = (((seed ^ (0x653 * x)) ^ (0x7a69 * y)) ^ (0x1b3b * z)) ^ (0x3f5 * w);
        return (((float)(((num * num) * num) * 0xec4d)) / 0x8000_0000f);
    }

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
        NoiseLookup,
        Distance,
        Distance2,
        Distance2Add,
        Distance2Sub,
        Distance2Mul,
        Distance2Div
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Float2
    {
        public readonly float x;
        public readonly float y;
        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Float3
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public enum FractalType
    {
        FBM,
        Billow,
        RigidMulti
    }

    public enum Interp
    {
        Linear,
        Hermite,
        Quintic
    }

    public enum NoiseType
    {
        Value,
        ValueFractal,
        Perlin,
        PerlinFractal,
        Simplex,
        SimplexFractal,
        Cellular,
        WhiteNoise,
        Cubic,
        CubicFractal
    }
}