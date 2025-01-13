using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        //when hitting the logs
        if (collision.collider.tag == "Obstacle")
        {
            HealthManager.health--;
            if (HealthManager.health <= 0)
            {
                Debug.Log("Game Over!");
            }
            else
            {
                StartCoroutine(GetHurt());
            }


        }
        Debug.Log(collision.collider.tag);

    }
    IEnumerator GetHurt()
    {

        Physics.IgnoreLayerCollision(9, 10, true);
        // GetComponent<Animator>().SetLayerWeight(1, 1);
        Debug.Log("Ignoring collision");
        yield return new WaitForSeconds(2);
        // GetComponent<Animator>().SetLayerWeight(1, 0);
        // Debug.Log("Turning off animation");
        Physics.IgnoreLayerCollision(9, 10, false);
    }
}
