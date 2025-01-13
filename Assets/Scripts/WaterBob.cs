using UnityEngine;

public class WaterBob : MonoBehaviour
{
    [SerializeField]
    float height = 0.1f;

    [SerializeField]
    float period = 1;

    private Vector3 initialPosition;
    private float offset;
    private Rigidbody rb;

    private void Awake()
    {
        initialPosition = transform.position;
        offset = 1 - (Random.value * 2);
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = initialPosition.y - Mathf.Sin((Time.time + offset) * period) * height;
        transform.position = currentPosition;
    }
}