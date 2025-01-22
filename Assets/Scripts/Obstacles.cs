using System;
using System.Collections;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject cubePrefab; // Assign the cube prefab in the inspector

    private bool isDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // public void GenerateObstaclesForChunk(float[,] heightmap){
    //     foreach(float height in heightmap){
    //         if(height<0){
    //             Debug.Log(height);
    //         }
    //     }
    // }
    // public void GenerateObstaclesForChunk(float[,] heightmap)
    // {
    //     int row = 20;
    //     System.Random random = new System.Random();

    //     while (row < heightmap.GetLength(0))
    //     {
    //         for (int column = 0; column < heightmap.GetLength(1); column++)
    //         {
    //             float height = heightmap[row, column];
    //             if (height < 0.007f)
    //             {
    //                 Debug.Log($"Height: {height}, Row: {row}, Column: {column}");
    //                 Vector3 position = new Vector3(column-256.5f, 0, row-256.5f);
    //                 Instantiate(cubePrefab, position, Quaternion.identity);
    //                 break;
    //             }
    //         }
    //         row += 20 + random.Next(5, 21);
    //     }
    // }
    public IEnumerator GenerateObstaclesForChunk(float[,] heightmap)
    {
        if (!isDone)
        {
            isDone = true;
            int row = 20;
            System.Random random = new System.Random();

            while (row < heightmap.GetLength(0))
            {
                for (int column = 0; column < heightmap.GetLength(1); column++)
                {
                    float height = heightmap[row, column];
                    if (height < 0.007f)
                    {
                        // row is z
                        // column is x
                        // Debug.Log($"Height: {height}, Row: {row}, Column: {column}");
                        Vector3 position = new Vector3(column - 256.5f, 0, row - 256.5f);
                        Instantiate(cubePrefab, position, Quaternion.identity);

                        break; // Exit the loop after placing one instance
                    }
                }
                row += 20 + random.Next(5, 21);

                // Yield to ensure the coroutine doesn't block the main thread
                yield return null;

            }

            Debug.Log("Finished generating obstacles for chunk.");
        }
        else
        {
            Debug.Log("Obstacles generation already done.");
        }

    }
}
