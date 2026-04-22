Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _BaseColor("Base Color (B)", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        
        [Header(Rim Light Settings)]
        _RimColor("Rim Color (A)", Color) = (1,0,0,1)
        _RimPower("Rim Power", Range(0.1, 8.0)) = 3.0
    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv: TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _RimColor;
                float _RimPower;
                float _Smoothness;
                float _Metallic;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings o;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                o.positionCS = posInputs.positionCS;
                o.uv = input.uv;
                o.normalWS = TransformObjectToWorldNormal(input.normalOS);
                o.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);
                return o;
            }

            float4 frag (Varyings input) : SV_Target
            {
                float3 n = normalize(input.normalWS);
                float3 v = normalize(input.viewDirWS);

                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _BaseColor;

                Light mainLight = GetMainLight();
                float3 lightDirection = mainLight.direction;
                
                float power1 = 1.0 - max(0.0, dot(n, lightDirection));
                float power2 = 1.0 - max(0.0, dot(n, v));
                float rimPower = power1 * power2;
                rimPower = pow(rimPower, _RimPower);

                float diffuse = max(0.0, dot(n, lightDirection));
                float ambientLight = 0.2;
                float3 lightColor = mainLight.color * diffuse + ambientLight;

                float3 rimColor = _RimColor.rgb * rimPower;

                float3 finalLig = lightColor + rimColor;
                float3 finalColor = texColor.rgb * finalLig;

                
                
                return float4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
}
