using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject cubePrefab; // Assign the cube prefab in the inspector
    private List<GameObject> createdInstances = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator GenerateObstaclesForChunk(List<Vector3> points, int chunk)
    {

        int row = 80;
        System.Random random = new System.Random();

        // Create a copy of the points list to avoid modifying the collection during enumeration
        List<Vector3> pointsCopy = new List<Vector3>(points);

        foreach (var point in pointsCopy)
        {
            if (point.z >= row && point.z <= (chunk * 513) - 256.5f + 513f)
            {
                Vector3 position = new Vector3(point.x, 0, point.z);
                GameObject instance = Instantiate(cubePrefab, position, Quaternion.identity);
                createdInstances.Add(instance);

                row += random.Next(4, 50);

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
}
