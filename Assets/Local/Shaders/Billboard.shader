// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Team12/Unlit/Billboard"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ZOffset ("Z Offset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ZOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 ori = mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1));
                float4 vt = v.vertex;
                vt.y = vt.z;
                vt.z = _ZOffset;
                vt.xyz += ori.xyz; //result is vt.z==ori.z ,so the distance to camera keeped ,and screen size keeped
                o.vertex = mul(UNITY_MATRIX_P, vt);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
