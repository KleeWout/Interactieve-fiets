using UnityEngine;

public class CameraFollow : MonoBehaviour
{
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
        // Calculate the desired position based on the target's position and the offset
        Vector3 desiredPosition = new Vector3(target.position.x - offset.x, offset.y, target.position.z - offset.z);
        // Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;

        // Optionally, make the camera always look at the target
        // transform.LookAt(target);
    }
}