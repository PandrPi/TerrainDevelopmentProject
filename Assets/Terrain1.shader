Shader "Custom/Terrain1"
{
    Properties
    {
        _GrassColor ("Grass Color", 2D) = "white" {}
        _GrassNormal ("Grass Bump", 2D) = "bump" {}
        _CliffColor ("Cliff Color", 2D) = "white" {}
		_CliffNormal("Cliff Bump", 2D) = "bump" {}
		_DetailNormal("Detail Bump", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_GrassSlopeThreshold("Grass Slope Threshold", Range(0,1)) = 0.0
		_GrassBlendAmount("Grass Blend Amount", Range(0,1)) = 0.0
		_DetailNormalBlendFactor("Detail Normal Blend Factor", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		ZWrite On
		ZTest LEqual

        LOD 2000

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _GrassColor;
		sampler2D _CliffColor;
		sampler2D _GrassNormal;
		sampler2D _CliffNormal;
		sampler2D _DetailNormal;

        struct Input
        {
            float2 uv_GrassColor;
            float2 uv_CliffColor;
            float2 uv_DetailNormal;
			float3 worldNormal; INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
		half _GrassSlopeThreshold;
		half _GrassBlendAmount;
		half _DetailNormalBlendFactor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Normal = half3(0, 0, 1);
			half3 worldNormal = WorldNormalVector(IN, half3(0, 0, 1));

			half worldNormalDotFactor = dot(half3(0, 1, 0), worldNormal);
			half slope = 1 - worldNormalDotFactor;
			half grassBlendHeight = _GrassSlopeThreshold * (1 - _GrassBlendAmount);
			half grassWeight = 1 - saturate((slope - grassBlendHeight) / (_GrassSlopeThreshold - grassBlendHeight));

			half4 grassTex = half4(0,0,0,0);
			half4 cliffTex = half4(0,0,0,0);

			if (grassWeight != 0) {
				grassTex = tex2D(_GrassColor, IN.uv_GrassColor) * grassWeight;
				o.Albedo += grassTex;
			}
			if (grassWeight != 1) {
				cliffTex = tex2D(_CliffColor, IN.uv_CliffColor) * (1 - grassWeight);
				o.Albedo += cliffTex;
			}

			half3 normal = UnpackScaleNormal(tex2D(_DetailNormal, IN.uv_DetailNormal), lerp(0.5, 0.25, grassWeight));
			o.Normal = normalize(normal);

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
