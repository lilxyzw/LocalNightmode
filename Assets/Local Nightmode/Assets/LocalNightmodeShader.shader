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
            Name "RENDERTEXTURE"
            Cull Off
            ZWrite Off
            ZTest Always
            Blend One Zero
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4x4 unity_ObjectToWorld;
            float4x4 unity_MatrixVP;
            float4 _ScreenParams;

            float4 vert(float4 positionOS : POSITION) : SV_POSITION
            {
                if(_ScreenParams.x != 1 || _ScreenParams.y != 1) return 0.0;
                return mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(positionOS.xyz, 1.0)));
            }

            float4 frag() : SV_Target
            {
                return 1.0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "MULTIPLY"
            Cull Off
            ZWrite Off
            ZTest Always
            Blend Zero SrcColor, Zero One
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))
                #define LIL_SAMPLE_2D(tex,samp,uv)              tex.Sample(samp,uv)
                #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)      tex.SampleLevel(sampler_linear_repeat,uv,lod)
                #if !defined TEXTURE2D
                    #define TEXTURE2D(tex)                      Texture2D tex
                #endif
                #if !defined SAMPLER
                    #define SAMPLER(tex)                        SamplerState tex
                #endif
            #else
                #define LIL_SAMPLE_2D(tex,samp,uv)              tex2D(tex,uv)
                #define LIL_SAMPLE_2D_LOD(tex,samp,uv)          tex2Dlod(tex,float4(uv,0,lod))
                #if !defined TEXTURE2D
                    #define TEXTURE2D(tex)                      sampler2D tex
                #endif
                #if !defined SAMPLER
                    #define SAMPLER(tex)
                #endif
            #endif

            float4 _ScreenParams;
            float _Strength;
            TEXTURE2D(_RenderTexture);
            SAMPLER(sampler_linear_repeat);

            float4 vert(float2 uv : TEXCOORD0) : SV_POSITION
            {
                if(_ScreenParams.x == 1 || _ScreenParams.y == 1) return 0.0;
                float renderCol = LIL_SAMPLE_2D_LOD(_RenderTexture,sampler_RenderTexture,0,0).r;
                if(renderCol!=1.0) return 0.0;
                return float4(uv*2.0-1.0,0.0,1.0);
            }

            float4 frag() : SV_Target
            {
                return 1.0-_Strength;
            }
            ENDHLSL
        }
    }
}
