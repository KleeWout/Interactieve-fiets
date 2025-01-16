using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using models.GameMode;

public class TerrainGen : MonoBehaviour
{
    private GameMode currentGameMode;
    public GameObject player;
    public Terrain terrain1;
    public Terrain terrain2;
    private TerrainData terrainData1;
    private TerrainData terrainData2;

    private int height = 513;
    private int width = 513;
    private float sizeY = 600;

    // Bezier
    public Vector3[] controlPoints;
    public List<Vector3> bezierPoints = new List<Vector3>();

    public List<Vector3> bezierCurve = new List<Vector3>();

    private int chunkCount = 0;


    private float[,] lastHeights;
    Task<float[,]> heights;


    public async void GenerateTerrain(GameMode mode)
    {
        currentGameMode = mode;

        bezierCurve.Clear();
        bezierPoints.Clear();
        heights = null;
        lastHeights = null;
        chunkCount = 0;
        controlPoints = new Vector3[14];
        terrain1.transform.position = new Vector3(-256.5f, -10, -256.5f);
        terrain2.transform.position = new Vector3(-256.5f, -10, 256.5f);


        Debug.Log("Generating terrain");
        if (mode == GameMode.SinglePlayer)
        {
            GenerateRandomBezierCurve(true, new Vector2(0, 0), 10);
            heights = CarveTerrainAsync();
            await heights;
            lastHeights = heights.Result;
            terrainData1.SetHeights(0, 0, heights.Result);

            chunkCount += 1;
            GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z), 10);
            heights = CarveTerrainAsync();
            await heights;
            lastHeights = heights.Result;
            terrainData2.SetHeights(0, 0, heights.Result);
        }
        else if (mode == GameMode.MultiPlayer)
        {
            GenerateRandomBezierCurve(true, new Vector2(0, 0), 50);
            heights = CarveTerrainAsync();
            await heights;
            lastHeights = heights.Result;
            terrainData1.SetHeights(0, 0, heights.Result);

            chunkCount += 1;
            GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z), 50);
            heights = CarveTerrainAsync();
            await heights;
            lastHeights = heights.Result;
            terrainData2.SetHeights(0, 0, heights.Result);
        }
    }

    void Start()
    {
        terrainData1 = terrain1.terrainData;
        terrainData2 = terrain2.terrainData;
    }

    async void Update()
    {
        if (player.transform.position.z > (chunkCount * 513))
        {
            if(currentGameMode == GameMode.SinglePlayer){
                if (chunkCount % 2 == 0)
                {
                    chunkCount += 1;
                    Vector3 newPosition = terrain2.transform.position;
                    newPosition.z += 1026f;
                    terrain2.transform.position = newPosition;
                    GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z), 0);
                    var heights = CarveTerrainAsync();
                    await heights;
                    lastHeights = heights.Result;
                    terrainData2.SetHeights(0, 0, heights.Result);
                }
                else
                {
                    chunkCount += 1;
                    Vector3 newPosition = terrain1.transform.position;
                    newPosition.z += 1026f;
                    terrain1.transform.position = newPosition;
                    GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z), 0);
                    var heights = CarveTerrainAsync();
                    await heights;
                    lastHeights = heights.Result;
                    terrainData1.SetHeights(0, 0, heights.Result);
                }
            }
            else if(currentGameMode == GameMode.MultiPlayer){
                if (chunkCount % 2 == 0)
                {
                    chunkCount += 1;
                    Vector3 newPosition = terrain2.transform.position;
                    newPosition.z += 1026f;
                    terrain2.transform.position = newPosition;
                    GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z), 50);
                    var heights = CarveTerrainAsync();
                    await heights;
                    lastHeights = heights.Result;
                    terrainData2.SetHeights(0, 0, heights.Result);
                }
                else
                {
                    chunkCount += 1;
                    Vector3 newPosition = terrain1.transform.position;
                    newPosition.z += 1026f;
                    terrain1.transform.position = newPosition;
                    GenerateRandomBezierCurve(false, new Vector2(bezierPoints[bezierPoints.Count - 1].x, bezierPoints[bezierPoints.Count - 1].z), 50);
                    var heights = CarveTerrainAsync();
                    await heights;
                    lastHeights = heights.Result;
                    terrainData1.SetHeights(0, 0, heights.Result);
                }
            }


        }
    }

    async Task<float[,]> CarveTerrainAsync()
    {
        float[,] heights = await Task.Run(() => CarveTerrain());
        return heights;
    }

    float[,] CarveTerrain()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = 10 / sizeY;
                heights[x, y] = heightValue;
            }
        }

        System.Random random = new System.Random();

        foreach (var point in bezierPoints)
        {
            float newx = point.x + 256f;
            float newz = point.z - ((chunkCount * 512) - 256f);

            var points3 = GetPointsInRadius(new Vector2(newz, newx), (float)random.NextDouble() * (35f - 12f) + 10f);
            foreach (var p in points3)
            {
                if (p.x >= 0 && p.x < 513 && p.y >= 0 && p.y < 513)
                {
                    heights[(int)p.x, (int)p.y] = ((float)random.NextDouble() * (10f - 8f) + 8f) / sizeY; //#8f
                    continue;
                }
            }
        }
        foreach (var point in bezierPoints)
        {
            float newx = point.x + 256f;
            float newz = point.z - ((chunkCount * 512) - 256f);

            var points3 = GetPointsInRadius(new Vector2(newz, newx), (float)random.NextDouble() * (10f - 8f) + 8f);
            foreach (var p in points3)
            {
                if (p.x >= 0 && p.x < 513 && p.y >= 0 && p.y < 513)
                {
                    heights[(int)p.x, (int)p.y] = ((float)random.NextDouble() * (5f - 3f) + 3f) / sizeY;
                    continue;
                }
            }
        }
        foreach (var point in bezierPoints)
        {
            float newx = point.x + 256f;
            float newz = point.z - ((chunkCount * 512) - 256f);

            var points3 = GetPointsInRadius(new Vector2(newz, newx), 5f);
            foreach (var p in points3)
            {
                if (p.x >= 0 && p.x < 513 && p.y >= 0 && p.y < 513)
                {
                    heights[(int)p.x, (int)p.y] = 2f / sizeY;
                    continue;
                }
            }
        }
        foreach (var point in bezierPoints)
        {
            float newx = point.x + 256f;
            float newz = point.z - ((chunkCount * 512) - 256f);

            var points3 = GetPointsInRadius(new Vector2(newz, newx), 2f);
            foreach (var p in points3)
            {
                if (p.x >= 0 && p.x < 513 && p.y >= 0 && p.y < 513)
                {
                    heights[(int)p.x, (int)p.y] = ((float)random.NextDouble() * (1f - 0f) + 0f) / sizeY;
                    continue;
                }
            }
        }


        heights = SmoothHeights(heights);

        if (chunkCount != 0)
        {
            for (int i = 0; i < 513; i++)
            {
                heights[0, i] = lastHeights[512, i];
            }
        }

        foreach (var point in bezierPoints)
        {
            float newx = point.x + 256f;
            float newz = point.z - ((chunkCount * 512) - 256f);

            var points3 = GetPointsInRadius(new Vector2(newz, newx), 2f);
            foreach (var p in points3)
            {
                if (p.x >= 0 && p.x < 513 && p.y >= 0 && p.y < 513)
                {
                    heights[(int)p.x, (int)p.y] = ((float)random.NextDouble() * (6f - 4f) + 4f) / sizeY;
                    continue;
                }
            }
        }

        return heights;
    }

    List<Vector2> GetPointsInRadius(Vector2 center, float radius)
    {
        List<Vector2> pointsInRadius = new List<Vector2>();
        int minX = Mathf.FloorToInt(center.x - radius);
        int maxX = Mathf.CeilToInt(center.x + radius);
        int minY = Mathf.FloorToInt(center.y - radius);
        int maxY = Mathf.CeilToInt(center.y + radius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2 point = new Vector2(x, y);
                if (Vector2.Distance(center, point) <= radius)
                {
                    pointsInRadius.Add(point);
                }
            }
        }

        return pointsInRadius;
    }


    void GenerateRandomBezierCurve(bool isFirstGen, Vector2 firstPoint, int deviation)
    {
        if (isFirstGen)
        {
            controlPoints[0] = new Vector3(0, 0, 0);
            controlPoints[1] = new Vector3(0, 0, 51.3f);
            controlPoints[2] = new Vector3(0, 0, 102.6f);
            for (int i = 3; i < controlPoints.Length; i++)
            {
                float randomX = Random.Range(-deviation, deviation);
                float z = i * 51.3f;
                controlPoints[i] = new Vector3(randomX, 0, z);
            }
        }
        else
        {
            controlPoints[0] = new Vector3(firstPoint.x, 0, firstPoint.y);
            for (int i = 1; i < controlPoints.Length; i++)
            {
                float randomX = Random.Range(-deviation, deviation);
                float z = i * 51.3f + firstPoint.y;
                controlPoints[i] = new Vector3(randomX, 0, z);
            }
        }

        for (int i = 0; i < controlPoints.Length - 3; i += 3)
        {
            Vector3 previousPoint = controlPoints[i];

            for (float t = 0; t <= 1; t += 0.002f)
            {
                Vector3 point = CalculateBezierPoint(t, controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3]);
                bezierPoints.Add(point);
                bezierCurve.Add(point);

                Debug.Log(point.z);

                // if(point.z < (chunkCount * 513)+256.5){
                //     bezierCurve.Add(point);
                // }
            }

        }

        List<Vector3> pointsToRemove = new List<Vector3>();

        foreach (Vector3 point in bezierPoints)
        {
            if (point.z < (chunkCount * 513) - 265.5f || point.z > (chunkCount * 513) + 265.5f)
            {
                pointsToRemove.Add(point);
            }
        }

        foreach (Vector3 point in pointsToRemove)
        {
            bezierPoints.Remove(point);
            if (point.z > (chunkCount * 513) + 256.5f)
            {
                if(bezierCurve.Contains(point)){
                    bezierCurve.Remove(point);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 point in bezierCurve)
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
        float threshold = 0.01f;
        float noiseScale1 = 0.1f;
        float noiseIntensity1 = 0.01f;
        float noiseScale2 = 0.02f;
        float noiseIntensity2 = 0.01f;
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
                noiseValue1 = Mathf.PerlinNoise((x + chunkCount * 512) * noiseScale1, y * noiseScale1) * noiseIntensity1;
                noiseValue2 = Mathf.PerlinNoise((x + chunkCount * 512) * noiseScale2, y * noiseScale2) * noiseIntensity2;
                float finalHeight = averageHeight + noiseValue1 + noiseValue2;
                smoothedHeights[x, y] = finalHeight;
            }
        }
        return smoothedHeights;
    }

}
