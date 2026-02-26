Shader "Custom/SkyboxBlend"
{
    Properties
    {
        _Tint("Tint", Color) = (1,1,1,1)
        _Exposure("Exposure", Range(0, 8)) = 1

        _CubeA("Sky A (Desert)", Cube) = "" {}
        _CubeB("Sky B (Galaxy)", Cube) = "" {}

        _Blend("Blend (0=A, 1=B)", Range(0,1)) = 0

        _RotationA("Rotation A (Degrees)", Range(0,360)) = 0
        _RotationB("Rotation B (Degrees)", Range(0,360)) = 0

        _GalaxyRotationSpeed("Galaxy Rotation Speed", Range(-5,5)) = 0.25
        _TwinkleStrength("Twinkle Strength", Range(0,1)) = 0.25
        _TwinkleSpeed("Twinkle Speed", Range(0,10)) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Tint;
            float _Exposure;

            samplerCUBE _CubeA;
            samplerCUBE _CubeB;

            float _Blend;
            float _RotationA;
            float _RotationB;

            float _GalaxyRotationSpeed;
            float _TwinkleStrength;
            float _TwinkleSpeed;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };

            float3 RotateY(float3 v, float degrees)
            {
                float rad = radians(degrees);
                float s = sin(rad);
                float c = cos(rad);
                float3 r;
                r.x = v.x * c + v.z * s;
                r.y = v.y;
                r.z = -v.x * s + v.z * c;
                return r;
            }

            float Hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = normalize(mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y;

                float3 dirA = RotateY(i.dir, _RotationA);

                // galaxy rotates slowly
                float rotB = _RotationB + (t * _GalaxyRotationSpeed * 30.0);
                float3 dirB = RotateY(i.dir, rotB);

                fixed3 colA = texCUBE(_CubeA, dirA).rgb;
                fixed3 colB = texCUBE(_CubeB, dirB).rgb;

                // twinkle only on galaxy
                float h = Hash(dirB * 12.345);
                float tw = (sin(t * _TwinkleSpeed + h * 6.2831) * 0.5 + 0.5);
                float twinkle = lerp(1.0, 1.0 + _TwinkleStrength, tw);
                colB *= twinkle;

                fixed3 col = lerp(colA, colB, saturate(_Blend));
                col *= _Tint.rgb;
                col *= _Exposure;

                return fixed4(col, 1);
            }
            ENDCG
        }
    }

    Fallback Off
}
