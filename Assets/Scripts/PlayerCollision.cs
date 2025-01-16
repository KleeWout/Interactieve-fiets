using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{

    private HealthManager HealthManager;
    private float lastDamageTime;
    public  float damageCooldown = 2f;

    void Start(){
        HealthManager = GetComponent<HealthManager>();
        lastDamageTime = Time.time - damageCooldown;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(Time.time - lastDamageTime < damageCooldown){
            return;
        }
        lastDamageTime = Time.time;
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
