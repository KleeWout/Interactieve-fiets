using UnityEngine;
using models.GameMode;

public class LoadPlayer : MonoBehaviour
{
    private Transform mainCameraTransform;
    private Transform canoeTransform;
    private Vector3 initialPosition = new Vector3(2, 1.25f, 3);
    private Quaternion initialRotation = Quaternion.Euler(6, -145, 0);
    private Vector3 targetPosition = new Vector3(0, 1.25f, -4);
    private Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
    private float lerpDuration = 1.0f;
    private float elapsedTime = 0.0f;
    private bool isLerping;
    private bool hasLerped;

    public void InitializeStart()
    {
        ResetPlayer();
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);

        isLerping = false;
        hasLerped = false;

        mainCameraTransform.position = initialPosition;
        mainCameraTransform.rotation = initialRotation;

        // canoeTransform.position = new Vector3(0, 0, 0);
        // canoeTransform.rotation = new Quaternion(0, 0, 0, 0);

    }
    void Start()
    {
        mainCameraTransform = transform.Find("Main Camera");
        canoeTransform = transform.Find("Canoe");
        InitializeStart();
    }

    void Update()
    {
        if (transform.position.z != 0 && !isLerping)
        {
            // Start lerping
            isLerping = true;
            elapsedTime = 0.0f; // Reset elapsed time
        }

        if (isLerping && !hasLerped)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpDuration;

            mainCameraTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            mainCameraTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            if (t >= 1.0f)
            {
                isLerping = false;
                hasLerped = true;
            }
        }
    }

    private void ResetPlayer()
    {
        // Destroy the current player object
        Destroy(canoeTransform.gameObject);

        // Create a new canoe object
        GameObject canoe = Instantiate(Resources.Load("Canoe")) as GameObject;
    }
}