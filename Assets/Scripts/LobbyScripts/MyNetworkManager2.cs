using UnityEngine.UI;
using UnityEngine;
using Mirror;
using TMPro;

namespace SFG.NetworkSystem
{
    public class MyNetworkManager2 : NetworkManager
    {
        RobLogger RL;
        void WriteALog(string mes)
        {
            string toWrite = string.Empty;
            toWrite += "NetMan(" + this.mode + ") " + mes;
            if (RL == null)
            {
                RL = RobLogger.GetRobLogger();
            }
            RL.writeInfo(toWrite);
        }


        #region Unity Callbacks
        /// virtual so that inheriting classes' OnValidate() can call base.OnValidate() too
        public override void OnValidate()
        {
            WriteALog("OnValidate called");
            base.OnValidate();
        }

        /// virtual so that inheriting classes' Awake() can call base.Awake() too
        public override void Awake()
        {
            WriteALog("Awake called");
            base.Awake();
        }

        /// virtual so that inheriting classes' Start() can call base.Start() too
        public override void Start()
        {
            WriteALog("Start called");
            base.Start();
        }

        /// virtual so that inheriting classes' LateUpdate() can call base.LateUpdate() too
        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        #endregion

        #region Start & Stop

        /// called when quitting the application by closing the window / pressing stop in the editor
        public override void OnApplicationQuit()
        {
            WriteALog("OnApplicationQuit called");
            base.OnApplicationQuit();
        }

        /// Set the frame rate for a headless server.
        public override void ConfigureServerFrameRate()
        {
            WriteALog("ConfigureServerFrameRate called");
            base.ConfigureServerFrameRate();
        }

        /// virtual so that inheriting classes' OnDestroy() can call base.OnDestroy() too
        public override void OnDestroy()
        {
            WriteALog("OnDestroy called");
            base.OnDestroy();
        }

        #endregion

        #region Scene Management

        /// This causes the server to switch scenes and sets the networkSceneName.
        public override void ServerChangeScene(string newSceneName)
        {
            WriteALog("ServerChangeScene called: " + newSceneName);
            base.ServerChangeScene(newSceneName);
        }

        #endregion

        #region Server System Callbacks

        /// Called on the server when a new client connects.
        public override void OnServerConnect(NetworkConnection conn)
        {
            WriteALog("OnServerConnect called: " + conn);
            base.OnServerConnect(conn);
        }

        /// Called on the server when a client disconnects.
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            WriteALog("OnServerDisconnect called: " + conn);
            base.OnServerDisconnect(conn);
        }

        /// Called on the server when a client is ready.
        public override void OnServerReady(NetworkConnection conn)
        {
            WriteALog("OnServerReady called: " + conn);
            base.OnServerReady(conn);
        }

        /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            WriteALog("OnServerAddPlayer called: " + conn);
            base.OnServerAddPlayer(conn);
        }

        /// Called on the server when a network error occurs for a client connection.
        public override void OnServerError(NetworkConnection conn, int errorCode)
        {
            WriteALog("OnServerError called: " + conn + " | " + errorCode);
            base.OnServerError(conn, errorCode);
        }

        /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
        public override void OnServerChangeScene(string newSceneName)
        {
            WriteALog("OnServerReady called: " + newSceneName);
            base.OnServerChangeScene(newSceneName);
        }

        /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
        public override void OnServerSceneChanged(string sceneName)
        {
            WriteALog("OnServerSceneChanged called: " + sceneName);
            base.OnServerSceneChanged(sceneName);
        }

        #endregion
        #region Client System Callbacks

        /// Called on the client when connected to a server.
        public override void OnClientConnect(NetworkConnection conn)
        {
            WriteALog("OnClientConnect called: " + conn);
            base.OnClientConnect(conn);
        }

        /// Called on clients when disconnected from a server.
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            WriteALog("OnClientDisconnect called: " + conn);
            base.OnClientDisconnect(conn);
        }

        /// Called on clients when a network error occurs.
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            WriteALog("OnClientError called: " + conn + " | " + errorCode);
            base.OnClientError(conn, errorCode);
        }

        /// Called on clients when a servers tells the client it is no longer ready.
        public override void OnClientNotReady(NetworkConnection conn)
        {
            WriteALog("OnClientNotReady called: " + conn);
            base.OnClientNotReady(conn);
        }

        /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            WriteALog("OnClientChangeScene called: " + newSceneName + " | " + sceneOperation + " | " + customHandling);
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        }

        /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            WriteALog("OnClientSceneChanged called: " + conn);
            base.OnClientSceneChanged(conn);
        }

        #endregion
        #region Start & Stop callbacks

        /// This is invoked when a host is started.
        public override void OnStartHost()
        {
            WriteALog("OnStartHost called");
            base.OnStartHost();
        }

        /// This is invoked when a server is started - including when a host is started.
        public override void OnStartServer()
        {
            WriteALog("OnStartServer called");
            base.OnStartServer();
        }

        /// This is invoked when the client is started.
        public override void OnStartClient()
        {
            WriteALog("OnStartClient called");
            base.OnStartClient();
        }

        /// This is called when a server is stopped - including when a host is stopped.
        public override void OnStopServer()
        {
            WriteALog("OnStopServer called");
            base.OnStopServer();
        }

        /// This is called when a client is stopped.
        public override void OnStopClient()
        {
            WriteALog("OnStopClient called");
            base.OnStopClient();
        }

        /// This is called when a host is stopped.
        public override void OnStopHost()
        {
            WriteALog("OnStopHost called");
            base.OnStopHost();
        }

        #endregion
    }
}
