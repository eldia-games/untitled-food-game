Shader "Custom/MagicOrb"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0.2,0.1,0.8,1)
        _RimColor ("Rim Color", Color) = (0.8,0.5,1,1)
        _RimPower ("Rim Power", Range(1,8)) = 3
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 2.0
        _NoiseSpeed ("Noise Speed", Float) = 0.5
        _EmissionIntensity ("Emission Intensity", Range(0,5)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos  : TEXCOORD1;
                float2 uv        : TEXCOORD2;
            };

            sampler2D _NoiseTex;
            float4 _MainColor;
            float4 _RimColor;
            float  _RimPower;
            float  _NoiseScale;
            float  _NoiseSpeed;
            float  _EmissionIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                // usa el built-in _Time.y para animación
                o.uv = v.uv * _NoiseScale + _Time.y * _NoiseSpeed;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Fresnel rim glow
                float fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower);

                // Ruido interno
                float noise = tex2D(_NoiseTex, i.uv).r;
                fixed4 baseCol = lerp(_MainColor, _RimColor, noise);

                // Combina color base con rim
                fixed4 col = baseCol + _RimColor * fresnel;

                // Emisión
                col.rgb *= _EmissionIntensity;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
