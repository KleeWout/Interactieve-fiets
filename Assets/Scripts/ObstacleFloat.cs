using System.Collections;
using UnityEngine;

public class ObstacleFloat : MonoBehaviour
{
    public float speed; // Movement speed
    private float originalSpeed; // Store the original speed
    private Animator animator;

    private Vector3 direction;
    private Vector3 initialPosition;

    private Vector3? firstCollisionLeft = null;
    private Vector3? firstCollisionRight = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
        direction = GetRandomDirection();
        speed = Random.Range(0.5f, 4f); // Set speed to a random value between 0.04 and 2
        originalSpeed = speed; // Store the original speed

        animator = GetComponent<Animator>();
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
            StopAllCoroutines();
            StartCoroutine(AdjustSpeed());
            if (firstCollisionLeft == null && direction.x < 0)
            {
                firstCollisionLeft = collision.contacts[0].point;
            }
            else if (firstCollisionRight == null && direction.x > 0)
            {
                firstCollisionRight = collision.contacts[0].point;
            }

            if (firstCollisionLeft.HasValue && firstCollisionRight.HasValue)
            {
                float distance = Vector3.Distance(firstCollisionLeft.Value, firstCollisionRight.Value);
                if (distance < 4f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (collision.collider.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.enabled = true;
            }
        }
    }
    private IEnumerator AdjustSpeed()
    {
        speed = 0.02f; // Drop the speed to half
        float elapsedTime = 0f;
        float duration = 260f; // Duration to regain original speed

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