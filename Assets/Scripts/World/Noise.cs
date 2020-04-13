﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise  {

    public static float[,,] GenerateNoiseMap(int seed, int chunkSize, Vector3 offset, float noiseScale, float noiseWeight, int octaves, float persistance, float lacunarity) {
        System.Random prng = new System.Random(seed);

        Vector3[] octaveOffsets = new Vector3[octaves];
        for (int i = 0; i < octaves; i++) { //also offset each level of noise
            octaveOffsets[i] = new Vector3(prng.Next(-100000,100000)+offset.x,prng.Next(-100000,100000)+offset.y,prng.Next(-100000,100000)+offset.z);
        }

        float[,,] noiseMap = new float[chunkSize + 1, chunkSize + 1, chunkSize + 1];

        float minNoise = float.MaxValue;
        float maxNoise = float.MinValue;

        for (int z = 0; z < chunkSize + 1; z++) {
            for (int y = 0; y < chunkSize + 1; y++) {
                for (int x = 0; x < chunkSize + 1; x++) {

                    float amp = 1;
                    float freq = 1;
                    float noise = 0;

                    for (int i = 0; i < octaves; i++) {
                        float px = x / noiseScale * freq + octaveOffsets[i].x;
                        float py = y / noiseScale * freq + octaveOffsets[i].y;
                        float pz = z / noiseScale * freq + octaveOffsets[i].z;
                        float perlinValue = Perlin3D(px, py, pz);
                        noise += perlinValue*amp;

                        //modify freq and amp per pass
                        amp *= persistance;
                        freq *= lacunarity;
                    }
                    float finalNoise = y + noise * noiseWeight;

                    noiseMap[z, y, x] = finalNoise;

                    if (finalNoise < minNoise) { minNoise = finalNoise; }
                    if (finalNoise > maxNoise) { maxNoise = finalNoise; }

                }
            }
        }

        //clamp all noise values so they are between 0 and 1
        for (int z = 0; z < chunkSize + 1; z++) {
            for (int y = 0; y < chunkSize + 1; y++) {
                for (int x = 0; x < chunkSize + 1; x++) {
                    noiseMap[z, y, x] = Mathf.InverseLerp(minNoise,maxNoise,noiseMap[z,y,x]);
                }
            }
        }
        return noiseMap;

    }

    public static float Perlin3D(float x, float y, float z) {
        //from https://www.youtube.com/watch?v=Aga0TBJkchM

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }
}
