Shader "NoiseBall/Surface"
{
    Properties
    {
        _MainTex("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("", Color) = (1,1,1,1)
        _Glossiness ("gloss", Range(0, 1)) = 0.5
        _Metallic ("metallic", Range(0, 1)) = 0.5
        _Radius("radius", Float) = 1
        _NoiseAmplitude("amplitude", Float) = 0.5
        _NoiseFrequency("freq", Float) = 0.5
        _NoiseOffset("offset", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0

        #include "Common.cginc"

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void vert(inout appdata_full v)
        {
            if (_NoiseAmplitude > 0)
            {
                float3 v1 = displace(v.vertex.xyz);
                float3 v2 = displace(v.texcoord.xyz);
                float3 v3 = displace(v.texcoord1.xyz);
                v.vertex.xyz = v1;
                v.normal = normalize(cross(v2 - v1, v3 - v1));
            }
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            if(_Color.a > 0)
                o.Albedo = _Color;
            else
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
