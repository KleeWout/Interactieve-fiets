using UnityEngine;
using Models.GameModeModel;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading;
using Models.WebSocketMessage;
using UnityEngine.SocialPlatforms.Impl;

public class GameSelect : MonoBehaviour
{
    public static GameMode gameMode;
    public GameObject canoeSingleplayer;
    public GameObject canoeMultiplayer;
    public GameObject playerObject;
    public HealthManager healthManager;
    public ScoreSystem scoreSystem;

    public GameObject hudCanvas;
    public TMP_Text gameCodeText;
    public RawImage qrDisplay;

    public GameObject terrainObject;
    private TerrainGen terrain;
    private CancellationTokenSource cts;



    private Animator singleplayerAnimator;
    private Animator multiplayerAnimator;

    public Transform cameraTransform;
    private Coroutine cameraTransitionCoroutine;

    private string gameCode;
    public static string userName;
    public static bool isGameStarted;
    public static bool isIdle = false;

    public TMP_Text ipAddress;

    void Start()
    {
        singleplayerAnimator = canoeSingleplayer.GetComponent<Animator>();
        multiplayerAnimator = canoeMultiplayer.GetComponent<Animator>();
        terrain = terrainObject.GetComponent<TerrainGen>();

        // terrain.GenerateTerrain(GameMode.MultiPlayer);
        // StartCoroutine(terrain.GenerateTerrain(GameMode.SinglePlayer));
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
        StartCoroutine(GetQrImageFromApi(code));
        StartCoroutine(GetWebLink());
        gameCodeText.text = code;
        gameCode = code;
    }
    IEnumerator GetQrImageFromApi(string code)
    {
        string url = "http://localhost:80/generateQR?code=" + code;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D qrTexture = DownloadHandlerTexture.GetContent(request);
            qrDisplay.texture = qrTexture;
        }
        else
        {
        }
    }
    IEnumerator GetWebLink(){
        string url = "http://localhost:80/api/getipaddress";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ip = request.downloadHandler.text;
            ipAddress.text = "http://" + ip;
            // Debug.Log("IP: " + ip);
        }
        else
        {
        }
    }

    // maak nog een ding spel gestart...

    public void ChangeState(GameMode mode)
    {
        if (cameraTransitionCoroutine != null)
        {
            StopCoroutine(cameraTransitionCoroutine);
        }
        if (mode == GameMode.SinglePlayer)
        {
            hudCanvas.SetActive(false);
            StartCoroutine(RotateAndSwitch(singleplayerAnimator, canoeMultiplayer, canoeSingleplayer));
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(3f, 0.1f, 0.7f), Quaternion.Euler(-20, -90, 0)));
        }
        else if (mode == GameMode.MultiPlayer)
        {
            hudCanvas.SetActive(false);
            StartCoroutine(RotateAndSwitch(multiplayerAnimator, canoeSingleplayer, canoeMultiplayer));
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(1.65f, 0.1f, -2.4f), Quaternion.Euler(-20, -20, 0)));
        }
        else if (mode == GameMode.Menu)
        {
            hudCanvas.SetActive(false);
            canoeSingleplayer.SetActive(false);
            canoeMultiplayer.SetActive(false);
            ResetPlayer();
            // reset terrain;
            cts?.Cancel();
            cts = new CancellationTokenSource();
            terrain.GenerateTerrain(GameMode.MultiPlayer, cts.Token);
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(0, 2, 0), Quaternion.Euler(-90, 0, 0)));
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

    public void StartGame()
    {
        playerObject.transform.rotation = Quaternion.identity;
        isGameStarted = true;
        HealthManager.health = 5;
        healthManager.UpdateHearts();
        hudCanvas.SetActive(true);
        if (gameMode == GameMode.SinglePlayer)
        {
            canoeSingleplayer.SetActive(true);
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(3.8f, 5f, 0.15f), Quaternion.Euler(50, -90, 0)));
        }
        else if (gameMode == GameMode.MultiPlayer)
        {
            canoeMultiplayer.SetActive(true);
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(0f, 1.25f, -4f), Quaternion.Euler(0, 0, 0)));
        }
    }


    public void LoadSinglePlayer()
    {
        if (!isGameStarted)
        {
            if (gameMode == GameMode.SinglePlayer)
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                terrain.GenerateTerrain(GameMode.SinglePlayer, cts.Token);
                return;
            }
            gameMode = GameMode.SinglePlayer;

            ChangeState(GameMode.SinglePlayer);


            cts?.Cancel();
            cts = new CancellationTokenSource();
            terrain.GenerateTerrain(GameMode.SinglePlayer, cts.Token);
        }

    }

    public void LoadMultiPlayer()
    {
        if (!isGameStarted)
        {
            if (gameMode == GameMode.MultiPlayer)
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                terrain.GenerateTerrain(GameMode.MultiPlayer, cts.Token);
                return;
            }
            gameMode = GameMode.MultiPlayer;

            ChangeState(GameMode.MultiPlayer);

            cts?.Cancel();
            cts = new CancellationTokenSource();
            terrain.GenerateTerrain(GameMode.MultiPlayer, cts.Token);
        }
    }


    public void ResetPlayer()
    {
        canoeMultiplayer.SetActive(false);
        canoeSingleplayer.SetActive(false);
        scoreSystem.ResetScore();

        playerObject.transform.position = Vector3.zero;
        playerObject.transform.rotation = Quaternion.identity;

        Rigidbody playerRb = playerObject.GetComponent<Rigidbody>();
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;


    //         scoreSystem.ResetScore();
    // Rigidbody playerRb = playerObject.GetComponent<Rigidbody>();
    // playerRb.linearVelocity = Vector3.zero;
    // playerRb.angularVelocity = Vector3.zero;

    // playerObject.transform.position = Vector3.zero;
    // playerObject.transform.rotation = Quaternion.identity;

    // // Reset the local rotation of the camera
    // cameraTransform.localRotation = Quaternion.identity;

    }
}
