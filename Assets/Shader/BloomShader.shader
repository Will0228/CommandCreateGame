Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // ポストエフェクト用の設定
        Cull Off ZWrite Off Ztest Always
//        Tags {
//            "RenderType"="Opaque"
//            "RenderPipeline"="UniversalPipeline"
//            "Queue" = "Geometry"
//        }
//        LOD 100
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        TEXTURE2D(_Tmp);
        SAMPLER(sampler_Tmp);

        CBUFFER_START(UnityPerMaterial)
            float _Strength;
            float _Blur;
            float _Threshold;
            float4 _MainTex_TexelSize; // x: 1/width, y: 1/height
        CBUFFER_END

        Varyings vert(Attributes input)
        {
            Varyings output;
            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            output.uv = input.uv;
            return output;
        }

        // Pass 0: 明るい部分の抽出
        float4 frag0(Varyings i) : SV_Target
        {
            float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            
            // 輝度の計算 (より正確な係数を使用)
            float bright = dot(col.rgb, float3(0.2126, 0.7152, 0.0722));
            
            // しきい値判定 (step関数の代わりに、滑らかに抽出するなら max(0, bright - _Threshold) もあり)
            float mask = step(_Threshold, bright);
            
            return col * mask * _Strength;
        }

        // Pass 1: ぼかしと合成
        float4 fragBlur(Varyings i) : SV_Target
        {
            float2 texelSize = _MainTex_TexelSize.xy;
            float3 blurResult = float3(0, 0, 0);
            float count = 0;

            // 二重ループによるボックスブラー
            // 注意: _Blurが大きすぎると非常に重くなるため、実用上は5〜9程度が限界です
            int blurRange = (int)_Blur;
            int halfBlur = blurRange / 2;

            for (int x = 0; x < blurRange; x++)
            {
                for (int y = 0; y < blurRange; y++)
                {
                    float2 offset = float2(x - halfBlur, y - halfBlur) * texelSize;
                    blurResult += SAMPLE_TEXTURE2D(_Tmp, sampler_Tmp, i.uv + offset).rgb;
                    count++;
                }
            }

            blurResult /= max(1.0, count);

            // 元の画像とぼかした光を合成
            float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            return float4(baseCol.rgb + blurResult, baseCol.a);
        }
        ENDHLSL

        // Pass 0: Brightness Extraction
        Pass
        {
            Name "BloomThreshold"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag0
            ENDHLSL
        }

        // Pass 1: Blur and Combine
        Pass
        {
            Name "BloomBlurCombine"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragBlur
            ENDHLSL
        }
    }
}
