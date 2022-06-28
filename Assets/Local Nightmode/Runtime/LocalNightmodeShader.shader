Shader "LocalNightmodeShader"
{
    Properties
    {
        _RenderTexture ("RenderTexture", 2D) = "black" {}
        _Strength ("Strength", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent+500"}

        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always
            Blend One OneMinusSrcAlpha, Zero One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Strength;
            Texture2D _RenderTexture;

            struct appdata
            {
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata i)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                if(_ScreenParams.x != 1 && _RenderTexture[uint2(0,0)].r!=1.0) return o;

                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = float4(i.uv*2.0-1.0,0.0,1.0);
                return o;
            }

            float4 frag() : SV_Target
            {
                return _ScreenParams.x == 1 ? (1.0).rrrr : float4(0,0,0,_Strength);
            }
            ENDHLSL
        }
    }
}