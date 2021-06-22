using System;
using System.Collections;
using System.Collections.Generic;
using BlueNoise_Scripts;
using MyWork.Scripts.propagation;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropagationSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float minHeightLimit = 3.0f;
    public float maxHeightLimit = 50.0f;
    public float baseTreeRadius = 5.0f;
    public float treeScale = 2;
    
    public GameObject treeObject;
    public Terrain plane;
    
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
       
        Forest test = new Forest(terrainWidth, terrainHeight, baseTreeRadius, 3);
        test.SimulateForrestGrowth(80);
        List<Vector3> points = test.GETPoints();
        
        foreach (var point in points)
        {
            Transform toPlace = treeObject.transform.GetChild(Random.Range(0, treeObject.transform.childCount));
            
            Vector3 position = point;
            position.y = plane.SampleHeight(position);
            
            if ((position.y > minHeightLimit && position.y <maxHeightLimit) && t.bounds.Contains(position))
            {
                int tracX = Mathf.FloorToInt((point.z / terrainWidth) * (t.alphamapWidth-1)); 
                int tracY = Mathf.FloorToInt((point.x / terrainHeight) * (t.alphamapHeight-1));
                if (Math.Abs(alphaMap[tracX, tracY, 1] - 1) > 0.01f)
                {
                    Transform tree = Instantiate(toPlace, position, new Quaternion(0.0f, Random.value,  0.0f, 1.0f), plane.transform);
                    float rdScaleIncrease = Random.Range(0, 0.2f);
                    float rdLenght = rdScaleIncrease + Random.Range(0, 0.2f);
                    tree.transform.localScale = new Vector3(1, 1, 1)*treeScale + new Vector3(rdScaleIncrease, rdLenght, rdScaleIncrease);
                    
                }
            }
        }
    }
    
    public void ResetPlacements()
    {
        Transform t = plane.transform;
        int nChildren = t.childCount;
        
        for (int i = 0; i < nChildren; i++)
        {
            DestroyImmediate(t.GetChild(0).gameObject); 
        }
    }

    // Update is called once per frame
}
