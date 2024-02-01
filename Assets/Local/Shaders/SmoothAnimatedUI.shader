Shader "Team12/UI/SmoothAnimated"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Speed ("Animation Speed", Range(0.01, 100)) = 1
        _Tiling ("Tiling (frames)", Vector) = (0, 0, 0, 0)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        LOD 100

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float frameDelta : TEXCOORD2;
                float4 worldPosition : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float4 _Tiling;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPosition = v.vertex;

                float rawFrame = _Time.y * _Speed;
                float frame = floor(rawFrame);
                float frameDelta = rawFrame - frame;
                o.frameDelta = frameDelta;
                
                frame = frame - floor(frame / _Tiling.x) * _Tiling.x;
                float frameY = floor(frame * _MainTex_ST.xy);
                float frameX = frame - frameY / _MainTex_ST.xy;

                o.uv1 = (v.uv + float2(frameX, frameY)) * _MainTex_ST.xy;


                frame = floor(rawFrame) - 1;
                frame = frame - floor(frame / _Tiling.x) * _Tiling.x;
                frameY = floor(frame * _MainTex_ST.xy);
                frameX = frame - frameY / _MainTex_ST.xy;

                o.uv2 = (v.uv + float2(frameX, frameY)) * _MainTex_ST.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv1);
                fixed4 colPrev = tex2D(_MainTex, i.uv2);
                col = colPrev * (1 - i.frameDelta) + col * i.frameDelta;

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif

                return col;
            }
            ENDCG
        }
    }
}
