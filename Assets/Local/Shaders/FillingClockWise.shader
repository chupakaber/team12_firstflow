Shader "Team12/Unlit/FillingClockWise"
{
    Properties
    {

        _Value ("Value", Float) = -100

        _BaseColor("BaseColor", Color) = (.25, .5, .5, 1)

        _FillColor("FillColor", Color) = (.5, .5, .5, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float value : TEXCOORD1;
            };

            float _Value;
            float4 _BaseColor;
            float4 _FillColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv*2 -1;
                float deltaTime = (_Time.y - _Value);
                o.value = deltaTime * 3.14 - 1.57 - max(0, deltaTime - 1) * 1000;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float sY = i.uv.y / abs(i.uv.y);
                float g = pow((i.uv.x*i.uv.x + i.uv.y*i.uv.y),0.5);
                float angle = asin((i.uv.x/g/2 + 0.5) * sY);
                float a = clamp((i.value - angle) * 1000000,0,1);
                fixed4 col = _BaseColor * (1 - a) + _FillColor * a;
                return col;
            }
            ENDCG
        }
    }
}
