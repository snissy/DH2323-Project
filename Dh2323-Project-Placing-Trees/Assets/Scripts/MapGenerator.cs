using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    [Range(0,1)]
    public float s; 
    
    public bool autoUpdate = false;
    
    public void GenerateMap()
    { 
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, s);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

}
