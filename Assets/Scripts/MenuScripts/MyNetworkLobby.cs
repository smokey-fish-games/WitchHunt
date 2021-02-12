using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyNetworkLobby : NetworkManager
{
    //[Scene] [SerializeField] private string menuScene = string.Empty;
    [SerializeField] private int minPlayers = 1;


    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    [Header("Room")]
    public List<MyNetworkRoomPlayerLobby> otherPlayers = new List<MyNetworkRoomPlayerLobby>();
    [SerializeField] private MyNetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    public List<PlayerController> GamePlayers = new List<PlayerController>();
    [SerializeField] private PlayerController gamePlayerPrefab = null;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect - Connection from: " + conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect - Connection from: " + conn);
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log("Adding player!" + conn);
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log("otherPlayers.Count=" + otherPlayers.Count);
            bool isLeader = otherPlayers.Count == 0;

            MyNetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            MyNetworkRoomPlayerLobby toRemove = conn.identity.GetComponent<MyNetworkRoomPlayerLobby>();

            otherPlayers.Remove(toRemove);
            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        otherPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (MyNetworkRoomPlayerLobby p in otherPlayers)
        {
            p.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers)
        {
            return false;
        }
        foreach (MyNetworkRoomPlayerLobby p in otherPlayers)
        {
            if (!p.IsReady)
            {
                return false;
            }
        }
        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (!IsReadyToStart())
            {
                return;
            }

            ServerChangeScene("TestBed");

        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().name == "MainMenu" && newSceneName == "TestBed")
        {
            for (int i = otherPlayers.Count - 1; i >= 0; i--)
            {
                var conn = otherPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(otherPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}

