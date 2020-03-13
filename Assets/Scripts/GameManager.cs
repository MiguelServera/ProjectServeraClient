using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using Assets.Scripts;

public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;

    private float spawnRate = 1.0f;
    private int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerNameText;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public bool isGameActive;
    public Text connectionsText;
    public Player player;
    public Game game;
    public Button quitButton;
    public Text gamesText;
    public int difficultySet;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        game = new Game();
        player = FindObjectOfType<Player>();
        playerNameText.text = player.Name + "(" + (DateTime.Now - player.BirthDay).Days / 365 + ")";
        scoreText.text = "Score: " + score.ToString();
        StartCoroutine(UpdateGet());
        StartCoroutine(GetAllGames());
    }

    public void StartGame(int difficulty)
    {
        game.DateStarted = DateTime.Now;
        difficultySet = difficulty;
        isGameActive = true;
        score = 0;
        UpdateScore(0);
        titleScreen.gameObject.SetActive(false);
        spawnRate /= difficulty;
        StartCoroutine(Helper.PlayerOnline(player));
        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int randomIndex = UnityEngine.Random.Range(0, 4);
            Instantiate(targets[randomIndex]);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene(3);
        StartCoroutine(DeleteFromBD());
    }

    public IEnumerator DeleteFromBD()
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/RemovePlayer", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerSerializable);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Delete user: " + httpClient.error);
            }

            Debug.Log("\n Delete user " + httpClient.responseCode);
        }
    }
    public IEnumerator UpdateGet()
    {
        while(true)
        {
            connectionsText.text = "";
            StartCoroutine(GetPlayerOnline());
            yield return new WaitForSeconds(3);
        }
    }
    public IEnumerator GetPlayerOnline()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/Online", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {
            string stringToRecieve = httpClient.downloadHandler.text;
            string json = "{\"players\":" + stringToRecieve + "}";
            PlayersList players = JsonUtility.FromJson<PlayersList>(json);
            foreach (PlayerSerializable p in players.players)
            {
                connectionsText.text += "Player " + p.Id.Substring(0, 5) + " " + p.Nickname + " <color=green> is Online! </color>" + "\n";
            }
        }

        httpClient.Dispose();
    }

    public IEnumerator GetAllPlayerOnline()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/AllGames", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {
            string stringToRecieve = httpClient.downloadHandler.text;
            string json = "{\"players\":" + stringToRecieve + "}";
            PlayersList players = JsonUtility.FromJson<PlayersList>(json);
            foreach (PlayerSerializable p in players.players)
            {
                connectionsText.text += "Player " + p.Id.Substring(0, 5) + " " + p.Nickname + " <color=green> is Online! </color>" + "\n";
            }
        }

        httpClient.Dispose();
    }

    public IEnumerator GetAllGames()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Game/AllGames", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("GetLastGames: " + httpClient.error);
        }
        else
        {
            string stringToRecieve = httpClient.downloadHandler.text;
            string jsonList = "{\"games\":" + stringToRecieve + "}";
            GamesList gameJ = JsonUtility.FromJson<GamesList>(jsonList);
            foreach (GameSerializable i in gameJ.games)
            {
                gamesText.text += "(" + i.Id.Substring(0, 5) + ") " + i.Nickname + ": " + i.Score + " points." + "\n";
            }
        }

        httpClient.Dispose();
    }
    public void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
        game.DateFinished = DateTime.Now;
        if (isGameActive) StartCoroutine(Helper.InsertGame(game, player, score, difficultySet));
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
