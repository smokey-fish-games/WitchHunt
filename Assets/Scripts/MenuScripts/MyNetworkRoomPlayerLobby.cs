using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class MyNetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    public GameObject lobbyUI = null;
    public TMP_Text[] playerNameTexts = new TMP_Text[4];
    public TMP_Text[] playerReadyTexts = new TMP_Text[4];
    public GameObject leaderAlert = null;
    public Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayeNameChanged))]
    public string DisplayName = "Loading ...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    [SyncVar(hook = nameof(IsLeaderChanged))]
    public bool IsLeader;

    public void IsLeaderChanged(bool oldValue, bool newValue)
    {
        IsLeader = newValue;
        startGameButton.gameObject.SetActive(newValue);
        leaderAlert.SetActive(newValue);
    }

    private MyNetworkLobby room;
    private MyNetworkLobby Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as MyNetworkLobby;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(MainMenuController.DisplayName);
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.otherPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.otherPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleDisplayeNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!isLocalPlayer)
        {
            foreach (MyNetworkRoomPlayerLobby p in Room.otherPlayers)
            {
                if (p.hasAuthority)
                {
                    p.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.otherPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.otherPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.otherPlayers[i].IsReady ? "<color=green>READY</color>" : "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!IsLeader)
        {
            return;
        }
        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string name)
    {
        DisplayName = name;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.otherPlayers[0].connectionToClient != connectionToClient && !this.isServerOnly)
        {
            return;
        }

        // Start the game!
        Debug.Log("Starting!");
        Room.StartGame();
    }
}

