using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainmenu;
    public GameObject multiplaymenu1;
    public GameObject loading;
    public GameObject joinMenu;

    public TMP_InputField nameInputField;
    public InputField ipInput;
    public static string DisplayName;

    public MyNetworkLobby myNetworkLobby;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        mainmenu.SetActive(true);
        multiplaymenu1.SetActive(false);
        loading.SetActive(false);
    }

    public void StartSinglePlayer()
    {
        mainmenu.SetActive(false);
        multiplaymenu1.SetActive(false);
        loading.SetActive(true);
        /* async load the next scene */
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ValidateNextMP1(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            joinMenu.SetActive(false);
            return;
        }
        if (input.Contains(" "))
        {
            joinMenu.SetActive(false);
            return;
        }
        joinMenu.SetActive(true);
        DisplayName = input;
    }

    public void OpenMultiplayer1()
    {
        OnEnable();
        mainmenu.SetActive(false);
        multiplaymenu1.SetActive(true);
        loading.SetActive(false);
    }

    private void OnEnable()
    {
        MyNetworkLobby.OnClientConnected += HandleClientConnected;
        MyNetworkLobby.OnClientDisconnected += HandleClientDisconnected;
        MyNetworkLobby.OnServerStopped += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        MyNetworkLobby.OnClientConnected -= HandleClientConnected;
        MyNetworkLobby.OnClientDisconnected -= HandleClientDisconnected;
        MyNetworkLobby.OnServerStopped -= HandleClientDisconnected;
    }

    public void DedicatedServer()
    {
        myNetworkLobby.StartServer();
        mainmenu.SetActive(false);
        multiplaymenu1.SetActive(false);
        loading.SetActive(false);
    }


    public void HostLobby()
    {
        myNetworkLobby.StartHost();
        mainmenu.SetActive(false);
        multiplaymenu1.SetActive(false);
        loading.SetActive(false);
    }

    public void JoinLobby()
    {
        myNetworkLobby.networkAddress = ipInput.text;
        myNetworkLobby.StartClient();
        mainmenu.SetActive(false);
        multiplaymenu1.SetActive(false);
        loading.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void HandleClientConnected()
    {
        OnDisable();
        mainmenu.SetActive(false);
        multiplaymenu1.SetActive(false);
        loading.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        OpenMultiplayer1();
    }
}
