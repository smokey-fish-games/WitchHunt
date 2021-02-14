using SFG.WitchHunt.NetworkSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SFG.WitchHunt
{
    public class MainMenuUIControl : MonoBehaviour
    {
        public Lobby networkManager2;
        public GameObject controlPanel;
        public GameObject playerPanel;
        public GameObject loading;
        public TMP_InputField IPInput;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            playerPanel.SetActive(false);
            loading.SetActive(false);
            controlPanel.SetActive(true);
            Lobby.OnConnected += HandleClientConnected;
            Lobby.OnDisconnected += HandleClientDisconnected;
            Lobby.OnGameStarting += HandleOnGameStarting;
        }

        public void StartServer()
        {
            playerPanel.SetActive(false);
            controlPanel.SetActive(false);
            loading.SetActive(true);
            networkManager2.currentScreen = CURSCREEN.LOBBY;
            networkManager2.StartServer();
        }

        public void StartHost()
        {
            playerPanel.SetActive(false);
            controlPanel.SetActive(false);
            loading.SetActive(true);
            networkManager2.currentScreen = CURSCREEN.LOBBY;
            networkManager2.StartHost();
        }

        public void StartClient()
        {
            playerPanel.SetActive(false);
            controlPanel.SetActive(false);
            loading.SetActive(true);
            networkManager2.networkAddress = IPInput.text;
            networkManager2.currentScreen = CURSCREEN.LOBBY;
            networkManager2.StartClient();
        }

        public void StartSinglePlayer()
        {
            loading.SetActive(true);
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

        public void Quit()
        {
            Application.Quit();
        }

        private void HandleClientConnected()
        {
            playerPanel.SetActive(true);
            loading.SetActive(false);
            controlPanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            playerPanel.SetActive(false);
            loading.SetActive(false);
            controlPanel.SetActive(true);
        }

        private void HandleOnGameStarting()
        {
            Lobby.OnConnected -= HandleClientConnected;
            Lobby.OnDisconnected -= HandleClientDisconnected;
            Lobby.OnGameStarting -= HandleOnGameStarting;
        }
    }
}
