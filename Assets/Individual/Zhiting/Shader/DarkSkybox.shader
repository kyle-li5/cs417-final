// Lacuna - Dark grey gradient skybox with slow drifting fog clouds
// Usage: Create Material -> Shader: Skybox/Lacuna Dark Grey
//        Window > Rendering > Lighting > Environment > Skybox Material
Shader "Skybox/Lacuna Dark Grey"
{
    Properties
    {
        [Header(Gradient)]
        _TopColor      ("Top Color", Color)     = (0.02, 0.02, 0.025, 1)
        _HorizonColor  ("Horizon Color", Color) = (0.32, 0.32, 0.34, 1)
        _BottomColor   ("Bottom Color", Color)  = (0.05, 0.05, 0.055, 1)
        _TopExponent   ("Top Falloff", Range(0.1, 8))    = 1.6
        _BottomExponent("Bottom Falloff", Range(0.1, 8)) = 2.5
        _HorizonGlow   ("Horizon Glow Width", Range(1, 64)) = 10
        _HorizonGlowStrength ("Horizon Glow Strength", Range(0, 1)) = 0.25

        [Header(Clouds)]
        _CloudColor    ("Cloud Color", Color) = (0.45, 0.45, 0.47, 1)
        _CloudScale    ("Cloud Scale", Range(0.1, 10)) = 2.2
        _CloudSpeed    ("Cloud Speed", Range(0, 0.2))  = 0.01
        _CloudCoverage ("Cloud Coverage", Range(0, 1)) = 0.5
        _CloudSoftness ("Cloud Softness", Range(0.01, 1)) = 0.35
        _CloudOpacity  ("Cloud Opacity", Range(0, 1)) = 0.55

        [Header(Overall)]
        _Exposure      ("Exposure", Range(0, 3)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TopColor, _HorizonColor, _BottomColor, _CloudColor;
            float _TopExponent, _BottomExponent, _HorizonGlow, _HorizonGlowStrength;
            float _CloudScale, _CloudSpeed, _CloudCoverage, _CloudSoftness, _CloudOpacity;
            float _Exposure;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // single-pass stereo for VR
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            // --- value noise + fbm ---
            float hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0;
                float amp = 0.5;
                for (int k = 0; k < 5; k++)
                {
                    v += amp * noise(p);
                    p *= 2.03;
                    amp *= 0.5;
                }
                return v;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 d = normalize(i.dir);
                float y = d.y;

                // vertical gradient
                float up   = pow(saturate(y), 1.0 / _TopExponent);
                float down = pow(saturate(-y), 1.0 / _BottomExponent);
                float3 col = _HorizonColor.rgb;
                col = lerp(col, _TopColor.rgb, up);
                col = lerp(col, _BottomColor.rgb, down);

                // soft brighter band at the horizon
                float glow = exp(-abs(y) * _HorizonGlow);
                col += _HorizonColor.rgb * glow * _HorizonGlowStrength;

                // clouds projected onto a dome above the horizon
                if (y > 0.0)
                {
                    float2 uv = d.xz / (y + 0.15) * _CloudScale * 0.25;
                    float t = _Time.y * _CloudSpeed;
                    float n = fbm(uv + float2(t, t * 0.6));
                    n = fbm(uv + n * 0.8 - float2(t * 0.5, 0)); // domain warp for wispy look
                    float c = smoothstep(1.0 - _CloudCoverage, 1.0 - _CloudCoverage + _CloudSoftness, n);
                    c *= saturate(y * 6.0);        // fade out near horizon
                    c *= 1.0 - saturate(y * 0.6);  // thinner straight overhead
                    col = lerp(col, _CloudColor.rgb, c * _CloudOpacity);
                }

                return fixed4(col * _Exposure, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
