using UnityEngine;
using models.GameMode;

public class GameSelect : MonoBehaviour
{
    TerrainGen terrain;
    LoadPlayer loadPlayer;
    public GameMode gameMode;
    private bool switchSingleplayer = false;
    private bool switchMultiplayer = false;

    void Start()
    {
        terrain = GameObject.Find("Terrains").GetComponent<TerrainGen>();
        loadPlayer = GameObject.Find("Player").GetComponent<LoadPlayer>();
        if (terrain == null)
        {
            Debug.LogError("Terrains GameObject or TerrainGen component not found.");
            return;
        }

        if(gameMode == GameMode.SinglePlayer){
            LoadSinglePlayer();
            switchMultiplayer = true;
        }
        else if(gameMode == GameMode.MultiPlayer){
            LoadMultiPlayer();
            switchSingleplayer = true;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameMode == GameMode.SinglePlayer && switchSingleplayer)
        {
            Debug.Log("Single Player");
            switchSingleplayer = false;
            switchMultiplayer = true;
            LoadSinglePlayer();

        }
        else if(gameMode == GameMode.MultiPlayer && switchMultiplayer)
        {
            Debug.Log("Multi Player");
            switchSingleplayer = true;
            switchMultiplayer = false;
            LoadMultiPlayer();

        }
    }



    void LoadSinglePlayer()
    {
        terrain.GenerateTerrain(gameMode);
        loadPlayer.InitializeStart();
    }

    void LoadMultiPlayer()
    {
        terrain.GenerateTerrain(gameMode);
        loadPlayer.InitializeStart();
    }

}
