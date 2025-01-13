using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData terrainData;
    private int height;
    private int width;

    // Bezier
    private Vector3[] controlPoints = new Vector3[11];
    private List<Vector3> bezierPoints = new List<Vector3>();


    public int worldHeight;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        width = terrainData.heightmapResolution;
        height = terrainData.heightmapResolution;


        GenerateRandomBezierCurve();
        CarveTerrain();
    }
    void CarveTerrain()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = worldHeight / terrainData.size.y;
                foreach (var point in bezierPoints)
                {
                    float Newx = point.x + 512;
                    float Newz = point.z + 512;
                    var distance = Vector2.Distance(new Vector2(Newz, Newx), new Vector2(x, y));
                    if (distance < 8)
                    {
                        heightValue = 1 / terrainData.size.y;
                        break;
                    }
                    else if (distance < 12)
                    {
                        heightValue = 6 / terrainData.size.y;
                    }
                }
                heights[x, y] = heightValue;
            }
        }

        heights = SmoothHeights(heights);
        terrainData.SetHeights(0, 0, heights);
    }

    void GenerateRandomBezierCurve()
    {
        controlPoints[0] = new Vector3(0, 0, 0);
        controlPoints[1] = new Vector3(0, 0, 50);
        controlPoints[2] = new Vector3(0, 0, 100);

        for (int i = 2; i < controlPoints.Length; i++)
        {
            float randomX = Random.Range(-200f, 200f);
            float z = i * 50;
            controlPoints[i] = new Vector3(randomX, 0, z);
        }

        for (int i = 0; i < controlPoints.Length - 3; i += 3)
        {
            Vector3 previousPoint = controlPoints[i];

            for (float t = 0; t <= 1; t += 0.01f)
            {
                Vector3 point = CalculateBezierPoint(t, controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3]);
                bezierPoints.Add(point);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < controlPoints.Length - 3; i += 3)
        {
            Vector3 previousPoint = controlPoints[i];

            for (float t = 0; t <= 1; t += 0.01f)
            {
                Vector3 point = CalculateBezierPoint(t, controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3]);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * p0
        p += 3 * uu * t * p1; // 3 * (1-t)^2 * t * p1
        p += 3 * u * tt * p2; // 3 * (1-t) * t^2 * p2
        p += ttt * p3; // t^3 * p3

        return p;
    }
    float[,] SmoothHeights(float[,] heights)
    {
        int kernelSize = 1;
        float threshold = 0.01f; // Adjust this value to control sensitivity to height changes
        float noiseScale1 = 0.1f; // Adjust this value to control the scale of the first noise map
        float noiseIntensity1 = 0.01f; // Adjust this value to control the intensity of the first noise map
        float noiseScale2 = 0.02f; // Adjust this value to control the scale of the second noise map
        float noiseIntensity2 = 0.01f; // Adjust this value to control the intensity of the second noise map
        float[,] smoothedHeights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float currentHeight = heights[x, y];
                float sum = currentHeight;
                int count = 1;

                for (int i = -kernelSize; i <= kernelSize; i++)
                {
                    for (int j = -kernelSize; j <= kernelSize; j++)
                    {
                        int nx = x + i;
                        int ny = y + j;

                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        {
                            float neighborHeight = heights[nx, ny];
                            if (Mathf.Abs(neighborHeight - currentHeight) > threshold)
                            {
                                sum += neighborHeight;
                                count++;
                            }
                        }
                    }
                }

                float averageHeight = sum / count;

                // Generate Perlin noise values
                float noiseValue1 = Mathf.PerlinNoise(x * noiseScale1, y * noiseScale1) * noiseIntensity1;
                float noiseValue2 = Mathf.PerlinNoise(x * noiseScale2, y * noiseScale2) * noiseIntensity2;

                // Add noise to the average height
                smoothedHeights[x, y] = averageHeight + noiseValue1 + noiseValue2;
            }
        }

        return smoothedHeights;
    }
}