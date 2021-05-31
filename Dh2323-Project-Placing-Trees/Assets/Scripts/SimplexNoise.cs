using System;
using UnityEngine;
using Klak.Math;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    // <summary>
    //This code is heavily based on Stefan's Gustavson work. And the wikipedia page on Simplex Noise.
    // https://weber.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
    // https://en.wikipedia.org/wiki/Simplex_noise 
    // Picking the gradients have been made after some research based on these articles. 
    // https://blog.unity.com/technology/a-primer-on-repeatable-random-numbers For hash 
    // https://github.com/keijiro/Klak/blob/master/Assets/Klak/Math/Runtime/XXHash.cs 
    // https://en.wikipedia.org/wiki/Pairing_function#Cantor_pairing_function 
    // </summary>
    public class LatticePoints
    {
        private static XXHash _hashEngine = XXHash.RandomHash;
        private const int NGradients = 16;
        private static readonly float[,] GVector2S = GenerateGradients();
        
        public static void UpdateHash()
        {
            _hashEngine = XXHash.RandomHash;
        }

        public static float[] FetchLatticePoint(int i, int j)
        {
            int n = (i + j) * (i + j + 1) / 2 + j;
            long hash = _hashEngine.GetHash(n) % NGradients;
            // return new float[] {GVector2S[hash, 0] ,GVector2S[hash, 1]};
            
            //long hash = _hashEngine.GetHash((int )_hashEngine.GetHash(i) + j) % NGradients;
            return new float[] {GVector2S[hash, 0] ,GVector2S[hash, 1]};
        }

        private static float[,] GenerateGradients()
        {
            var gradients = new float[NGradients,2];
            float divider = NGradients;
            
            for (int i = 0; i < NGradients; i++)
            {
                float angle = ((i+1)/divider) * 2.0f * Mathf.PI;
                
                gradients[i, 0] = Mathf.Cos(angle);
                gradients[i, 1] = Mathf.Sin(angle);

            }
            return gradients;
        }

       
    }
    
    public class SimplexNoise {
        // This code is heavily based on Stefans Gustavson work. And the wikipedia page on Simplex Noise.
        // https://weber.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
        // https://en.wikipedia.org/wiki/Simplex_noise 
        
        private static float F2 = 0.5f * (Mathf.Sqrt(3.0f) -1);
        private static float G2 = (3.0f - Mathf.Sqrt(3.0f))/6.0f;
            
        //  F2 = 0.5 * (np.sqrt(3.0) - 1.0)  # Here we create a factor in order to go from our triangle grid to quad grid.
        // G2 = (3.0 - np.sqrt(3.0)) / 6.0
        public static float GETNoiseValue(float xin, float yin)
        {
            // First we skew the input space inorder to figure out which cell we are in. 
            float s = (xin + yin) * F2;
            
            int i = (int) Mathf.Floor(xin + s);
            int j = (int) Mathf.Floor(yin + s);

            float t = (i + j) * G2;

            float X0 = i - t;
            float Y0 = j - t;

            float x0 = xin - X0;
            float y0 = yin - Y0;

            int i1;
            int j1;

            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }
            else
            {
                i1 = 0;
                j1 = 1;
            }

            float x1 = x0 - i1 + G2;
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2;
            float y2 = y0 - 1.0f + 2.0f * G2;
            
            // Here we fetch the gradients that we need. 
            float[] gi0 = LatticePoints.FetchLatticePoint(j, i);
            float[] gi1 = LatticePoints.FetchLatticePoint(j + j1, i + i1);
            float[] gi2 = LatticePoints.FetchLatticePoint(j + 1, i + 1);
            
            float n0;
            float n1;
            float n2;
            
            float t0 = 0.5f - x0 * x0 - y0 * y0;
            float t1 = 0.5f - x1 * x1 - y1 * y1;
            float t2 = 0.5f - x2 * x2 - y2 * y2;
            
            if (t0 < 0) {
                n0 = 0.0f;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * (gi0[0] * x0 + gi0[1]*y0);
            }

            if (t1 < 0)
            {
                n1 = 0.0f;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * (gi1[0] * x1 + gi1[1]*y1);
            }


            if (t2 < 0)
            {
                n2 = 0.0f;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * (gi2[0] * x2 + gi2[1]*y2);
            }
            
            return 70.0f * (n0 + n1 + n2);

        }
    }
    
}