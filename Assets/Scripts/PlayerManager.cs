using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Player player;
    public InputField nickName;
    public InputField dateOfBirth;
    public InputField country;
    public InputField playerName;
    public Button editButton;
    public Button updateButton;
    public Text textId;
    public Image avatarImage;
    public Text debugText;
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        player = FindObjectOfType<Player>();
        textId.text = "(" + player.Id.Substring(0,5) + ")";
        playerName.text = player.Name;
        dateOfBirth.text = player.BirthDay.ToString();
        country.text = player.Country;
        nickName.text = player.Nickname;
        StartCoroutine(LoadImage());
        playerName.interactable = false;
        nickName.interactable = false;
        dateOfBirth.interactable = false;
        country.interactable = false;
    }

    public void OnEditButtonClick()
    {
        playerName.interactable = true;
        nickName.interactable = true;
        nickName.image.enabled = true;
        dateOfBirth.interactable = true;
        country.interactable = true;
    }

    public void onUpdateButtonClick()
    {
        StartCoroutine(UpdateAndGet());
    }

    public IEnumerator LoadImage()
    {
        using (UnityWebRequest httpClient = new UnityWebRequest(player.BlobUri))
        {
            httpClient.downloadHandler = new DownloadHandlerTexture();
            yield return httpClient.SendWebRequest();
            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                Debug.Log(httpClient.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(httpClient);
                avatarImage.sprite = Sprite.Create(texture,
                                                    new Rect(0.0f, 0.0f, texture.width, texture.height),
                                                    new Vector2(0.5f, 0.5f),
                                                    100.0f);
            }
        }
    }
    public IEnumerator UpdateAndGet()
    {
        StartCoroutine(UpdatePlayer());
        yield return new WaitForSeconds(2);
        StartCoroutine(Helper.GetPlayerInfo());
    }
    private IEnumerator UpdatePlayer()
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        if(playerName.text != "") playerSerializable.Name = playerName.text;
        if (dateOfBirth.text != "") playerSerializable.BirthDay = dateOfBirth.text;
        if(nickName.text != "") playerSerializable.Nickname = nickName.text;
        if(country.text != "") playerSerializable.Country = country.text;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/UpdatePlayer", "POST"))
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
                throw new Exception("UpdatePlayer: " + httpClient.error);
            }

            debugText.text = "You updated your profile successfully";
        }

    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void onDeleteButtonClicked()
    {
        StartCoroutine(Helper.DeleteAccount(player));
        StartCoroutine(Helper.DeleteAccountAsp(player));
    }
}
