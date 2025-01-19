using UnityEngine;
using Models.GameModeModel;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Collections;

public class GameSelect : MonoBehaviour
{
    public GameMode gameMode;
    public GameObject canoeSingleplayer;
    public GameObject canoeMultiplayer;
    public GameObject playerObject;

    public GameObject terrainObject;
    private TerrainGen terrain;


    private Animator singleplayerAnimator;
    private Animator multiplayerAnimator;

    public Transform cameraTransform;
    private Coroutine cameraTransitionCoroutine;

    private string gameCode;
    public static string userName;


    void Start()
    {
        singleplayerAnimator = canoeSingleplayer.GetComponent<Animator>();
        multiplayerAnimator = canoeMultiplayer.GetComponent<Animator>();
        terrain = terrainObject.GetComponent<TerrainGen>();

        if (gameMode == GameMode.SinglePlayer)
        {
            ChangeState(GameMode.SinglePlayer);
        }
        else if (gameMode == GameMode.MultiPlayer)
        {
            ChangeState(GameMode.MultiPlayer);
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

    public void ChangeState(GameMode mode)
    {
        if (cameraTransitionCoroutine != null)
        {
            StopCoroutine(cameraTransitionCoroutine);
        }
        if (mode == GameMode.SinglePlayer)
        {
            StartCoroutine(RotateAndSwitch(singleplayerAnimator, canoeMultiplayer, canoeSingleplayer));
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(2f,1.25f,0.15f),  Quaternion.Euler(25, -90, 0)));       
        }
        else if (mode == GameMode.MultiPlayer)
        {
            StartCoroutine(RotateAndSwitch(multiplayerAnimator, canoeSingleplayer, canoeMultiplayer));
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(0f,1.25f,-2.5f),  Quaternion.Euler(25, 0, 0)));
        }
    }

    private IEnumerator RotateAndSwitch(Animator toAnimator, GameObject fromObject, GameObject toObject)
    {
        ResetPlayer();
        fromObject.SetActive(false);
        toObject.SetActive(true);
        toAnimator.enabled = true;
        yield return new WaitForSeconds(0.5f);
        toAnimator.enabled = false;
    }
    private IEnumerator AnimateCamera(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        float elapsedTime = 0f;     

        while (elapsedTime < 0.5f)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 0.5f);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = targetPosition;
        cameraTransform.rotation = targetRotation;
    }


    void LoadSinglePlayer()
    {
        if(gameMode == GameMode.SinglePlayer)
        {
            return;
        }
        gameMode = GameMode.SinglePlayer;
   
        ChangeState(GameMode.SinglePlayer);

        // try
        // {
        //     terrain.GenerateTerrain(GameMode.SinglePlayer);
        // }
        // catch (System.Exception ex)
        // {
        //     Debug.LogError($"Error generating terrain: {ex.Message}");
        // }

    }

    void LoadMultiPlayer()
    {
        if(gameMode == GameMode.MultiPlayer)
        {
            return;
        }
        gameMode = GameMode.MultiPlayer;

        ChangeState(GameMode.MultiPlayer);

        // try
        // {
        //     terrain.GenerateTerrain(GameMode.MultiPlayer);
        // }
        // catch (System.Exception ex)
        // {
        //     Debug.LogError($"Error generating terrain: {ex.Message}");
        // }   
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
