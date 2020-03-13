using Assets.Scripts;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Helper : MonoBehaviour
{
    internal static IEnumerator InitializeToken(string email, string password)
    {
        Player player = FindObjectOfType<Player>();
        if (string.IsNullOrEmpty(player.Token))
        {
            UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/Token", "POST");

            WWWForm dataToSend = new WWWForm();
            dataToSend.AddField("grant_type", "password");
            dataToSend.AddField("username", email);
            dataToSend.AddField("password", password);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Accept", "application/json");
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Helper > InitToken: " + httpClient.error);
            }
            else
            {
                string jsonResponse = httpClient.downloadHandler.text;
                AuthorizationToken authToken = JsonUtility.FromJson<AuthorizationToken>(jsonResponse);
                player.Token = authToken.access_token;
            }
            httpClient.Dispose();
        }
    }

    internal static IEnumerator GetPlayerId()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Account/UserId", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerId: " + httpClient.error);
        }
        else
        {
            player.Id = httpClient.downloadHandler.text.Replace("\"", "");
        }

        httpClient.Dispose();
    }

    internal static IEnumerator GetPlayerInfo()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/Info", "GET");

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
            PlayerSerializable playerSerializable = JsonUtility.FromJson<PlayerSerializable>(stringToRecieve);
            player.Id = playerSerializable.Id;
            player.Name = playerSerializable.Name;
            player.Nickname = playerSerializable.Nickname;
            player.Email = playerSerializable.Email;
            player.Country = playerSerializable.Country;
            player.BirthDay = DateTime.Parse(playerSerializable.BirthDay);
            player.BlobUri = playerSerializable.BlobUri;
        }

        httpClient.Dispose();
    }

    internal static IEnumerator PlayerOnline(Player player)
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.Nickname = player.Nickname;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/RegisterOnline", "POST"))
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
                throw new Exception("RegisterPlayerOnline: " + httpClient.error);
            }

            Debug.Log("\nRegisterOnline: " + httpClient.responseCode);
        }
    }

    internal static IEnumerator InsertGame(Game game, Player player, int score, int difficulty)
    {
        GameSerializable newGame = new GameSerializable();
        newGame.Id = player.Id;
        newGame.Nickname = player.Nickname;
        newGame.Score = score.ToString();
        newGame.Difficulty = difficulty.ToString();
        newGame.DateStarted = game.DateStarted.ToString();
        newGame.DateFinished = game.DateFinished.ToString();

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Game/RegisterGame", "POST"))
        {
            string playerData = JsonUtility.ToJson(newGame);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("RegisterNewGame>" + httpClient.error);
            }

            Debug.Log("\nRegisterGame: " + httpClient.responseCode);
        }
    }

    internal static IEnumerator DeleteAccount(Player player)
    {
        PlayerSerializable playerAccount = new PlayerSerializable();
        playerAccount.Id = player.Id;
        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/RemovePlayerBD", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerAccount);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("RegisterNewGame>" + httpClient.error);
            }

            Debug.Log("\nDelete my account from players " + httpClient.responseCode);
        }
    }

    internal static IEnumerator DeleteAccountAsp(Player player)
    {
        PlayerSerializable account = new PlayerSerializable();
        account.Id = player.Id;
        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Account/RemovePlayerAccount", "POST"))
        {
            string playerData = JsonUtility.ToJson(account);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Delete My Account: " + httpClient.error);
            }
            Debug.Log("\nDelete my account from ASP: " + httpClient.responseCode);
            SceneManager.LoadScene(0);
        }
    }
}
