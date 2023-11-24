Shader "Team12/DiffuseWithAO" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AmbientOcclusionTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
        _AOBrightness ("AO Brightness", Range(0, 3)) = 0
        _AOPreMultiply ("AO Pre Multiply", Range(0, 3)) = 1
        _PreMultiply ("Pre Multiply", Range(0.5, 2)) = 1
        _Multiply ("Multiply", Range(0.5, 2)) = 1
        _PreBrightness ("Pre Brightness", Range(-3, 3)) = 0
        _Brightness ("Brightness", Range(-3, 3)) = 0
        _PreIntensity ("Pre Intencity", Range(-5, 5)) = 1
        _Intensity ("Intencity", Range(-5, 5)) = 1

        _SaturationBase ("Saturation Base", Range(0, 1)) = 0.5
        _AOSaturation ("AO Saturation", Range(-1, 10)) = 0
        _Saturation ("Saturation", Range(-1, 10)) = 0
        _LightColor ("Light Color", Color) = (0, 0, 0, 0)
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            Tags { "LightMode" = "ForwardBase" } 

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"

            struct vertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct vertexOutput {
                float4 pos : SV_POSITION;
                float4 col : COLOR;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            uniform float4 _LightColor0;
            uniform sampler2D _MainTex;
            uniform sampler2D _AmbientOcclusionTex;
            uniform float _AOBrightness;
            uniform float _AOPreMultiply;
            uniform float _SaturationBase;
            uniform float _AOSaturation;
            uniform float _Saturation;
            uniform float _Lighten;
            uniform float _PreMultiply;
            uniform float _Multiply;
            uniform float _PreBrightness;
            uniform float _Brightness;
            uniform float _PreIntensity;
            uniform float _Intensity;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;

            vertexOutput vert(vertexInput i) 
            {
                vertexOutput o;

                o.uv0 = i.uv0;
                o.uv1 = i.uv1;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                //float3 normalDirection = normalize(mul(float4(i.normal, 0.0), modelMatrixInverse).xyz);
                float3 normalDirection = UnityObjectToWorldNormal(i.normal);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

                float3 diffuseReflection = _LightColor0.rgb * (0.5 + max(0.0, dot(normalDirection, lightDirection)) * 0.5);

                o.col = float4(diffuseReflection, 1.0);
                o.pos = UnityObjectToClipPos(i.vertex);
                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                half4 color = tex2D(_MainTex, i.uv1);
                half4 ao = tex2D(_AmbientOcclusionTex, i.uv0);
                half saturation = (ao.r - _SaturationBase) * _AOSaturation + _Saturation;
                ao = ao * _AOPreMultiply + _AOBrightness;
                color *= i.col;
                color += _PreBrightness;
                color *= _PreIntensity;
                color *= ao;
                color = mul(color, _PreMultiply);
                // half minColor = min(color.r, min(color.b, color.g));
                half midColor = (color.r + color.b + color.g) * 0.333;
                color.r += (color.r - midColor) * saturation;
                color.g += (color.g - midColor) * saturation;
                color.b += (color.b - midColor) * saturation;
                color = mul(color, _Multiply);
                color *= _Intensity;
                color += _Brightness;
                half shadowFactor = clamp((_SaturationBase - ao) / _SaturationBase, 0, 1);
                color += (1 - shadowFactor) * _LightColor;
                color += shadowFactor * _ShadowColor;
                return color;
            }

            ENDCG
        }
    }
}