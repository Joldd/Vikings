Shader "Custom/WaterShader"
{
    Properties
    {
        _Color("Color", Color) = (0.0, 0.5, 0.7, 0.5)
        _Transparency("Transparency", Range(0, 1)) = 0.5
        _NormalMap("Normal Map", 2D) = "bump" {}
    }
        SubShader
        {
            Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _NormalMap;
        fixed4 _Color;
        half _Transparency;

        struct Input
        {
            float2 uv_NormalMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 normalTex = tex2D(_NormalMap, IN.uv_NormalMap);
            o.Normal = UnpackNormal(normalTex);
            o.Albedo = _Color.rgb;
            o.Alpha = _Transparency;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
