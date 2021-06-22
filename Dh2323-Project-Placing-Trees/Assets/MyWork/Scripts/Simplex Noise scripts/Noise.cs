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
                }
            }
            // LatticePoints.UpdateHash();
        }
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                max = Mathf.Max(noiseMap[x, y], max);
                min = Mathf.Min(noiseMap[x, y], min);
            }
        }

        // float a = Mathf.Log(2.0f) / s;
        float a = 0.9f / s;
        
        for (int y = 0; y < mapHeight; y++)
        { 
            for (int x = 0; x < mapWidth; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= min;
                noiseValue *= 1 / (max - min);
                
                if (noiseValue <= s)
                {
                    noiseMap[x, y] = Mathf.Log10(noiseValue * a + 0.1f) + 1;
                    //noiseMap[x, y] = Mathf.Exp(noiseValue* a)-1;
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
