Shader "IronExiles/ProceduralStarfield"
{
    Properties
    {
        _StarDensity ("Star Density", Range(5, 80)) = 40
        _StarBrightness ("Star Brightness", Range(1, 5)) = 2.5
        _Twinkle ("Twinkle Speed", Range(0, 2)) = 0.5
        _NebulaColor1 ("Nebula Color 1", Color) = (0.06, 0.02, 0.12, 1)
        _NebulaColor2 ("Nebula Color 2", Color) = (0.02, 0.06, 0.1, 1)
        _NebulaIntensity ("Nebula Intensity", Range(0, 0.2)) = 0.06
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Background"
            "Queue" = "Background"
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType" = "Skybox"
        }

        Pass
        {
            Name "Starfield"
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            float _StarDensity;
            float _StarBrightness;
            float _Twinkle;
            float4 _NebulaColor1;
            float4 _NebulaColor2;
            float _NebulaIntensity;

            float2 hash22(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            float hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float hash13(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.zyx + 31.32);
                return frac((p.x + p.y) * p.z);
            }

            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                return lerp(
                    lerp(lerp(hash13(i), hash13(i + float3(1,0,0)), f.x),
                         lerp(hash13(i + float3(0,1,0)), hash13(i + float3(1,1,0)), f.x), f.y),
                    lerp(lerp(hash13(i + float3(0,0,1)), hash13(i + float3(1,0,1)), f.x),
                         lerp(hash13(i + float3(0,1,1)), hash13(i + float3(1,1,1)), f.x), f.y),
                    f.z);
            }

            float starLayer(float3 dir, float scale)
            {
                float3 p = dir * scale;
                float3 cell = floor(p);
                float3 local = frac(p) - 0.5;

                float star = 0.0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            float3 offset = float3(x, y, z);
                            float3 id = cell + offset;

                            float rand = hash13(id);
                            if (rand > 0.4) continue;

                            float3 starPos = float3(hash13(id + 100.0), hash13(id + 200.0), hash13(id + 300.0)) - 0.5;
                            float3 diff = offset + starPos - local;
                            float dist = dot(diff, diff);

                            float brightness = pow(hash13(id + 500.0), 0.5) * _StarBrightness;
                            float size = 0.06 + hash13(id + 400.0) * 0.04;
                            float falloff = exp(-dist / (size * size));

                            star += falloff * brightness;
                        }
                    }
                }

                return star;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.viewDir = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 dir = normalize(input.viewDir);

                float stars = starLayer(dir, _StarDensity);
                stars += starLayer(dir, _StarDensity * 0.5) * 0.6;
                stars += starLayer(dir, _StarDensity * 2.0) * 0.3;

                float3 warmTint = float3(1.0, 0.85, 0.7);
                float3 coolTint = float3(0.7, 0.85, 1.0);
                float3 neutralTint = float3(1.0, 1.0, 1.0);
                float tintRand = hash13(floor(dir * _StarDensity) + 777.0);
                float3 starColor = tintRand < 0.33 ? warmTint : (tintRand < 0.66 ? coolTint : neutralTint);

                float n1 = noise(dir * 3.0);
                float n2 = noise(dir * 5.0 + 100.0);
                float nebula = n1 * n2;
                float3 nebulaColor = lerp(_NebulaColor1.rgb, _NebulaColor2.rgb, noise(dir * 2.0 + 50.0));
                nebulaColor *= nebula * _NebulaIntensity;

                float3 color = stars * starColor + nebulaColor;

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
