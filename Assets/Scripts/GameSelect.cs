using UnityEngine;
using models.GameMode;

public class GameSelect : MonoBehaviour
{
    public GameMode gameMode;




    public GameObject canoeSingleplayer;
    public GameObject canoeMultiplayer;
    public GameObject playerObject;

    public GameObject terrainObject;
    private TerrainGen terrain;


    void Start()
    {
        terrain = terrainObject.GetComponent<TerrainGen>();

        if (gameMode == GameMode.SinglePlayer)
        {
            LoadSinglePlayer();
        }
        else if (gameMode == GameMode.MultiPlayer)
        {
            LoadMultiPlayer();
        }
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
