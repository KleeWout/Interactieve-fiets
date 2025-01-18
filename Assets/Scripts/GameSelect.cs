using UnityEngine;
using Models.GameModeModel;
using System.ComponentModel;
using System.Net.WebSockets;

public class GameSelect : MonoBehaviour
{
    public GameMode gameMode;
    public GameObject canoeSingleplayer;
    public GameObject canoeMultiplayer;
    public GameObject playerObject;

    public GameObject terrainObject;
    private TerrainGen terrain;



    private string gameCode;


    void Start()
    {
        terrain = terrainObject.GetComponent<TerrainGen>();

        if (gameMode == GameMode.SinglePlayer)
        {
            canoeSingleplayer.SetActive(true);
            canoeMultiplayer.SetActive(false);
        }
        else if (gameMode == GameMode.MultiPlayer)
        {
            canoeSingleplayer.SetActive(false);
            canoeMultiplayer.SetActive(true);
        }
        terrain.GenerateTerrain(gameMode);
    }

    public void SwitchGameMode(GameMode mode)
    {
        if (mode == GameMode.SinglePlayer)
        {
            LoadSinglePlayer();
        }
        else if (mode == GameMode.MultiPlayer)
        {
            LoadMultiPlayer();
        }
    }

    public void SetGameCode(string code)
    {
        Debug.Log("Setting game code to: " + code);
        gameCode = code;
    }



    void LoadSinglePlayer()
    {
        if(gameMode == GameMode.SinglePlayer)
        {
            return;
        }
        gameMode = GameMode.SinglePlayer;
        canoeMultiplayer.SetActive(false);
        ResetPlayer();
        canoeSingleplayer.SetActive(true);

        terrain.GenerateTerrain(GameMode.SinglePlayer);

    }

    void LoadMultiPlayer()
    {
        if(gameMode == GameMode.MultiPlayer)
        {
            return;
        }
        gameMode = GameMode.MultiPlayer;
        canoeSingleplayer.SetActive(false);
        ResetPlayer();
        canoeMultiplayer.SetActive(true);

        terrain.GenerateTerrain(GameMode.MultiPlayer);
    }

    void ResetPlayer()
    {
        Rigidbody playerRb = playerObject.GetComponent<Rigidbody>();
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        playerObject.transform.position = Vector3.zero;
        playerObject.transform.rotation = Quaternion.identity;

    }

}
