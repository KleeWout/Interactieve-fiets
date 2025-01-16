using UnityEngine;
using models.GameMode;

public class GameSelect : MonoBehaviour
{
    public GameMode gameMode;
    private bool switchSingleplayer = false;
    private bool switchMultiplayer = false;




    public GameObject canoeSingleplayer;
    public GameObject canoeMultiplayer;
    public GameObject playerObject;

    public GameObject terrainObject;


    void Start()
    {

        if (gameMode == GameMode.SinglePlayer)
        {
            LoadSinglePlayer();
            switchMultiplayer = true;
        }
        else if (gameMode == GameMode.MultiPlayer)
        {
            LoadMultiPlayer();
            switchSingleplayer = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMode == GameMode.SinglePlayer && switchSingleplayer)
        {
            Debug.Log("Single Player");
            switchSingleplayer = false;
            switchMultiplayer = true;
            LoadSinglePlayer();

        }
        else if (gameMode == GameMode.MultiPlayer && switchMultiplayer)
        {
            Debug.Log("Multi Player");
            switchSingleplayer = true;
            switchMultiplayer = false;
            LoadMultiPlayer();

        }
    }



    void LoadSinglePlayer()
    {
        canoeMultiplayer.SetActive(false);
        ResetPlayer();
        canoeSingleplayer.SetActive(true);


        // terrain.GenerateTerrain(gameMode);
        // loadPlayer.InitializeStart();

    }

    void LoadMultiPlayer()
    {
        canoeSingleplayer.SetActive(false);
        ResetPlayer();
        canoeMultiplayer.SetActive(true);



        // terrain.GenerateTerrain(gameMode);
        // loadPlayer.InitializeStart();
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
