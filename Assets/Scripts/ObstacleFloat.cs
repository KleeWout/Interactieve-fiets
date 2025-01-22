using System.Collections;
using UnityEngine;

public class ObstacleFloat : MonoBehaviour
{
    public float speed; // Movement speed
    private float originalSpeed; // Store the original speed

    private Vector3 direction;
    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
        direction = GetRandomDirection();
        speed = Random.Range(0.1f, 1.5f); // Set speed to a random value between 0.04 and 2
        originalSpeed = speed; // Store the original speed
    }

    // Update is called once per frame
    void Update()
    {
        MoveObstacle();
    }

    private void MoveObstacle()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private Vector3 GetRandomDirection()
    {
        // Randomly choose left or right direction
        return Random.value > 0.5f ? Vector3.right : Vector3.left;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
        {
            direction = -direction;
            // StopCoroutine(AdjustSpeed());
            StopAllCoroutines();
            StartCoroutine(AdjustSpeed());
        }
    }
    private IEnumerator AdjustSpeed()
    {
        speed = 0.02f; // Drop the speed to half
        float elapsedTime = 0f;
        float duration = 50f; // Duration to regain original speed

        yield return new WaitForSeconds(0.2f);

        while (elapsedTime < duration)
        {
            speed = Mathf.Lerp(speed, originalSpeed, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        speed = originalSpeed; // Ensure the speed is set back to the original speed
    }
}