using Mirror;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.NetworkSystem
{
    public class SFGNetworkManager : NetworkManager
    {
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

        #region Unity Callbacks
        /// virtual so that inheriting classes' OnValidate() can call base.OnValidate() too
        public override void OnValidate()
        {
            RL.writeTraceEntry();
            base.OnValidate();
            RL.writeTraceExit(null);
        }

        /// virtual so that inheriting classes' Awake() can call base.Awake() too
        public override void Awake()
        {
            RL.writeTraceEntry();
            base.Awake();
            RL.writeTraceExit(null);
        }

        /// virtual so that inheriting classes' Start() can call base.Start() too
        public override void Start()
        {
            RL.writeTraceEntry();
            base.Start();
            RL.writeTraceExit(null);
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
            RL.writeTraceEntry();
            base.OnApplicationQuit();
            RL.writeTraceExit(null);
        }

        /// Set the frame rate for a headless server.
        public override void ConfigureServerFrameRate()
        {
            RL.writeTraceEntry();
            base.ConfigureServerFrameRate();
            RL.writeTraceExit(null);
        }

        /// virtual so that inheriting classes' OnDestroy() can call base.OnDestroy() too
        public override void OnDestroy()
        {
            RL.writeTraceEntry();
            base.OnDestroy();
            RL.writeTraceExit(null);
        }

        #endregion

        #region Scene Management

        /// This causes the server to switch scenes and sets the networkSceneName.
        public override void ServerChangeScene(string newSceneName)
        {
            RL.writeTraceEntry(newSceneName);
            base.ServerChangeScene(newSceneName);
            RL.writeTraceExit(null);
        }

        #endregion

        #region Server System Callbacks

        /// Called on the server when a new client connects.
        public override void OnServerConnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnServerConnect(conn);
            RL.writeTraceExit(null);
        }

        /// Called on the server when a client disconnects.
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnServerDisconnect(conn);
            RL.writeTraceExit(null);
        }

        /// Called on the server when a client is ready.
        public override void OnServerReady(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnServerReady(conn);
            RL.writeTraceExit(null);
        }

        /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnServerAddPlayer(conn);
            RL.writeTraceExit(null);
        }

        /// Called on the server when a network error occurs for a client connection.
        public override void OnServerError(NetworkConnection conn, int errorCode)
        {
            RL.writeTraceEntry(conn, errorCode);
            base.OnServerError(conn, errorCode);
            RL.writeTraceExit(null);
        }

        /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
        public override void OnServerChangeScene(string newSceneName)
        {
            RL.writeTraceEntry(newSceneName);
            base.OnServerChangeScene(newSceneName);
            RL.writeTraceExit(null);
        }

        /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
        public override void OnServerSceneChanged(string sceneName)
        {
            RL.writeTraceEntry(sceneName);
            base.OnServerSceneChanged(sceneName);
            RL.writeTraceExit(null);
        }

        #endregion
        #region Client System Callbacks

        /// Called on the client when connected to a server.
        public override void OnClientConnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnClientConnect(conn);
            RL.writeTraceExit(null);
        }

        /// Called on clients when disconnected from a server.
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnClientDisconnect(conn);
            RL.writeTraceExit(null);
        }

        /// Called on clients when a network error occurs.
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            RL.writeTraceEntry(conn, errorCode);
            base.OnClientError(conn, errorCode);
            RL.writeTraceExit(null);
        }

        /// Called on clients when a servers tells the client it is no longer ready.
        public override void OnClientNotReady(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnClientNotReady(conn);
            RL.writeTraceExit(null);
        }

        /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            RL.writeTraceEntry(newSceneName, sceneOperation, customHandling);
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
            RL.writeTraceExit(null);
        }

        /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            RL.writeTraceEntry(conn);
            base.OnClientSceneChanged(conn);
            RL.writeTraceExit(null);
        }

        #endregion
        #region Start & Stop callbacks

        /// This is invoked when a host is started.
        public override void OnStartHost()
        {
            RL.writeTraceEntry();
            base.OnStartHost();
            RL.writeTraceExit(null);
        }

        /// This is invoked when a server is started - including when a host is started.
        public override void OnStartServer()
        {
            RL.writeTraceEntry();
            base.OnStartServer();
            RL.writeTraceExit(null);
        }

        /// This is invoked when the client is started.
        public override void OnStartClient()
        {
            RL.writeTraceEntry();
            base.OnStartClient();
            RL.writeTraceExit(null);
        }

        /// This is called when a server is stopped - including when a host is stopped.
        public override void OnStopServer()
        {
            RL.writeTraceEntry();
            base.OnStopServer();
            RL.writeTraceExit(null);
        }

        /// This is called when a client is stopped.
        public override void OnStopClient()
        {
            RL.writeTraceEntry();
            base.OnStopClient();
            RL.writeTraceExit(null);
        }

        /// This is called when a host is stopped.
        public override void OnStopHost()
        {
            RL.writeTraceEntry();
            base.OnStopHost();
            RL.writeTraceExit(null);
        }

        #endregion
    }
}
