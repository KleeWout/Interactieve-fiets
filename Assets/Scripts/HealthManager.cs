using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    private int health = 5;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Animator[] heartAnimators; // Add this line to reference the animators

    // public GameOverScreen GameOverScreen;
    
    public void TakeDamage()
    {
        health -= 1;
        if (health <= 0)
        {
            Debug.Log("gameover");
            // send gameover
        }
        else
            PlayHeartBounceAnimation();
            UpdateHearts();
    }
    public void GetHeart()
    {
        if (health < 5)
        {
            health += 1;
            UpdateHearts();
        }
    }
    void UpdateHearts()
    {
        foreach (Image img in hearts)
        {
            img.sprite = emptyHeart;
        }
        for (int i = 0; i < health; i++)
        {
            hearts[i].sprite = fullHeart;
        }
    }

    void PlayHeartBounceAnimation()
    {
        heartAnimators[health].SetTrigger("Bounce");
    }
}
