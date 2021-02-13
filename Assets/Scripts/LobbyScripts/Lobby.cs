using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using System;

namespace SFG.NetworkSystem
{
    public enum CURSCREEN
    {
        MAIN,
        LOBBY
    }

    public class Lobby : MyNetworkManager2
    {
        public TMP_Text ConnectedStats = null;
        public TMP_Text ReadyStats = null;
        public Button startButton = null;
        bool starting = false;

        public CURSCREEN currentScreen = CURSCREEN.MAIN;

        public static event Action OnConnected;
        public static event Action OnDisconnected;

        [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;
        public GameObject lobbyPlayerParent = null;
        public List<LobbyPlayer> otherPlayers = new List<LobbyPlayer>();

        public List<PlayerController> GamePlayers = new List<PlayerController>();
        [SerializeField] private PlayerController gamePlayerPrefab = null;

        RobLogger RL;
        public TMP_Text debug;


        void WriteALog(string mes)
        {
            string toWrite = string.Empty;
            toWrite += "Lobby(" + this.mode + ") " + mes;
            if (RL == null)
            {
                RL = RobLogger.GetRobLogger();
            }
            RL.writeInfo(toWrite);
        }


        // public enum NetworkManagerMode { Offline, ServerOnly, ClientOnly, Host }
        NetworkManagerMode thisMode;
        string netAddress;
        int maxConns;
        int servtickrate;
        int numofPlayers;
        bool iAmConned;
        string curScene;
        int startPos;

        void PrepareDebug()
        {
            thisMode = this.mode;
            netAddress = this.networkAddress;
            maxConns = this.maxConnections;
            servtickrate = this.serverTickRate;
            numofPlayers = this.numPlayers;
            iAmConned = this.isNetworkActive;
            curScene = Lobby.networkSceneName;
            startPos = Lobby.startPositionIndex;
        }

        void setDebug()
        {
            string toWrite = string.Empty;
            debug.text = string.Empty;

            toWrite += "            Mode: <b>" + thisMode + "</b>\n";
            toWrite += " Network Address: <b>" + netAddress + "</b>\n";
            toWrite += " Max Connections: <b>" + maxConns + "</b>\n";
            toWrite += "Server Tick Rate: <b>" + servtickrate + "</b>\n";
            toWrite += "         Players: <b>" + numofPlayers + "</b>\n";
            toWrite += "  Active Network: <b>" + (iAmConned ? "<color=green>TRUE</color>" : "<color=red>FALSE</color>") + "</b>\n";
            toWrite += "   Network Scene: <b>" + curScene + "</b>\n";
            toWrite += " Start Posistion: <b>" + startPos + "</b>\n";
            toWrite += "    otherPlayers: <b>" + otherPlayers.Count + "</b>\n";

            debug.text = toWrite;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (starting)
                return;

            PrepareDebug();
            setDebug();

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
            WriteALog("OnServerConnect - Connection from: " + conn);
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }
            // Check we in main menu
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            WriteALog("OnServerDisconnect - Connection: " + conn);
            if (conn.identity != null)
            {
                LobbyPlayer toRemove = conn.identity.GetComponent<LobbyPlayer>();
                otherPlayers.Remove(toRemove);
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            WriteALog("OnServerAddPlayer: " + conn);
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

            WriteALog("Player(" + otherPlayers.Count + ") " + (otherPlayers.Count == 0 ? "" : "NOT ") + " Leader=" + conn);
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }

        public override void OnStopServer()
        {
            WriteALog("OnStopServer");
            base.OnStopServer();
            OnDisconnected?.Invoke();
        }

        public override void OnStartServer()
        {
            WriteALog("OnStartServer");
            base.OnStartServer();

            if (this.mode == NetworkManagerMode.ServerOnly)
            {
                OnConnected?.Invoke();
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            WriteALog("OnClientConnect - Connection from: " + conn);
            base.OnClientConnect(conn);

            OnConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            WriteALog("OnClientDisconnect - Connection: " + conn);
            base.OnClientDisconnect(conn);

            OnDisconnected?.Invoke();
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            WriteALog("OnClientError - Connection: " + conn + " | " + errorCode);
            base.OnClientError(conn, errorCode);

            OnDisconnected?.Invoke();
        }

        public void DisconnectClicked()
        {
            WriteALog("DisconnectClicked");
            switch (this.mode)
            {
                case NetworkManagerMode.Host:
                    WriteALog("DisconnectClicked - Stopping Host");
                    this.StopHost();
                    break;
                case NetworkManagerMode.ServerOnly:
                    WriteALog("DisconnectClicked - Stopping Server");
                    this.StopServer();
                    break;
                case NetworkManagerMode.ClientOnly:
                    WriteALog("DisconnectClicked - Stopping Client");
                    this.StopClient();
                    break;
                case NetworkManagerMode.Offline:
                    WriteALog("DisconnectClicked - Already Offline");
                    break;
                default:
                    WriteALog("DisconnectClicked - Unknown mode: " + this.mode);
                    break;
            }
        }

        public void lobbyStartGame()
        {
            WriteALog("lobbyStartGame");
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                if (!IsReadyToStart())
                {
                    return;
                }
                starting = true;

                ServerChangeScene("TestBed");
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers == 0)
            {
                return false;
            }
            foreach (LobbyPlayer p in otherPlayers)
            {
                if (!p.IsReady)
                {
                    return false;
                }
            }
            return true;
        }

        public override void ServerChangeScene(string newSceneName)
        {
            WriteALog("ServerChangeScene: " + newSceneName);
            // From menu to game
            if (SceneManager.GetActiveScene().name == "MainMenu" && newSceneName == "TestBed")
            {
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
        }
    }
}