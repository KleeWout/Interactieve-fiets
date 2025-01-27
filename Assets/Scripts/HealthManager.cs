using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static int health = 5;
    public GameObject canoeMultiplayer;
    public GameObject canoeSingleplayer;

    public Material[] materials;
    public Material outsideMaterial;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Animator[] heartAnimators; // Add this line to reference the animators

    public GameObject gameOverScreen; // Add this line to reference the GameOverScreen

    // public GameOverScreen GameOverScreen;    



    public void TakeDamage()
    {
        if(GameSelect.gameMode == Models.GameModeModel.GameMode.SinglePlayer){
            StartCoroutine(WaitAndShowGameOverScreen());
        }
        else{
            health -= 1;
            if (health <= 0)
            {
                StartCoroutine(WaitAndShowGameOverScreen());
            }
            else
                PlayHeartBounceAnimation();
                UpdateHearts();

            StartCoroutine(FlashMaterials());
        }

    }

    public IEnumerator WaitAndShowGameOverScreen()
    {
        foreach(Material material in materials)
        {
            material.renderQueue = 1;
        }
        if(canoeMultiplayer.activeSelf)
        {
            canoeMultiplayer.GetComponent<WaterBob>().isSunk = true;
            yield return new WaitForSeconds(1f);
            gameOverScreen.SetActive(true);
            canoeMultiplayer.GetComponent<WaterBob>().isSunk = false;


        }
        else if(canoeSingleplayer.activeSelf)
        {
            canoeSingleplayer.GetComponent<WaterBob>().isSunk = true;
            yield return new WaitForSeconds(1f);
            gameOverScreen.SetActive(true);
            canoeSingleplayer.GetComponent<WaterBob>().isSunk = false;
        }
        foreach(Material material in materials)
        {
            material.renderQueue = 5000;
        }

    }

    public void GetHeart()
    {
        if (health < 5)
        {
            health += 1;
            UpdateHearts();
        }
    }
    public void UpdateHearts()
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
    private IEnumerator FlashMaterials()
    {
        var colorChange = materials[0].color;
        var startColor = outsideMaterial.color;
        outsideMaterial.color = colorChange;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = startColor;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = colorChange;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = startColor;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = colorChange;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = startColor;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = colorChange;
        yield return new WaitForSeconds(0.25f);
        outsideMaterial.color = startColor;
        yield return new WaitForSeconds(0.25f);
    }
}
