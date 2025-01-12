using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;
public class CanoeMovement : MonoBehaviour
{
    private Rigidbody rb;
    private EspListener espListener;
    private float userInput;
    private float userThrust;

    public bool isSinglePlayer = false;


    //boat animations
    public float wobbleMaxAngle;
    public float wobbleSmoothing;
    public float maxTiltAngle;
    public float tiltSmoothing;

    // boat movement speed
    public float steeringSpeed;
    public float thrustSpeed;

    // input
    public enum InputType
    {
        Keyboard,
        Controller,
        ESP
    }
    public InputType inputType;

    // peddle animation values
    internal float leftAnimation;
    internal float rightAnimation;

    void Start()
    {
        espListener = GetComponent<EspListener>();
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {

        if (inputType == InputType.Controller)
        {
            if (Gamepad.all.Count > 0)
            {
                leftAnimation = Gamepad.all[0].leftStick.ReadValue().y;
                rightAnimation = Gamepad.all[0].rightStick.ReadValue().y;
                userInput = Gamepad.all[0].leftStick.ReadValue().y - Gamepad.all[0].rightStick.ReadValue().y;
                userThrust = (Gamepad.all[0].leftStick.ReadValue().y + Gamepad.all[0].rightStick.ReadValue().y)/2;  
            }

        }
        else if(inputType == InputType.Keyboard)
        {
            userInput = Input.GetAxis("Horizontal");
            userThrust = Input.GetAxis("Vertical");
        }
        else if(!isSinglePlayer)
        {
            leftAnimation = espListener.valueLeft;
            rightAnimation = espListener.valueRight;
            userInput = (espListener.valueLeft / 1.5f) - (espListener.valueRight / 1.5f);
            userThrust = ((espListener.valueLeft / 3) + (espListener.valueRight / 3)) / 2;

        }
        else
        {
            userInput = 0;
            userThrust = (espListener.valueLeft / 3) + (espListener.valueRight / 3);
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
        Vector3 forceDirection = transform.forward * (userThrust * thrustSpeed);
        Vector3 forcePosition = transform.TransformPoint(new Vector3(0, 0, 1f));
        rb.AddForceAtPosition(forceDirection, forcePosition);

    }
}