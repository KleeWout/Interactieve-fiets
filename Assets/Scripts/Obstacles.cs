using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models.GameModeModel;
using UnityEngine.UIElements.Experimental;

public class Obstacles : MonoBehaviour
{
    public GameObject[] obstaclePrefab; // Assign the cube prefab in the inspector
    private List<GameObject> createdInstances = new List<GameObject>();
    System.Random random = new System.Random();
    float row;
    float randomValue;
    private int randomObstacle;

    public IEnumerator GenerateObstaclesForChunk(List<Vector3> points, int chunk, GameMode mode)
    {
        RemoveInstancesAboveZ((chunk * 513) - 256.5f);

        List<Vector3> pointsCopy = new List<Vector3>(points);

        if (mode == GameMode.SinglePlayer)
        {
            if (chunk == 0)
            {
                row = 5f;
            }
            else
            {
                row = (chunk * 513) - 256.5f + random.Next(1, 5); ;
            }
        }
        else if (mode == GameMode.MultiPlayer)
        {
            if (chunk == 0)
            {
                row = 20f;
            }
            else
            {
                row = (chunk * 513) - 256.5f + random.Next(1, 5); ;
            }
        }

        foreach (var point in pointsCopy)
        {
            if (mode == GameMode.SinglePlayer)
            {
                randomValue = (float)(random.NextDouble() * (8.0 - 2.0) + 2.0);
            }
            else if (mode == GameMode.MultiPlayer)
            {
                randomValue = (float)(random.NextDouble() * (15.0 - 2.0) + 2.0);
            }
            if (point.z >= row && point.z < (chunk * 513) + 256.5f)
            {
                if (random.Next(1, 21) == 1 && mode == GameMode.MultiPlayer)
                {
                    randomObstacle = 4;
                }
                else
                {
                    randomObstacle = random.Next(0, 3);
                }
                Vector3 position = new Vector3(point.x, 0, point.z);
                GameObject instance = Instantiate(obstaclePrefab[randomObstacle], position, Quaternion.identity);
                createdInstances.Add(instance);

                row += randomValue;

                yield return null;
            }
        }

        RemoveInstancesBelowZ((chunk * 513) - 769.5f);

    }

    private void RemoveInstancesBelowZ(float zThreshold)
    {
        for (int i = createdInstances.Count - 1; i >= 0; i--)
        {
            if (createdInstances[i].transform.position.z < zThreshold)
            {
                Destroy(createdInstances[i]);
                createdInstances.RemoveAt(i);
            }
        }
    }
    private void RemoveInstancesAboveZ(float zThreshold)
    {
        for (int i = createdInstances.Count - 1; i >= 0; i--)
        {
            if (createdInstances[i].transform.position.z > zThreshold)
            {
                Destroy(createdInstances[i]);
                createdInstances.RemoveAt(i);
            }
        }
    }
}
