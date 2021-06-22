using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlueNoise_Scripts
{
    public class ArrayMethods
    {
        public static void Fill2D<T>( T[,] array, T value)
        {
            int dimX = array.GetLength(0);
            int dimY = array.GetLength(1);
            
            for(int i = 0; i < dimX; i++) {
                
                for (int j = 0; j < dimY; j++)
                {
                    array[i, j] = value;
                }
            }
        }
        public static bool WithinBoundaries2D<T>(T[,] array, int x, int y)
        {
            return ((0 <= x) & (x < array.GetLength(0))) & ((0 <= y ) & (y < array.GetLength(1)));
        }
    }
    
    public class BlueNoiseGenerator
    {

        public static List<Vector3> GETPoints(float radius, int k, int nDim, float mapWidth, float mapHeight )
        {

            float cellSize = radius / Mathf.Sqrt(nDim);
            
            // in python we set everything to -1, but here it's just 1. 
            int[,] grid = new int[Mathf.CeilToInt(mapWidth / cellSize), Mathf.CeilToInt(mapHeight / cellSize)];
            
            ArrayMethods.Fill2D(grid, -1);
            
            List<Vector3> allPoints = new List<Vector3>();
            List<Vector3> activePoints = new List<Vector3>();
            
            Vector3 startSample = new Vector3(Random.Range(0, mapWidth),0.0f, Random.Range(0, mapHeight));
            
            allPoints.Add(startSample);
            activePoints.Add(startSample);

            int gridX = Mathf.FloorToInt(startSample.x / cellSize);  // cell_size)
            int gridY = Mathf.FloorToInt(startSample.z / cellSize);  // cell_size)
            
            grid[gridX, gridY] = 0;

            while (activePoints.Count > 0  && activePoints.Count<1000)
            {
                var randomIndex = Random.Range(0, activePoints.Count);
                Vector3 rdPoint = activePoints[randomIndex];
                bool foundOkPoints = false;

                for (int i = 0; i < k; i++)
                {
                    float angle = Random.value * 2 * Mathf.PI;
                    float annulusR = radius + Random.value * radius;
                    Vector3 kPoint = rdPoint + new Vector3(annulusR* Mathf.Cos(angle), 0.0f, annulusR * Mathf.Sin(angle));
                    
                    gridX  = Mathf.FloorToInt(kPoint.x / cellSize);
                    gridY  = Mathf.FloorToInt(kPoint.z / cellSize);
                    if (ArrayMethods.WithinBoundaries2D(grid, gridX, gridY)) // Check In boundaries  withBoundaries(n_cells_width, n_cells_height, gridX, gridY):
                    {
                        bool okPoint = true;

                        for (int x = -2; x < 3; x++)
                        {
                            for (int y = -2; y < 3; y++)
                            {   
                                int checkX = gridX + x;
                                int checkY = gridY + y;

                                if (ArrayMethods.WithinBoundaries2D(grid, checkX, checkY )) // Check In boundaries
                                {
                                    int checkIndex = grid[checkX, checkY];
                                    if (checkIndex > -1){
                                        Vector3 checkPoint = allPoints[checkIndex];
                                        if (Vector3.Distance(kPoint ,checkPoint )<= radius) {
                                            okPoint = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (okPoint)
                        {
                            activePoints.Add(kPoint);
                            allPoints.Add(kPoint);
                            grid[gridX, gridY] = allPoints.Count - 1;
                            foundOkPoints = true;
                        }
                        
                    }
                    
                }

                if (!foundOkPoints)
                {
                    activePoints.RemoveAt(randomIndex);
                }
                
            }


            return allPoints;
        }
    }
}