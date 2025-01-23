using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;
using Models.WebSocketMessage;
public class CanoeMovement : MonoBehaviour
{
    private Rigidbody rb;
    private EspListener espListener;
    private GameObject player;
    private Rigidbody rbPlayer;


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

    private float zeroInputStartTime = -1;
    private float timeOutTime = 20f;

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
        espListener = GetComponentInParent<EspListener>();
        rb = GetComponent<Rigidbody>();
        player = transform.parent.gameObject;// Get the parent transform
        rbPlayer = player.GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if (GameSelect.isGameStarted)
        {
            if (inputType == InputType.Controller)
            {
                if (Gamepad.all.Count > 0)
                {
                    leftAnimation = Gamepad.all[0].leftStick.ReadValue().y;
                    rightAnimation = Gamepad.all[0].rightStick.ReadValue().y;
                    userInput = Gamepad.all[0].leftStick.ReadValue().y - Gamepad.all[0].rightStick.ReadValue().y;
                    userThrust = (Gamepad.all[0].leftStick.ReadValue().y + Gamepad.all[0].rightStick.ReadValue().y) / 2;
                }

            }
            else if (inputType == InputType.Keyboard)
            {
                userInput = Input.GetAxis("Horizontal");
                userThrust = Input.GetAxis("Vertical");
                if (isSinglePlayer)
                {
                    userInput = 0;
                    userThrust = Input.GetAxis("Vertical");
                }
            }
            else if (!isSinglePlayer)
            {
                leftAnimation = espListener.valueLeft;
                rightAnimation = espListener.valueRight;
                userInput = (espListener.valueLeft / 1.5f) - (espListener.valueRight / 1.5f);
                userThrust = (espListener.valueLeft + espListener.valueRight) / 2;

            }
            else
            {
                userInput = 0;
                userThrust = (espListener.valueLeft) + (espListener.valueRight);
            }
            if(!GameSelect.isIdle){
                zeroInputStartTime = -1f;
            }
            else if (userThrust <= 0.05)
            {
                if (zeroInputStartTime < 0)
                {
                    zeroInputStartTime = Time.time;
                }
                else if (Time.time - zeroInputStartTime >= timeOutTime)
                {
                    // user has been idle too long
                    WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { NewConnection = true });
                    GameSelect.isGameStarted = false;
                    GameSelect.isIdle = false;
                    zeroInputStartTime = -1f;
                }
            }
            else
            {
                zeroInputStartTime = -1f;
            }
        }
        else
        {
            userInput = 0;
            userThrust = 0;
        }



        // boat animations (rotations left/right and wobble)
        float wobble = Mathf.Sin(Time.time * 2f) * wobbleMaxAngle;
        float newZ = userInput * maxTiltAngle;
        float currentZ = rb.rotation.eulerAngles.z;
        float smoothedZ = Mathf.LerpAngle(currentZ, newZ, Time.deltaTime * tiltSmoothing);
        float wobbles = Mathf.LerpAngle(smoothedZ, wobble, Time.deltaTime * wobbleSmoothing);
        float newY = rb.rotation.eulerAngles.y + userInput * steeringSpeed * Time.deltaTime;
        // transform.rotation = Quaternion.Euler(0, newY, wobbles);

        player.transform.rotation = Quaternion.Euler(0, newY, player.transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, wobbles);

        // boat thurst (forward)
        Vector3 forceDirection = transform.forward * (userThrust * thrustSpeed);
        Vector3 forcePosition = transform.TransformPoint(new Vector3(0, 0, 1f));
        rbPlayer.AddForceAtPosition(forceDirection, forcePosition);

    }
}