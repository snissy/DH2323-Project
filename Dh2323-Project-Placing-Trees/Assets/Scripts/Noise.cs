using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Noise {
    // Start is called before the first frame update

    // ReSharper disable Unity.PerformanceAnalysis
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, float s)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;

        if (scale <= 0)
        {
            scale = 0.00001f;
        }
        for (int i = 0; i < 4; i++)
        { 
            float freq = (scale / (Mathf.Pow(2, i))); 
            float apm = (2 / (Mathf.Pow(2, i + 1)));
            
            for (int y = 0; y < mapHeight; y++) { 
                for (int x = 0; x < mapWidth; x++) 
                {
                    float sampleX = x/freq;
                    float sampleY = y/freq; 
                    float noiseValue = apm*SimplexNoise.GETNoiseValue(sampleX, sampleY);
                    noiseMap[x, y] += noiseValue;
                    max = Mathf.Max(noiseMap[x, y], max);
                    min = Mathf.Min(noiseMap[x, y], min);
                }
            }
            // LatticePoints.UpdateHash();
        }
        
        float a = Mathf.Log(1.95f) / s;
        
        for (int y = 0; y < mapHeight; y++)
        { 
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] -= min;
                noiseMap[x, y] *= 1 / (max - min);
                if (0 < s) {
                    noiseMap[x, y] = Mathf.Exp(noiseMap[x, y] * a - 0.95f);
                }
                else
                {
                    noiseMap[x, y] = 1.0f;
                }
            }
        }
  
        
        return noiseMap;
    }
}
