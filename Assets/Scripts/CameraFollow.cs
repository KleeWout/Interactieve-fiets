using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public TerrainGen terrainGen;

    // Reference to the target (the cube in this case)
    public Transform target;

    // Offset between the camera and the target
    public Vector3 offset;

    // Smoothing factor for smooth camera movement
    private float smoothSpeed = 100;

    void Start()
    {
    }

    void LateUpdate()
    {

        // Vector3 desiredRotation = new Vector3(target.rotation.x)

        // var test = GetNearestPoint(terrainGen.bezierCurve);

        // Debug.Log(GetOrientation(terrainGen.bezierCurve[test], terrainGen.bezierCurve[test+1000]));
        // Debug.Log(GetNearestPoint(terrainGen.bezierCurve));
        // GetNearestPoint(terrainGen.bezierCurve);

        // // Debug.Log(terrainGen.bezierCurve[GetNearestPoint(terrainGen.bezierCurve)]);

        // Vector3 desiredPosition = new Vector3(target.position.x - offset.x, offset.y, target.position.z - offset.z);
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // GetNearestPoint(terrainGen.bezierCurve);
        // Vector3 desiredPosition = new Vector3(terrainGen.bezierCurve[0].x, 1.25f, terrainGen.bezierCurve[0].z);
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 * Time.deltaTime);
        // transform.position = smoothedPosition;

        // float orientation = GetOrientation(terrainGen.bezierCurve[0], terrainGen.bezierCurve[0+5]);
        // Quaternion desiredRotation = Quaternion.Euler(0, orientation, 0);
        // Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, 1 * Time.deltaTime);

        // transform.rotation = smoothedRotation;
    }

    List<Vector3> nearestPoints = new List<Vector3>();
    Vector3 latestpoint;
    Vector3 latestpoint1;
    Vector3 latestpoint2;

    public int GetNearestPoint(List<Vector3> points)
    {
        int nearestPointIndex = 0;
        float minDistance = Vector3.Distance(target.position, points[0]);

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].z >= target.position.z-2 && points[i].z <= target.position.z+2)
            {
                float distance = Vector3.Distance(target.position, points[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPointIndex = i;
                }
            }
        }
        for(int i = 0; i < nearestPointIndex; i++){
            // Debug.Log($"Removed {terrainGen.bezierCurve[i]}");
            terrainGen.bezierCurve.Remove(terrainGen.bezierCurve[i]);
        }

        return nearestPointIndex;
    }

    public float GetOrientation(Vector3 point1, Vector3 point2)
    {
        Vector3 direction = point2 - point1;

        return direction.x * 100f;
    }
}