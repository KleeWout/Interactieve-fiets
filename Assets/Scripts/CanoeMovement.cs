using UnityEngine;

public class BoatWobble : MonoBehaviour
{
    public float wobbleMaxAngle = 30f;
    public float wobbleSmoothing = 2f;
    public float maxTiltAngle = 20f;
    public float tiltSmoothing = 8f;
    public float steeringSpeed = 20f;
    public float thrustSpeed = 10f;


    public Rigidbody rb;

    void FixedUpdate()
    {

        float horizontalInput = Input.GetAxis("Horizontal");


        float wobble = Mathf.Sin(Time.time * 2f) * wobbleMaxAngle;
        float newZ = horizontalInput * maxTiltAngle;
        float currentZ = rb.rotation.eulerAngles.z;
        float smoothedZ = Mathf.LerpAngle(currentZ, newZ, Time.deltaTime * tiltSmoothing);
        float wobbles = Mathf.LerpAngle(smoothedZ, wobble, Time.deltaTime * wobbleSmoothing);


        float newY = rb.rotation.eulerAngles.y + horizontalInput * steeringSpeed * Time.deltaTime;

        
        transform.rotation = Quaternion.Euler(0, newY, wobbles);

        Vector3 forceDirection = transform.forward * Mathf.Abs(horizontalInput*thrustSpeed);
        Vector3 forcePosition = transform.TransformPoint(new Vector3(0, 0, 1f));
        rb.AddForceAtPosition(forceDirection, forcePosition);

  
    }

}