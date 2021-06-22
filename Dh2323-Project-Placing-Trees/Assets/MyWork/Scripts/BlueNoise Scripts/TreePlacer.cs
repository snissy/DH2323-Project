using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using BlueNoise_Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TreePlacer : MonoBehaviour
{
    public int k = 30;
    public float r = 7.5f;
    public float offset = 10;
    public float minHeightLimit = 3.0f;
    public float maxHeightLimit = 50.0f;
    public float treeScale = 2;
    
    public float noiseScale = 100;
    
    [Range(0.0f, 1.0f)]
    public float sLimit = 0.5f;

    public GameObject treeObject;
    public Terrain plane;
    public  bool autoUpdate = false;
    public bool drawNoise = false;
    
    public void PlaceTrees()
    {
        ResetPlacements();
        
        var t = plane.terrainData;
        Bounds terrainBounds = t.bounds;

        Vector3 terrainMax = terrainBounds.max;
        Vector3 terrainMin = terrainBounds.min;
        
        float terrainHeight = terrainMax.z - terrainMin.z;
        float terrainWidth = terrainMax.x - terrainMin.x;
        
        float[,,] alphaMap = t.GetAlphamaps(0, 0, t.alphamapWidth, t.alphamapHeight);
        float [,] simplexNoise = Noise.GenerateNoiseMap((int) terrainWidth+1, (int) terrainHeight+1, noiseScale, sLimit);
        List<Vector3> points = BlueNoiseGenerator.GETPoints(r, k, 2, terrainWidth, terrainHeight);
        
        
        float rdFactor = (r / Mathf.Max(offset, 0.0001f)); 
        
        foreach (var point in points)
        {
            Transform toPlace = treeObject.transform.GetChild(Random.Range(0, treeObject.transform.childCount));
            
            // Don't know if we are going to use this but I think it's good
            Vector3 position = rdFactor*Random.onUnitSphere +  point;
            position.y = plane.SampleHeight(position);
            
            if ((position.y > minHeightLimit &&position.y <maxHeightLimit) && t.bounds.Contains(position))
            {
                int tracX = Mathf.FloorToInt((point.z / terrainWidth) * (t.alphamapWidth-1)); 
                int tracY = Mathf.FloorToInt((point.x / terrainHeight) * (t.alphamapHeight-1));
                if (Math.Abs(alphaMap[tracX, tracY, 1] - 1) > 0.01f)
                {
                    int noiseX = Mathf.FloorToInt(point.x);
                    int noiseY = Mathf.FloorToInt(point.z);
        
                    float propconditon = simplexNoise[noiseY, noiseX];
                    float outcome = Random.value;
        
                    if (outcome <= propconditon)
                    {
                        Transform tree = Instantiate(toPlace, position, new Quaternion(0.0f, Random.value,  0.0f, 1.0f), plane.transform);
                        float rdScaleIncrease = Random.Range(0, 0.2f);
                        float rdLenght = rdScaleIncrease + Random.Range(0, 0.2f);
                        tree.transform.localScale = new Vector3(1, 1, 1)*treeScale + new Vector3(rdScaleIncrease, rdLenght, rdScaleIncrease);
                    }
                }
            }
        }
        if (drawNoise)
        {
            float alphaHeight = t.alphamapHeight;
            float alphaWidth= t.alphamapHeight;
            for (int y = 0; y < alphaHeight; y++) {
                for (int x = 0; x < alphaWidth; x++)
                {
                    int sY = Mathf.FloorToInt( (y / alphaHeight) * terrainHeight);
                    int sX = Mathf.FloorToInt((x / alphaWidth) * terrainWidth);
                    alphaMap[x, y, 8] = simplexNoise[sX,sY];
                }
            }
            t.SetAlphamaps(0, 0, alphaMap);
            
        }

    }
    
    public void ResetPlacements()
    {
        ClearSimplexMap();
        Transform t = plane.transform;
        int nChildren = t.childCount;
        
        for (int i = 0; i < nChildren; i++)
        {
            DestroyImmediate(t.GetChild(0).gameObject); 
        }
    }
    
    void ClearSimplexMap(){
        
        var t = plane.terrainData;

        float[,,] map = t.GetAlphamaps(0, 0, t.alphamapWidth, t.alphamapHeight);
        
        for (int y = 0; y < t.alphamapHeight; y++) {
            for (int x = 0; x < t.alphamapWidth; x++) {
                map[x, y, 8] = 0;
            }
        }
        t.SetAlphamaps(0, 0, map);
    }
    
}