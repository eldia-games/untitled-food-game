Shader "Custom/SeamlessDistortThenGradient"
{
    Properties
    {
        _MainTex           ("Textura (tileable)", 2D)     = "white" {}
        _ScrollSpeed       ("Velocidad Scroll (X,Y)", Vector) = (0.05, 0.02, 0, 0)
        _DistortAmplitude  ("Amplitud Distorsión", Float)    = 0.02
        _DistortFrequency  ("Frecuencia Distorsión", Float)  = 2
        _DistortSpeed      ("Velocidad Distorsión", Float)   = 1
        _ColorBottom       ("Color Inferior", Color)         = (0, 0, 0, 1)
        _ColorTop          ("Color Superior", Color)         = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Off
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            static const float PI = 3.14159265;

            sampler2D _MainTex;
            float2   _ScrollSpeed;
            float    _DistortAmplitude;
            float    _DistortFrequency;
            float    _DistortSpeed;
            fixed4   _ColorBottom;
            fixed4   _ColorTop;
            //float4   _Time;  // .y = segundos desde inicio

            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos       : SV_POSITION;
                float2 uvScreen  : TEXCOORD0; // Para el degradado
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.uvScreen = o.pos.xy / o.pos.w; // normalizado 0..1
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1) Scroll en espacio pantalla
                float2 baseUV = i.uvScreen + _ScrollSpeed * _Time.y;

                // 2) Distorsión periódica (sin costuras)
                float wave    = _Time.y * _DistortSpeed;
                float freqRad = _DistortFrequency * 2.0 * PI;
                float dx      = sin(baseUV.y * freqRad + wave) * _DistortAmplitude;
                float dy      = cos(baseUV.x * freqRad + wave) * _DistortAmplitude;
                float2 uvDist = baseUV + float2(dx, dy);

                // 3) Tile sin costuras
                float2 uv = frac(uvDist);

                // 4) Muestreo de la textura distorsionada
                fixed4 texColor = tex2D(_MainTex, uv);

                // 5) Ahora calculamos un degradado vertical puro
                //    basado en la posición de pantalla, para tintar el resultado:
                float t = saturate(i.uvScreen.y);
                fixed4 gradColor = lerp(_ColorBottom, _ColorTop, t);

                // 6) Combinación final: textura distorsionada + degradado
                return texColor * gradColor;
            }
            ENDCG
        }
    }
}
