using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button logoutButton;
    //public Button playGameButton;
    public Text messageBoardText;
    public Player player;
    public Image avatarImage;

    //Search for the gameObject "Player"
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);
        yield return Helper.GetPlayerInfo();
        SceneManager.LoadScene(1);
    }
}
