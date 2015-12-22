Shader "Deform" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _DeformCenter("Deform Center", Float) = 0
        _DeformAxis("Deform Axis", Int) = 0
        _PerpAxis("Perpendicular Axis", Int) = 1
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
    
    float _Freq, _Amplitude, _Deform, _DeformCenter;
    int _DeformAxis, _PerpAxis;
    float4 _Color;

    void vert(inout appdata_full v) {
        float perpAxis;
        float deformCenter = mul(_Object2World, float4(0, 0, 0, 1)).z;
        if (_PerpAxis == 0)
        {
            perpAxis = v.vertex.x;
        }
        else if (_PerpAxis == 1)
        {
            perpAxis = v.vertex.y;
        }
        else if (_PerpAxis == 2)
        {
            perpAxis = v.vertex.z;
        }

        if (_DeformAxis == 0)
        {
            v.vertex.x = v.vertex.x * (1 - _Deform) + _Deform * (_DeformCenter + _Amplitude * sin(perpAxis * _Freq * 20) * 10);
        }
        else if (_DeformAxis == 1)
        {
            v.vertex.y = v.vertex.y * (1 - _Deform) + _Deform * (_DeformCenter + _Amplitude * sin(perpAxis * _Freq * 20) * 10);
        }
        else if (_DeformAxis == 2)
        {
            v.vertex.z = v.vertex.z * (1 - _Deform) + _Deform * (_DeformCenter + _Amplitude * sin(perpAxis * _Freq * 20) * 10);
        }
    }
    sampler2D _MainTex;
    void surf(Input IN, inout SurfaceOutput o) {
        o.Albedo = _Color;// tex2D(_MainTex, IN.uv_MainTex).rgb;
    }
    ENDCG
    }
        Fallback "Diffuse"
}