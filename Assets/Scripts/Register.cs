using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Globalization;
public class Register : MonoBehaviour
{
    // Cached references
    public InputField nameInputField;
    public InputField nicknameInputField;
    public InputField birthdateInputField;
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public InputField countryInputField;
    public Button registerButton;
    public Player player;
    public Image avatarImage;
    public Text debugText;

    //Search for the gameObject "Player"
    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(LoadImage());
    }
    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterNewUser());
    }

    public IEnumerator LoadImage()
    {
        using (UnityWebRequest httpClient = new UnityWebRequest("https://clickystorage.blob.core.windows.net/clickycrates-blobs/DevilPika.png"))
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
    private IEnumerator RegisterNewUser()
    {
        yield return RegisterUser();
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);  //Sets player.Token
        Debug.Log("\nToken: " + player.Token.Substring(0, 7) + "...");
        yield return Helper.GetPlayerId();  //Sets player.Id
        Debug.Log("\nId: " + player.Id);
        player.Email = emailInputField.text;
        player.Name = nameInputField.text;
        player.Nickname = nicknameInputField.text;
        player.Country = countryInputField.text;
        player.BirthDay = DateTime.Parse(birthdateInputField.text);
        player.BlobUri = "https://clickystorage.blob.core.windows.net/clickycrates-blobs/DevilPika.png";
        yield return InsertPlayer();
        debugText.text = ($"\nPlayer \"{player.Name}\" registered.");
        player.Id = string.Empty;
        player.Token = string.Empty;
        player.Name = string.Empty;
        player.Email = string.Empty;
        player.BirthDay = DateTime.MinValue;
    }

    private IEnumerator RegisterUser()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Account/Register", "POST");

        UserRegister newUser = new UserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(newUser);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
        //Error, it's Content-type, not Content-Type
        httpClient.SetRequestHeader("Content-type", "application/json");
        //We need the certificate to return true, so we create a class that will always return true.
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("OnRegisterButtonClick: Error > " + httpClient.error);
        }

        Debug.Log("\nOnRegisterButtonClick: " + httpClient.responseCode);

        httpClient.Dispose();
    }

    private IEnumerator InsertPlayer()
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.Name = player.Name;
        playerSerializable.Nickname = player.Nickname;
        playerSerializable.Email = player.Email;
        playerSerializable.Country = player.Country;
        playerSerializable.BirthDay = player.BirthDay.ToString();
        playerSerializable.BlobUri = player.BlobUri;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/RegisterPlayer", "POST"))
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
                throw new Exception("RegisterNewPlayer > InsertPlayer: " + httpClient.error);
            }

            Debug.Log("\nRegisterNewPlayer > InsertPlayer: " + httpClient.responseCode);
        }

    }
}
