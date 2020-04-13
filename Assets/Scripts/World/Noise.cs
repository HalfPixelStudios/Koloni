using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise  {

    public static float[,,] GenerateNoiseMap(int seed, int chunkSize, Vector3 offset, float noiseScale, int octaves, float persistance, float lacunarity) {
        System.Random prng = new System.Random(seed);

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
                        float px = x / noiseScale * freq;
                        float py = y / noiseScale * freq;
                        float pz = z / noiseScale * freq;
                        float perlinValue = Perlin3D(px, py, pz);
                        noise += perlinValue;

                        //modify freq and amp per pass
                        amp *= persistance;
                        freq *= lacunarity;
                    }
                    noiseMap[z, y, x] = noise;

                    if (noise < minNoise) { minNoise = noise; }
                    if (noise > maxNoise) { maxNoise = noise; }

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
