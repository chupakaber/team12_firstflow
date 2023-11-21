Shader "Team12/VertexColorWithAO" {
    Properties {
        _AmbientOcclusionTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
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
                float4 color : COLOR;
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
            uniform sampler2D _AmbientOcclusionTex;

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

                float3 diffuseReflection = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));

                o.col = i.color * float4(diffuseReflection, 1.0);
                o.pos = UnityObjectToClipPos(i.vertex);
                return o;
            }

            float4 frag(vertexOutput i) : COLOR
            {
                half4 color = tex2D(_AmbientOcclusionTex, i.uv1) * i.col;
                return color;
            }

            ENDCG
        }
    }
}