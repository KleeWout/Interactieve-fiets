using UnityEngine;

public class CanoeMovement : MonoBehaviour
{
    private EspListener espListener;

    public float wobbleMaxAngle = 30f;
    public float wobbleSmoothing = 2f;
    public float maxTiltAngle = 20f;
    public float tiltSmoothing = 8f;
    public float steeringSpeed = 20f;
    public float thrustSpeed = 10f;

    public EspListener EspListener;
    public float value1;
    public float value2;


    public Rigidbody rb;
    void Start()
    {
        espListener = GetComponent<EspListener>();
    }


    void FixedUpdate()
    {
        //getal tssn -1 en 1
        value1 = espListener.value1;
        value2 = espListener.value2;
        float normalizedValue1 = (value1 / 150f);
        float normalizedValue2 = (value2 / 150f);

        float horizontalInput = (normalizedValue1 - normalizedValue2);


        float wobble = Mathf.Sin(Time.time * 2f) * wobbleMaxAngle;
        float newZ = horizontalInput * maxTiltAngle;
        float currentZ = rb.rotation.eulerAngles.z;
        float smoothedZ = Mathf.LerpAngle(currentZ, newZ, Time.deltaTime * tiltSmoothing);
        float wobbles = Mathf.LerpAngle(smoothedZ, wobble, Time.deltaTime * wobbleSmoothing);


        float newY = rb.rotation.eulerAngles.y + horizontalInput * steeringSpeed * Time.deltaTime;


        transform.rotation = Quaternion.Euler(0, newY, wobbles);
        // Check if value1 and value2 are approximately equal
        if (Mathf.Abs(value1 - value2) < 1f) // Or some small threshold to consider them equal
        {
            // Move forward if values are equal
            Vector3 forwardDirection = transform.forward * thrustSpeed;
            rb.AddForce(forwardDirection, ForceMode.Force);
        }

        // Apply rotational forces
        Vector3 forceDirection = transform.forward * Mathf.Abs(horizontalInput * thrustSpeed);
        Vector3 forcePosition = transform.TransformPoint(new Vector3(0, 0, 1f));
        rb.AddForceAtPosition(forceDirection, forcePosition);

        // Debug.Log("Value 1: " + espListener.value1 + " Value 2: " + espListener.value2);
    }



}