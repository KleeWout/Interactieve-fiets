using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{

    private HealthManager HealthManager;
    private float lastDamageTime;
    private float lastHealingTime;
    public  float damageCooldown = 2f;

    void Start(){
        HealthManager = GetComponent<HealthManager>();
        lastDamageTime = Time.time - damageCooldown;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Lifesaver" && Time.time - lastHealingTime > damageCooldown)
        {
            lastDamageTime = Time.time;
            HealthManager.GetHeart();
            StartCoroutine(DisableAfterDelay(collision.gameObject, 0.2f)); // Disable after 0.5s
        }
        else if(Time.time - lastDamageTime < damageCooldown){
            return;
        }
        else{
            lastDamageTime = Time.time;
            HealthManager.TakeDamage();
        }

    }
    IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
