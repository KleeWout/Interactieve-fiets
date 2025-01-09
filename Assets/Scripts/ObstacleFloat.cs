using UnityEngine;

public class ObstacleFloat : MonoBehaviour
{
    // -6 tot 2

    public float minPosition = -6;
    public float maxPosition = 2;

    private float amplitude;
    private float shift;

    private float offset;
    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = 1 - (Random.value * 2);

        initialPosition = transform.position;
        amplitude = (maxPosition - minPosition) / 2;
        shift = (maxPosition + minPosition) / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.x = amplitude * Mathf.Sin(Time.time * offset) + shift;
        currentPosition.y = initialPosition.y - Mathf.Sin(Time.time * 3.129f + offset) * 0.06f;
        transform.position = currentPosition;
    }
}
