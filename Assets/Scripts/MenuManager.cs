using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Player player;
    public Text nameText;
    public Button clickyButton;
    public Button profileButton;
    public Button logout;
    public Text infoGames;
    public Text topScore;
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        player = FindObjectOfType<Player>();
        nameText.text = "Welcome to the <color=black>Clicky Crates</color> game, <color=blue>" + player.Name + "</color>!";
        StartCoroutine(GetLastGames());
        StartCoroutine(GetTopScore());
    }

    public void onProfileButtonClicked()
    {
        SceneManager.LoadScene(2);
    }
    public void onClickyButtonClicked()
    {
        SceneManager.LoadScene(3);
    }

    public void onLogoutButtonClicked()
    {
        player.Token = "";
        player.Id = "";
        player.Name = "";
        player.Nickname = "";
        player.Country = "";
        player.Email = "";
        player.BlobUri = "";
        player.BirthDay = DateTime.MinValue;
        SceneManager.LoadScene(0);
    }

    public IEnumerator GetLastGames()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Game/GameInfo", "GET");

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
                infoGames.text += "(" + i.Id.Substring(0, 5) + ") " + i.Nickname + ": " + i.Score + " points." + "\n" + "Date Started:(" + DateTime.Parse(i.DateStarted) + ")" + "\n" + "Date Finished:(" + DateTime.Parse(i.DateFinished) + ")" + "\n";
            }
        }

        httpClient.Dispose();
    }

    public IEnumerator GetTopScore()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Game/TopScore", "GET");

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
            GameSerializable gameJ = JsonUtility.FromJson<GameSerializable>(stringToRecieve);
            topScore.text = "Your top score ever is <color=red>" + gameJ.Score + "</color>!";
        }

        httpClient.Dispose();
    }
}
