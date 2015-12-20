Shader "Deform" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Freq("Freq Amount", Range(0,10)) = 1
        _Amplitude("Amplitude Amount", Range(0,0.01)) = 0
        _Deform("Deform Amount", Range(0,1)) = 0.5
    }
        SubShader{
        Tags{ "RenderType" = "Opaque" }
        CGPROGRAM
#pragma surface surf Lambert vertex:vert
    struct Input {
        float2 uv_MainTex;
    };
    float _Freq, _Amplitude, _Deform;
    void vert(inout appdata_full v) {
        v.vertex.x = v.vertex.x * (1 - _Deform) + _Deform * _Amplitude * sin(v.vertex.y * _Freq * 20) * 10;
    }
    sampler2D _MainTex;
    void surf(Input IN, inout SurfaceOutput o) {
        o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
    }
    ENDCG
    }
        Fallback "Diffuse"
}