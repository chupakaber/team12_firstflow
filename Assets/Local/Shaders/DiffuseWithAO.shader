Shader "Team12/DiffuseWithAO" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AmbientOcclusionTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
        _AOBrightness ("AO Brightness", Range(0, 3)) = 0.1
        _AOPreMultiply ("AO Pre Multiply", Range(0, 3)) = 1.5
        //_PreMultiply ("Pre Multiply", Range(0.5, 2)) = 1
        _Multiply ("Multiply", Range(0.5, 2)) = 1
        //_PreBrightness ("Pre Brightness", Range(-3, 3)) = 0
        _Brightness ("Brightness", Range(-3, 3)) = 0
        //_PreIntensity ("Pre Intencity", Range(-5, 5)) = 2.5
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
        _FogColor ("Fog Color", Color) = (0.82, 0.82, 0.82, 0)
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
                float4 fog : TEXCOORD2;
            };

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _AmbientOcclusionTex;
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
            uniform float4 _CameraWorldPosition;
            uniform float _LambertIntensity;
            uniform float _LambertOffset;
            uniform float4 _WorldSpaceLightDirectionTest;

            vertexOutput vert(vertexInput i) 
            {
                vertexOutput o;

                o.uv0 = i.uv0;
                o.uv1 = i.uv1;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                //float3 normalDirection = normalize(mul(float4(i.normal, 0.0), modelMatrixInverse).xyz);
                float3 normalDirection = UnityObjectToWorldNormal(i.normal);
                float3 lightDirection = normalize(_WorldSpaceLightDirectionTest.xyz);

                float3 diffuseReflection = (_LambertOffset + max(0.0, dot(normalDirection, lightDirection)) * _LambertIntensity);

                o.col = float4(diffuseReflection, 1.0);
                o.pos = UnityObjectToClipPos(i.vertex);
                
                float4 worldPosition = mul(unity_ObjectToWorld, i.vertex);
                half fogValue = length(_CameraWorldPosition - worldPosition);
                fogValue = max(pow((fogValue + _FogStart) * _FogScale, _FogPower), (-worldPosition.y + _FogHeight) * _FogHeightScale);
                fogValue = clamp(fogValue, 0, 1);
                o.fog = float4(_FogColor.rgb, _FogColor.a * fogValue);
                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                half4 color = tex2D(_MainTex, (i.uv0 + _MainTex_ST.zw) * _MainTex_ST.xy);
                half4 ao = tex2D(_AmbientOcclusionTex, i.uv1);
                half saturation = (ao.r - _SaturationBase) * _AOSaturation + _Saturation;
                ao = ao * _AOPreMultiply + _AOBrightness;
                color *= i.col;
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
                half shadowFactor = clamp((_SaturationBase - ao) / _SaturationBase, 0, 1);
                color += (1 - shadowFactor) * _LightColor;
                color += shadowFactor * _ShadowColor;
                color.rgb = color.rgb * (1 - i.fog.a) + i.fog.rgb * i.fog.a;
                return color;
            }

            ENDCG
        }
    }
}