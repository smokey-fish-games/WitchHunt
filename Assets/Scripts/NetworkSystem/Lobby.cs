using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using System;
using SFG.WitchHunt.MultiPlayer;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.NetworkSystem
{
    public enum CURSCREEN
    {
        MAIN,
        LOBBY
    }

    public class Lobby : SFGNetworkManager
    {
        public TMP_Text ConnectedStats = null;
        public TMP_Text ReadyStats = null;
        public Button startButton = null;
        bool starting = false;

        public CURSCREEN currentScreen = CURSCREEN.MAIN;

        public static event Action OnConnected;
        public static event Action OnDisconnected;
        public static event Action OnGameStarting;

        [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;
        public GameObject lobbyPlayerParent = null;
        public List<LobbyPlayer> otherPlayers = new List<LobbyPlayer>();

        public List<PlayerController> GamePlayers = new List<PlayerController>();
        [SerializeField] private PlayerController gamePlayerPrefab = null;

        private RobLogger rl;
        RobLogger RL
        {
            get
            {
                if (rl != null)
                {
                    return rl;
                }
                return rl = RobLogger.GetRobLogger();
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (starting)
                return;

            // Update topbar
            if (currentScreen == CURSCREEN.LOBBY)
            {
                UpdateTopBar();
            }
        }

        void UpdateTopBar()
        {
            string context = string.Empty;
            string reatext = string.Empty;
            int readied = 0;


            ConnectedStats.text = string.Empty;
            ReadyStats.text = string.Empty;

            if (otherPlayers.Count == maxConnections)
            {
                context += "<color=red>";
            }
            else
            {
                context += "<color=green>";
            }
            context += "Connected: ";
            context += otherPlayers.Count;
            context += "/";
            context += maxConnections;
            context += "</color>";
            ConnectedStats.text = context;

            foreach (LobbyPlayer l in otherPlayers)
            {
                if (l.IsReady)
                {
                    readied++;
                }
            }

            if (readied != otherPlayers.Count || otherPlayers.Count == 0)
            {
                startButton.interactable = false;
                reatext += "<color=red>";
            }
            else
            {
                startButton.interactable = true;
                reatext += "<color=green>";
            }
            reatext += "Ready: ";
            reatext += readied;
            reatext += "/";
            reatext += otherPlayers.Count;
            reatext += "</color>";
            ReadyStats.text = reatext;
        }

        /// Called on the server when a new client connects.
        public override void OnServerConnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }
            // Check we in main menu
            RL.writeTraceExit(null);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            if (conn.identity != null)
            {
                LobbyPlayer toRemove = conn.identity.GetComponent<LobbyPlayer>();
                otherPlayers.Remove(toRemove);
            }

            base.OnServerDisconnect(conn);
            RL.writeTraceExit(null);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            LobbyPlayer roomPlayerInstance = Instantiate(lobbyPlayerPrefab, lobbyPlayerParent.transform);
            roomPlayerInstance.startButton = startButton;

            if (otherPlayers.Count == 0)
            {
                roomPlayerInstance.isLeader = true;
            }
            if (this.mode == NetworkManagerMode.ServerOnly)
            {
                // Make sure otherplayers is kept up to date
                otherPlayers.Add(roomPlayerInstance);
            }

            RL.writeInfo(RobLogger.LogLevel.STANDARD, "Adding Player(" + otherPlayers.Count + ") " + (otherPlayers.Count == 0 ? "" : "NOT ") + " Leader=" + conn);
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            RL.writeTraceExit(null);
        }

        public override void OnStopServer()
        {
            RL.writeTraceEntry();
            base.OnStopServer();
            OnDisconnected?.Invoke();
            RL.writeTraceExit(null);
        }

        public override void OnStartServer()
        {
            RL.writeTraceEntry();
            base.OnStartServer();

            if (this.mode == NetworkManagerMode.ServerOnly)
            {
                OnConnected?.Invoke();
            }
            RL.writeTraceExit(null);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnClientConnect(conn);

            OnConnected?.Invoke();
            RL.writeTraceExit(null);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnClientDisconnect(conn);

            OnDisconnected?.Invoke();
            RL.writeTraceExit(null);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            RL.writeTraceEntry(conn, errorCode);
            base.OnClientError(conn, errorCode);

            OnDisconnected?.Invoke();
            RL.writeTraceExit(null);
        }

        public void DisconnectClicked()
        {
            RL.writeTraceEntry();
            switch (this.mode)
            {
                case NetworkManagerMode.Host:
                    this.StopHost();
                    break;
                case NetworkManagerMode.ServerOnly:
                    this.StopServer();
                    break;
                case NetworkManagerMode.ClientOnly:
                    this.StopClient();
                    break;
                case NetworkManagerMode.Offline:
                    break;
                default:
                    RL.writeError("Unknown mode: " + this.mode);
                    break;
            }
            RL.writeTraceExit(null);
        }

        public void lobbyStartGame()
        {
            RL.writeTraceEntry();

            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                if (!IsReadyToStart())
                {
                    RL.writeTraceExit(null);
                    return;
                }
                RL.writeInfo(RobLogger.LogLevel.STANDARD, "Server Starting game");
                starting = true;
                ServerChangeScene("TestBed");
            }
            RL.writeTraceExit(null);
        }

        private bool IsReadyToStart()
        {
            RL.writeTraceEntry();
            if (numPlayers == 0)
            {
                RL.writeTraceExit(false);
                return false;
            }
            foreach (LobbyPlayer p in otherPlayers)
            {
                if (!p.IsReady)
                {
                    return false;
                }
            }
            RL.writeTraceExit(true);
            return true;
        }

        public override void ServerChangeScene(string newSceneName)
        {
            RL.writeTraceEntry(newSceneName);
            // From menu to game
            if (SceneManager.GetActiveScene().name == "MainMenu" && newSceneName == "TestBed")
            {
                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Going from mainmeny to game");
                for (int i = otherPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = otherPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                    gameplayerInstance.SetDisplayName(otherPlayers[i].myDisName);

                    NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
                }
            }

            base.ServerChangeScene(newSceneName);
            RL.writeTraceExit(null);
        }

        public override void OnServerChangeScene(string newSceneName)
        {
            RL.writeTraceEntry(newSceneName);
            if (SceneManager.GetActiveScene().name == "MainMenu" && newSceneName == "TestBed")
            {
                OnGameStarting?.Invoke();
            }
            base.OnServerChangeScene(newSceneName);
            RL.writeTraceExit(null);
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            RL.writeTraceEntry(newSceneName, sceneOperation, customHandling);
            if (SceneManager.GetActiveScene().name == "MainMenu" && newSceneName == "TestBed")
            {
                OnGameStarting?.Invoke();
            }
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
            RL.writeTraceExit(null);
        }
    }
}