using System;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HeightMapEvaluatorSO", menuName = "ScriptableObjects/World/HeightMapEvaluatorSO", order = 1)]
public class HeightMapEvaluatorSO : ScriptableObject
{
    public float waterHeight = 0.0f;
    // public float roadHeight = 1.0f;
    public float landHeight = 0.5f;

    public float scale = 1.0f;
    public float lacunarity = 2.0f;
    public float persistence = 0.5f;
    public int octaves = 4;
    public float landAmplitude = 1.0f;
    public Texture2D planetTexture;

    public float roadSmoothnessSpread = 0.01f;
    public int roadSmoothnessSamplesPerAxis = 2;

    private NoiseUtils _noiseUtils;

    public HeightMapEvaluatorSO()
    {
        _noiseUtils = new NoiseUtils();
    }

    public Vector2 NormalizedPositionToUV(Vector3 normalizedPosition)
    {
        // First figure out what cube face we're on
        CubemapFace face;
        Vector2 uv;
        
        CubemapUtils.DirectionToCubemapFaceUV(normalizedPosition,
            out face,
            out uv);


        if (face == CubemapFace.PositiveZ) {
            uv = new Vector2(1.0f - uv.x, 1.0f - uv.y);
            uv /= 4.0f;
            uv += new Vector2(0.25f, 0.25f);
        }
        else if (face == CubemapFace.PositiveX) {
            uv = new Vector2(1.0f - uv.x, 1.0f - uv.y);
            uv /= 4.0f;
            uv += new Vector2(0.0f, 0.25f);
        }
        else if (face == CubemapFace.NegativeX) {
            uv = new Vector2(1.0f - uv.x, 1.0f - uv.y);
            uv /= 4.0f;
            uv += new Vector2(0.5f, 0.25f);
        }
        else if (face == CubemapFace.NegativeY) {
            uv = new Vector2(1.0f - uv.x, uv.y);
            uv /= 4.0f;
            uv += new Vector2(0.25f, 0.0f);
        }
        else if (face == CubemapFace.PositiveY) {
            uv = new Vector2(1.0f - uv.x, uv.y);
            uv /= 4.0f;
            uv += new Vector2(0.25f, 0.5f);
        }
        else if (face == CubemapFace.NegativeZ) {
            uv = new Vector2(1.0f - uv.x, 1.0f - uv.y);
            uv /= 4.0f;
            uv += new Vector2(0.75f, 0.25f);
        }
        else
            throw new Exception("WTF?");

        return uv;
    }

    public float Evaluate(Vector3 normalizedPosition)
    {
        Vector2 uv = NormalizedPositionToUV(normalizedPosition);
        Color color = planetTexture.GetPixelBilinear(uv.x, uv.y);

        float rWeight = 10.0f;

        float bPortion = color.b / (rWeight*color.r + color.g + color.b);
        float rPortion = color.r*rWeight / (rWeight*color.r + color.g + color.b);
        float gPortion = color.g / (rWeight*color.r + color.g + color.b);

        float sampledHeightSum = 0.0f;

        if (rPortion > 0.01f)
        {
            // We need to place a road. The way we do that is we smoothly sample many points around the current point
            for (int x = -roadSmoothnessSamplesPerAxis; x <= roadSmoothnessSamplesPerAxis; x++)
            {
                for (int y = -roadSmoothnessSamplesPerAxis; y <= roadSmoothnessSamplesPerAxis; y++)
                {
                    Vector3 axis1 = Vector3.up;
                    Vector3 axis2 = Vector3.Cross(normalizedPosition, axis1);
                    Vector3 samplePos = normalizedPosition + ((axis1 * x + axis2 * y) * roadSmoothnessSpread);
                    samplePos.Normalize();
                    sampledHeightSum += _noiseUtils.Fbm(
                        samplePos.x * scale,
                        samplePos.y * scale,
                        samplePos.z * scale,
                        octaves,
                        lacunarity,
                        persistence);
                }
            }
        }
        float roadHeight = (rPortion > 0.01f) ? sampledHeightSum / (float)((2 * roadSmoothnessSamplesPerAxis + 1) * (2 * roadSmoothnessSamplesPerAxis + 1)) * landAmplitude : 0.0f;

        float noise = _noiseUtils.Fbm(
            normalizedPosition.x * scale,
            normalizedPosition.y * scale,
            normalizedPosition.z * scale,
            octaves,
            lacunarity,
            persistence);

        return bPortion * waterHeight + rPortion * roadHeight + (gPortion * (landHeight + landAmplitude * noise));
    }

    public bool IsRoadAtPoint(Vector3 normalizedPosition)
    {
        Vector2 uv = NormalizedPositionToUV(normalizedPosition);
        Color color = planetTexture.GetPixelBilinear(uv.x, uv.y);

        float rWeight = 10.0f;

        float rPortion = color.r * rWeight / (rWeight * color.r + color.g + color.b);

        return (rPortion > 0.9f);
    }

}