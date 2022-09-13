Shader "Unlit/Healthbar"
{
    Properties {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Health ("Health", Range(0,1)) = 1
    }

    SubShader {
        Tags { 
            "RenderType"="Transparent"
            "RenderPipeline" = "UniversalRenderPipeline"
            "Queue"="Transparent"
        }

        Pass {
            // Dooesn't write to depth buffer
            ZWrite Off

            // src * X + dst * Y base blending
            // src * srcAlpha + dst * (1-srcAlpha)
            Blend SrcAlpha oneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Health;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp(float a, float b, float v) {
                return (v-a)/(b-a);
            }

            float4 frag (Interpolators i) : SV_Target {
                // float4 col = tex2D(_MainTex, i.uv);


                float threshold = saturate(InverseLerp(0.2, 0.8, _Health));

                float3 healthBarColor = lerp(float3(1,0,0), float3(0,1,0), threshold);
                float healthBarMask = _Health > i.uv.x;

                // Removes the black pixels (under 0 in value)
                // clip(healthBarMask - 0.001);


                return float4(healthBarColor, healthBarMask);
                // return healthBarMask;
            }
            ENDHLSL
        }
    }
}
