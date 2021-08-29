using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UniformedRandomSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public float minHeightLimit = 3.0f;
    public float maxHeightLimit = 50.0f;
    public float treeScale = 2;
    public int numberOfTrees = 650;
    
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


        for (int i = 0; i < numberOfTrees; i++)
        {
            Transform toPlace = treeObject.transform.GetChild(Random.Range(0, treeObject.transform.childCount));

            Vector3 position = new Vector3(Random.Range( 0.0f, terrainWidth), 0.0f ,Random.Range( 0.0f, terrainHeight));
            position.y = plane.SampleHeight(position);

            if ((position.y > minHeightLimit && position.y < maxHeightLimit) && t.bounds.Contains(position))
            {
                int tracX = Mathf.FloorToInt((position.z / terrainWidth) * (t.alphamapWidth - 1));
                int tracY = Mathf.FloorToInt((position.x / terrainHeight) * (t.alphamapHeight - 1));
                if (Math.Abs(alphaMap[tracX, tracY, 1] - 1) > 0.01f)
                {
                    Transform tree = Instantiate(toPlace, position, new Quaternion(0.0f, Random.value, 0.0f, 1.0f),
                        plane.transform);
                    float rdScaleIncrease = Random.Range(0, 0.2f);
                    float rdLenght = rdScaleIncrease + Random.Range(0, 0.2f);
                    tree.transform.localScale = new Vector3(1, 1, 1) * treeScale +
                                                new Vector3(rdScaleIncrease, rdLenght, rdScaleIncrease);

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
