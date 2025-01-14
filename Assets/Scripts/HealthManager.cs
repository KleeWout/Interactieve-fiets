using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    public static int health = 5;
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameOverScreen GameOverScreen;

    bool isAlive = true;

    void Awake()
    {
        health = 5;
    }
    void Update()
    {
        foreach (Image img in hearts)
        {
            img.sprite = emptyHeart;
        }
        for (int i = 0; i < health; i++)
        {
            hearts[i].sprite = fullHeart;
        }
        if (health <= 0 && isAlive)
        {
            isAlive = false;
            GameOverScreen.Setup();
        }
    }
}
