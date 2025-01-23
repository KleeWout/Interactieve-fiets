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
    public GameMode gameMode;
    public GameObject canoeSingleplayer;
    public GameObject canoeMultiplayer;
    public GameObject playerObject;

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
            Debug.LogError("Failed to download QR code: " + request.error);
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
            StartCoroutine(RotateAndSwitch(singleplayerAnimator, canoeMultiplayer, canoeSingleplayer));
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(2f, 1.25f, 0.15f), Quaternion.Euler(25, -90, 0)));
        }
        else if (mode == GameMode.MultiPlayer)
        {
            StartCoroutine(RotateAndSwitch(multiplayerAnimator, canoeSingleplayer, canoeMultiplayer));
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(0f, 1.25f, -2.5f), Quaternion.Euler(25, 0, 0)));
        }
        else if (mode == GameMode.Menu)
        {
            hudCanvas.SetActive(false);
            canoeSingleplayer.SetActive(false);
            canoeMultiplayer.SetActive(false);
            ResetPlayer();
            TerrainGen.currentlyLoadedGameMode = GameMode.Menu;
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

    public void StartGame(GameMode mode)
    {
        isGameStarted = true;
        hudCanvas.SetActive(true);
        if (mode == GameMode.SinglePlayer)
        {
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(3.8f, 5f, 0.15f), Quaternion.Euler(50, -90, 0)));
        }
        else if (mode == GameMode.MultiPlayer)
        {
            cameraTransitionCoroutine = StartCoroutine(AnimateCamera(new Vector3(0f, 1.25f, -4f), Quaternion.Euler(0, 0, 0)));
        }
    }


    void LoadSinglePlayer()
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

    void LoadMultiPlayer()
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


    void ResetPlayer()
    {
        ScoreSystem.score = 0f;
        Rigidbody playerRb = playerObject.GetComponent<Rigidbody>();
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        playerObject.transform.position = Vector3.zero;
        playerObject.transform.rotation = Quaternion.identity;

    }
}
