using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    public static int health = 5;

    [SerializeField] //change the health in the inspector
    private int inspectorHealth = 5;

    void Start()
    {
        health = inspectorHealth;
    }
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameOverScreen GameOverScreen;

    bool isAlive = true;


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
