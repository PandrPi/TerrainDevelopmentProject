Shader "Custom/Diffuse POM"
{
	Properties
	{
			_MainTex("Diffuse map (RGB)", 2D) = "white" {}
			_HeightMap("Height map (R)", 2D) = "white" {}
			_Tiling("Texture tiling", Float) = 1
			_Parallax("Height scale", Range(0.0005, 0.005)) = 0.08
			_ParallaxSamples("Parallax samples", Range(10, 100)) = 40
			_ParallaxDistance("Parallax distance", Range(0, 100)) = 50
			_ParallaxFalloff("Parallax falloff", Range(0, 5)) = 1
			_OcclusionMapYOffset("Occlusion map y offset", Range(0, 1)) = 0

	}
		SubShader
			{
				Pass
				{
					Tags { "LightMode" = "ForwardBase" }
					CGPROGRAM
					#pragma vertex vertex_shader
					#pragma fragment pixel_shader
					#pragma target 3.0

					#include "UnityCG.cginc"
					#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
					#include "AutoLight.cginc"

					sampler2D _MainTex;
					sampler2D _HeightMap;
					float _Tiling;
					float _Parallax;
					float _ParallaxSamples;
					float _ParallaxDistance;
					float _ParallaxFalloff;
					float _OcclusionMapYOffset;
					uniform float4 _LightColor0;

					struct vertexInput
					{
						float4 vertex: POSITION;
						float3 normal: NORMAL;
						float2 texcoord: TEXCOORD0;
						float4 tangent  : TANGENT;
					};

					struct vertexOutput
					{
						float4 pos: SV_POSITION;
						float2 tex: TEXCOORD0;
						SHADOW_COORDS(1)
						float4 posWorld: TEXCOORD2;
						float3x3 tSpace : TEXCOORD3;
						float3 normal  : TEXCOORD6;
						float3 Diffuse : TEXCOORD7;
					};

					vertexOutput vertex_shader(vertexInput v)
					{
						vertexOutput o;
						o.posWorld = mul(unity_ObjectToWorld, v.vertex);
						fixed3 worldNormal = mul(v.normal.xyz, (float3x3)unity_WorldToObject);
						fixed3 worldTangent = normalize(mul((float3x3)unity_ObjectToWorld,v.tangent.xyz));
						fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
						o.tSpace = transpose(float3x3(float3(worldTangent.x, worldBinormal.x, worldNormal.x),
													  float3(worldTangent.y, worldBinormal.y, worldNormal.y),
													  float3(worldTangent.z, worldBinormal.z, worldNormal.z)));
						o.pos = UnityObjectToClipPos(v.vertex);
						o.tex = v.texcoord;
						o.normal = worldNormal;

						float3 NdotL = saturate(dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
						o.Diffuse = _LightColor0.rgb * NdotL;

						TRANSFER_SHADOW(o)

						return o;
					}

					float4 pixel_shader(vertexOutput i) : SV_TARGET
					{
						float3	normalDirection = normalize(i.normal);
						fixed3	worldViewDirNonNormalized = _WorldSpaceCameraPos.xyz - i.posWorld.xyz;
						fixed3	worldViewDir = normalize(worldViewDirNonNormalized);
						fixed	cameraDistance = length(worldViewDirNonNormalized);
						half	lerpValue = pow(saturate(cameraDistance / _ParallaxDistance), _ParallaxFalloff);
						float	nMinSamples = 6;
						half	currentSamples = lerp(_ParallaxSamples, nMinSamples, lerpValue);
						half	currentParallax = lerp(_Parallax, 0, lerpValue);

						fixed3	viewDir = mul(i.tSpace, worldViewDir);
						float2	vParallaxDirection = normalize(viewDir.xy);
						float	fLength = length(viewDir);
						float	fParallaxLength = sqrt(fLength * fLength - viewDir.z * viewDir.z) / viewDir.z;
						float2	vParallaxOffsetTS = vParallaxDirection * fParallaxLength * currentParallax;
						float	nMaxSamples = min(currentSamples, 100);
						int		nNumSamples = (int)(lerp(nMinSamples, nMaxSamples, 1 - dot(worldViewDir , i.normal)));
						float	fStepSize = 1.0 / (float)nNumSamples;
						int		nStepIndex = 0;
						float	fCurrHeight = 0.0;
						float	fPrevHeight = 1.0;
						float2	vTexOffsetPerStep = fStepSize * vParallaxOffsetTS;
						float2	vTexCurrentOffset = i.tex.xy;
						float	fCurrentBound = 1.0;
						float	fParallaxAmount = 0.0;
						float2	pt1 = 0;
						float2	pt2 = 0;
						float2	dx = ddx(i.tex.xy);
						float2	dy = ddy(i.tex.xy);
						for (nStepIndex = 0; nStepIndex < nNumSamples; nStepIndex++)
						{
							vTexCurrentOffset -= vTexOffsetPerStep;
							fCurrHeight = tex2D(_HeightMap, vTexCurrentOffset * _Tiling, dx, dy).r;
							fCurrentBound -= fStepSize;
							if (fCurrHeight > fCurrentBound)
							{
								pt1 = float2(fCurrentBound, fCurrHeight);
								pt2 = float2(fCurrentBound + fStepSize, fPrevHeight);
								fPrevHeight = fCurrHeight;
								//nStepIndex = nNumSamples + 1;   //Exit loop
								break;
							}
							else
							{
								fPrevHeight = fCurrHeight;
							}
						}
						float fDelta2 = pt2.x - pt2.y;
						float fDelta1 = pt1.x - pt1.y;
						float fDenominator = fDelta2 - fDelta1;
						if (fDenominator == 0.0f)
						{
							fParallaxAmount = 0.0f;
						}
						else
						{
							fParallaxAmount = (pt1.x * fDelta2 - pt2.x * fDelta1) / fDenominator;
						}
						i.tex.xy -= vParallaxOffsetTS * (1 - fParallaxAmount);
						i.tex.xy *= _Tiling;

						

						float shadow = SHADOW_ATTENUATION(i);
						
						float4 tex = tex2D(_MainTex, i.tex.xy) * saturate(fCurrentBound + lerp(_OcclusionMapYOffset, 1, lerpValue));
						return float4(tex.xyz * (i.Diffuse * shadow + ShadeSH9(half4(normalDirection, 1))), 1.0);
					}
					ENDCG
				}
				UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
			}
}