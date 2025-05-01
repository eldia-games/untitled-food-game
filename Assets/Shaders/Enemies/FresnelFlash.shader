Shader "Custom/FresnelFlash/StandardPBR"
{
    Properties
    {
        _MainTex     ("Albedo (RGB)", 2D)    = "white" {}
        _Metallic    ("Metallic", Range(0,1))= 0.0
        _Smoothness  ("Smoothness", Range(0,1)) = 0.5
        _FlashColor  ("Flash Color", Color)  = (1,1,1,1)
        _FlashPower  ("Flash Power", Range(0,5)) = 0

        _NoiseTex               ("Dissolve Noise", 2D)       = "white" {}
        _DissolveThreshold      ("Dissolve Threshold", Range(0,1)) = 0
        _DissolveEdgeColor      ("Edge Color", Color)        = (1,0.5,0,1)
        _DissolveEdgeWidth      ("Edge Width", Range(0,1))   = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        fixed4   _FlashColor;
        float    _FlashPower;
        half     _Metallic;
        half     _Smoothness;
        float    _DissolveThreshold;
        fixed4   _DissolveEdgeColor;
        float    _DissolveEdgeWidth;

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Base PBR
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo     = c.rgb;
            o.Metallic   = _Metallic;
            o.Smoothness = _Smoothness;

            // Flash emission (fresnel)
            float NdotV   = saturate(dot(normalize(IN.viewDir), o.Normal));
            float fresnel = pow(1 - NdotV, 3);
            o.Emission    = _FlashColor.rgb * fresnel * _FlashPower;

            if (_DissolveThreshold > 0.0) {
                float noise = 1.0f - sqrt(tex2D(_NoiseTex, IN.uv_MainTex).r);
                clip(noise - _DissolveThreshold);

                float edge = saturate(1.0 - abs(noise - _DissolveThreshold) / _DissolveEdgeWidth);
                o.Emission += _DissolveEdgeColor.rgb * edge;
            }
        }
        ENDCG
    }
    FallBack "Standard"
}
