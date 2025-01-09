using UnityEngine;
using UnityEngine.InputSystem;
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

    public bool controllerActivated = true;
    public float leftStickY;
    public float rightStickY;
    float horizontalInput;



    public Rigidbody rb;
    void Start()
    {
        espListener = GetComponent<EspListener>();
        //controller
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log("Gamepad " + i + ": " + Gamepad.all[i].name);
        }
    }


    void FixedUpdate()
    {
        if (controllerActivated)
        {

            //controller
            if (Gamepad.all.Count > 0)
            {
                leftStickY = Gamepad.all[0].leftStick.ReadValue().y;
                rightStickY = Gamepad.all[0].rightStick.ReadValue().y;

                horizontalInput = (leftStickY - rightStickY);

                //Move forward if both sticks are pushed forward
                if (leftStickY > 0.5f && rightStickY > 0.5f)
                {
                    Vector3 forwardDirection = transform.forward * thrustSpeed;
                    rb.AddForce(forwardDirection, ForceMode.Force);
                }


            }
        }
        else
        {
            //getal tssn -1 en 1
            value1 = espListener.value1;
            value2 = espListener.value2;
            float normalizedValue1 = (value1 / 150f);
            float normalizedValue2 = (value2 / 150f);

            horizontalInput = (normalizedValue1 - normalizedValue2);

            // Check if value1 and value2 are approximately equal and not zero
            if (Mathf.Abs(value1 - value2) < 1f && Mathf.Abs(value1) > 0.1f && Mathf.Abs(value2) > 0.1f)
            {
                // Move forward if values are equal and not zero
                Vector3 forwardDirection = transform.forward * thrustSpeed;
                rb.AddForce(forwardDirection, ForceMode.Force);
            }

            // Apply rotational forces
            Vector3 forceDirection = transform.forward * Mathf.Abs(horizontalInput * thrustSpeed);
            Vector3 forcePosition = transform.TransformPoint(new Vector3(0, 0, 1f));
            rb.AddForceAtPosition(forceDirection, forcePosition);

            // Debug.Log("Value 1: " + espListener.value1 + " Value 2: " + espListener.value2);

        }
        float wobble = Mathf.Sin(Time.time * 2f) * wobbleMaxAngle;
        float newZ = horizontalInput * maxTiltAngle;
        float currentZ = rb.rotation.eulerAngles.z;
        float smoothedZ = Mathf.LerpAngle(currentZ, newZ, Time.deltaTime * tiltSmoothing);
        float wobbles = Mathf.LerpAngle(smoothedZ, wobble, Time.deltaTime * wobbleSmoothing);


        float newY = rb.rotation.eulerAngles.y + horizontalInput * steeringSpeed * Time.deltaTime;


        transform.rotation = Quaternion.Euler(0, newY, wobbles);


    }



}