using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{

    private HealthManager HealthManager;

    void Start(){
        HealthManager = GetComponent<HealthManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        HealthManager.TakeDamage();
        //when hitting the logs
        if (collision.collider.tag == "Obstacle")
        {
            HealthManager.TakeDamage();
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
