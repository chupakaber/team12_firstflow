Shader "Team12/DiffuseWithLightmap" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AOBrightness ("AO Brightness", Range(0, 3)) = 0.1
        _AOPreMultiply ("AO Pre Multiply", Range(0, 3)) = 1.5
        // _PreMultiply ("Pre Multiply", Range(0.5, 2)) = 1
        _Multiply ("Multiply", Range(0.5, 2)) = 1
        // _PreBrightness ("Pre Brightness", Range(-3, 3)) = 0
        _Brightness ("Brightness", Range(-3, 3)) = 0
        // _PreIntensity ("Pre Intencity", Range(-5, 5)) = 2.5
        _Intensity ("Intencity", Range(-5, 5)) = 1
        _LambertIntensity ("_LambertIntensity", Range(0, 3)) = 1
        _LambertOffset ("_LambertOffset", Range(-5, 5)) = 0

        _SaturationBase ("Saturation Base", Range(0, 1)) = 0.6
        _AOSaturation ("AO Saturation", Range(-1, 10)) = 3
        _Saturation ("Saturation", Range(-1, 10)) = 1.8
        _LightColor ("Light Color", Color) = (0, 0, 0, 0)
        _ShadowColor ("Shadow Color", Color) = (0.07, 0, 0.21, 1)
        _FogStart ("Fog Start", Range(-100, 100)) = -14
        _FogScale ("Fog Scale", Range(0.001, 100)) = 0.04
        _FogPower ("Fog Power", Range(0.5, 2)) = 2
        _FogHeight ("Fog Height", Range(-10, 10)) = 0.5
        _FogHeightScale ("Fog Height Scale", Range(0.01, 10)) = 0.15
        _FogColor ("Fog Color", Color) = (0.82, 0.82, 0.82, 1)
        _FogColor2 ("Fog Color 2", Color) = (0.82, 0.82, 0.82, 1)

        _GIAlbedoColor ("Color Albedo (GI)", Color)=(1,1,1,1)
        _GIAlbedoTex ("Albedo (GI)",2D)="white"{}
    }

    SubShader {
        LOD 100
        Tags { "LightMode" = "ForwardBase" "RenderType" = "Opaque" } 

        Pass {

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 
            #pragma LIGHTMAP_ON Standard

            #include "UnityCG.cginc"

            struct vertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
            };

            struct vertexOutput {
                float4 pos : SV_POSITION;
                float4 col : COLOR;
                float2 uv0 : TEXCOORD0;
                float4 lightmapUV : TEXCOORD1;
                float4 fog : TEXCOORD2;
                float4 worldPosition : TEXCOORD3;
            };

            uniform sampler2D _MainTex;
            uniform float _AOBrightness;
            uniform float _AOPreMultiply;
            uniform float _SaturationBase;
            uniform float _AOSaturation;
            uniform float _Saturation;
            uniform float _Lighten;
            // uniform float _PreMultiply;
            uniform float _Multiply;
            // uniform float _PreBrightness;
            uniform float _Brightness;
            // uniform float _PreIntensity;
            uniform float _Intensity;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;
            uniform float _FogStart;
            uniform float _FogScale;
            uniform float _FogPower;
            uniform float _FogHeight;
            uniform float _FogHeightScale;
            uniform float4 _FogColor;
            uniform float4 _FogColor2;
            uniform float4 _CameraWorldPosition;
            uniform float _LambertIntensity;
            uniform float _LambertOffset;
            uniform float4 _WorldSpaceLightDirectionTest;

            vertexOutput vert(vertexInput i) 
            {
                vertexOutput o;

                o.uv0 = i.uv0;

                o.lightmapUV.xy = i.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.lightmapUV.zw = 0;

                float4x4 modelMatrix = UNITY_MATRIX_M;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                //float3 normalDirection = normalize(mul(float4(i.normal, 0.0), modelMatrixInverse).xyz);
                float3 normalDirection = UnityObjectToWorldNormal(i.normal);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

                float3 diffuseReflection = (_LambertOffset + max(0.0, dot(normalDirection, lightDirection)) * _LambertIntensity);

                o.col = i.color * float4(diffuseReflection, 1.0);
                o.pos = UnityObjectToClipPos(i.vertex);
                
                float4 worldPosition = mul(modelMatrix, i.vertex);
                o.worldPosition = worldPosition;
                half fogValue = length(_CameraWorldPosition - worldPosition);
                half fogValue1 = clamp(pow(max(0, (fogValue + _FogStart) * _FogScale), _FogPower), 0, 1);
                half fogValue2 = clamp((-worldPosition.y + _FogHeight) * _FogHeightScale, 0, 1);
                fogValue1 = fogValue1 * (1 - clamp(fogValue2, 0, 1));
                fogValue = max(fogValue1, fogValue2);
                fogValue1 = fogValue1 / max(fogValue1 + fogValue2, 0.01);
                fogValue2 = 1 - fogValue1;
                o.fog = float4(_FogColor.rgb * fogValue1 + _FogColor2.rgb * fogValue2, _FogColor.a * fogValue);
                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                half4 color = tex2D(_MainTex, i.uv0) * i.col;
                
                half4 bakedLighmap = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV.xy);
                half3 decodedLightmap = DecodeLightmap(bakedLighmap);
                half ao = (decodedLightmap.r + decodedLightmap.g + decodedLightmap.b) / 3;
                half saturationBaseDiff = ao - _SaturationBase;
                half saturation = saturationBaseDiff * _AOSaturation + _Saturation;
                ao = ao * _AOPreMultiply + _AOBrightness;
                // color += _PreBrightness;
                // color *= _PreIntensity;
                color *= ao;
                // color = mul(color, _PreMultiply);
                // half minColor = min(color.r, min(color.b, color.g));
                half midColor = (color.r + color.b + color.g) * 0.333;
                color.r += (color.r - midColor) * saturation;
                color.g += (color.g - midColor) * saturation;
                color.b += (color.b - midColor) * saturation;
                color = mul(color, _Multiply);
                color *= _Intensity;
                color += _Brightness;
                half shadowFactor = clamp(-saturationBaseDiff / _SaturationBase, 0, 1);
                color += (1 - shadowFactor) * _LightColor;
                color += shadowFactor * _ShadowColor;

                //half fogValue = length(_CameraWorldPosition - i.worldPosition);
                //fogValue = max(pow(max(0, (fogValue + _FogStart) * _FogScale), _FogPower), (-i.worldPosition.y + _FogHeight) * _FogHeightScale);
                //fogValue = clamp(fogValue, 0, 1);
                //i.fog = float4(_FogColor.rgb, _FogColor.a * fogValue);

                color.rgb = color.rgb * (1 - i.fog.a) + i.fog.rgb * i.fog.a;
                return color;
            }

            ENDCG
        }
    }
}