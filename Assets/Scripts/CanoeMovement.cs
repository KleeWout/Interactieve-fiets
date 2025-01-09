using UnityEngine;
using UnityEngine.InputSystem;
public class CanoeMovement : MonoBehaviour
{
    private Rigidbody rb;
    private EspListener espListener;
    private float userInput;
    private float userThrust;


    //boat animations
    public float wobbleMaxAngle;
    public float wobbleSmoothing;
    public float maxTiltAngle;
    public float tiltSmoothing;

    // boat movement speed
    public float steeringSpeed;
    public float thrustSpeed;

    // controller
    public bool controllerActivated = true;

    void Start()
    {
        espListener = GetComponent<EspListener>();
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {

        if (controllerActivated)
        {
            if (Gamepad.all.Count > 0)
            {
                userInput = Gamepad.all[0].leftStick.ReadValue().y - Gamepad.all[0].rightStick.ReadValue().y;
                userThrust = (Gamepad.all[0].leftStick.ReadValue().y + Gamepad.all[0].rightStick.ReadValue().y)/2;
            }

        }
        else
        {
            userInput = (espListener.valueLeft / 3) - (espListener.valueRight / 3);
            userThrust = ((espListener.valueLeft / 3) + (espListener.valueRight / 3)) / 2;
        }

        // boat animations (rotations left/right and wobble)
        float wobble = Mathf.Sin(Time.time * 2f) * wobbleMaxAngle;
        float newZ = userInput * maxTiltAngle;
        float currentZ = rb.rotation.eulerAngles.z;
        float smoothedZ = Mathf.LerpAngle(currentZ, newZ, Time.deltaTime * tiltSmoothing);
        float wobbles = Mathf.LerpAngle(smoothedZ, wobble, Time.deltaTime * wobbleSmoothing);
        float newY = rb.rotation.eulerAngles.y + userInput * steeringSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, newY, wobbles);

        // boat thurst (forward)
        Vector3 forceDirection = transform.forward * Mathf.Abs(userThrust * thrustSpeed);
        Vector3 forcePosition = transform.TransformPoint(new Vector3(0, 0, 1f));
        rb.AddForceAtPosition(forceDirection, forcePosition);

    }
}