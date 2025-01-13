using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public Terrain terrain1;
    public Terrain terrain2;
    private TerrainData terrainData1;
    private TerrainData terrainData2;

    private int height = 513;
    private int width = 513;
    private float sizeY = 600;

    // Bezier
    private Vector3[] controlPoints = new Vector3[14];
    private List<Vector3> bezierPoints = new List<Vector3>();


    private float currentChunk = 0f;



    void Start()
    {
        terrainData1 = terrain1.terrainData;
        terrainData2 = terrain2.terrainData;

        GenerateRandomBezierCurve(true, new Vector2(0, 0));
        CarveTerrain();

        currentChunk += 513;
        GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z));
        CarveTerrain();
        

    }

    // Update is called once per frame
    void Update()
    {
        // dit is voor de volgende 
        // GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z));
    }

    void CarveTerrain()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = 10 / sizeY;
                foreach (var point in bezierPoints)
                {
                    float newx = point.x + 256f;
                    float newz;
                    if(currentChunk == 0)
                    {
                        newz = point.z + 256f;
                    }
                    else{
                        newz = point.z - 256f;
                    }
                    var distance = Vector2.Distance(new Vector2(newz, newx), new Vector2(x, y));
                    if (distance < 8)
                    {
                        heightValue = 0 / sizeY;
                        break;
                    }
                    else if (distance < 12)
                    {
                        heightValue = 6 / sizeY;
                    }
                }
                heights[x, y] = heightValue;
            }
        }

        heights = SmoothHeights(heights);
        if(currentChunk/513 % 2 == 0)
        {
            terrainData1.SetHeights(0, 0, heights);
        }
        else{
            terrainData2.SetHeights(0, 0, heights);
        }
    }





    void GenerateRandomBezierCurve(bool isFirstGen, Vector2 firstPoint)
    {
        if (isFirstGen)
        {
            controlPoints[0] = new Vector3(0, 0, 0);
            controlPoints[1] = new Vector3(0, 0, 50);
            controlPoints[2] = new Vector3(0, 0, 100);
            for (int i = 3; i < controlPoints.Length; i++)
            {
                float randomX = Random.Range(-200f, 200f);
                float z = i * 50;
                controlPoints[i] = new Vector3(randomX, 0, z);
            }
        }
        else
        {
            controlPoints[0] = new Vector3(firstPoint.x, 0, firstPoint.y);
            for (int i = 1; i < controlPoints.Length; i++)
            {
                float randomX = Random.Range(-200f, 200f);
                float z = i * 50 + firstPoint.y;
                controlPoints[i] = new Vector3(randomX, 0, z);
            }
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

        // remove all points that are not in the current chunk
        List<Vector3> pointsToRemove = new List<Vector3>();

        foreach(Vector3 point in bezierPoints)
        {
            if(point.z < currentChunk - 300 || point.z > currentChunk + 300)
            {
                pointsToRemove.Add(point);
            }
        }

        foreach(Vector3 point in pointsToRemove)
        {
            bezierPoints.Remove(point);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 point in bezierPoints)
        {
            Gizmos.DrawSphere(point, 1);
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


                float noiseValue1;
                float noiseValue2;
                // Generate Perlin noise values
                if(currentChunk == 0){
                    noiseValue1 = Mathf.PerlinNoise((x + 0.0f) * noiseScale1, y * noiseScale1) * noiseIntensity1;
                    noiseValue2 = Mathf.PerlinNoise((x + 0.0f) * noiseScale2, y * noiseScale2) * noiseIntensity2;
                }
                else{
                    noiseValue1 = Mathf.PerlinNoise((x + 512.0f) * noiseScale1, y * noiseScale1) * noiseIntensity1;
                    noiseValue2 = Mathf.PerlinNoise((x + 512.0f) * noiseScale2, y * noiseScale2) * noiseIntensity2;
                }

                float finalHeight = averageHeight + noiseValue1 + noiseValue2;
                smoothedHeights[x, y] = finalHeight;
            }
        }

        return smoothedHeights;
    }

}
