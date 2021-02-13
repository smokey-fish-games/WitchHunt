using UnityEngine.UI;
using UnityEngine;
using Mirror;
using TMPro;

namespace SFG.NetworkSystem

{
    public class LobbyPlayer : MyNetworkBehaviour2
    {
        [SyncVar(hook = nameof(updateTextField))]
        public string myDisName = "Player";
        RobLogger RL;
        public TMP_Text debug = null;

        public Button readyUpButton;
        public Button startButton;
        public GameObject LedaerIcon;
        public TMP_InputField nameEnter;

        [SyncVar]
        public bool isLeader = false;
        [SyncVar]
        public bool IsReady = false;

        private Lobby room;
        private Lobby Room
        {
            get
            {
                if (room != null)
                {
                    return room;
                }
                return room = NetworkManager.singleton as Lobby;
            }
        }

        void updateTextField(string oldValue, string newValue)
        {
            WriteALog("updateTextField");
            myDisName = newValue;
            nameEnter.text = myDisName;
        }

        public void SetName(string newName)
        {
            WriteALog("SetName");
            CmdSyncChar(newName, isLeader);
        }

        public void ClickReadyBut()
        {
            WriteALog("ClickReadyBut");
            CmdChangeReadyFlag();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // No log
            LedaerIcon.SetActive(isLeader);
            ColorBlock befor = readyUpButton.colors;
            if (IsReady)
            {
                befor.normalColor = Color.green;
                befor.disabledColor = Color.green;
                befor.highlightedColor = Color.green;
                befor.pressedColor = Color.green;
                befor.selectedColor = Color.green;
                readyUpButton.GetComponentInChildren<Text>().text = "READY";
            }
            else
            {
                befor.normalColor = Color.red;
                befor.disabledColor = Color.red;
                befor.highlightedColor = Color.red;
                befor.pressedColor = Color.red;
                befor.selectedColor = Color.red;
                readyUpButton.GetComponentInChildren<Text>().text = "NOT READY";
            }
            readyUpButton.colors = befor;
        }

        public override void OnStartAuthority()
        {
            WriteALog("OnStartAuthority");
            CmdSyncChar(myDisName, isLeader);
            readyUpButton.interactable = true;
            nameEnter.interactable = true;
            nameEnter.text = myDisName;
            startButton = Room.startButton;
            if (isLeader)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.AddListener(delegate { TryStart(); });
            }
        }

        public override void OnStartClient()
        {
            WriteALog("OnStartClient");
            Room.otherPlayers.Add(this);
            // reparent
            this.gameObject.transform.parent = room.lobbyPlayerParent.transform;
        }

        public override void OnStopClient()
        {
            WriteALog("OnStopClient");
            Room.otherPlayers.Remove(this);
        }

        void WriteALog(string mes)
        {
            string toWrite = string.Empty;
            toWrite += "LobbyPlayer(" + myDisName + ") " + mes;
            if (RL == null)
            {
                RL = RobLogger.GetRobLogger();
            }
            RL.writeInfo(toWrite);
        }

        [Command]
        private void CmdSyncChar(string name, bool lead)
        {
            WriteALog("CmdSyncChar");
            myDisName = name;
            isLeader = lead;
        }

        [Command]
        public void CmdChangeReadyFlag()
        {
            WriteALog("CmdChangeReadyFlag");
            IsReady = !IsReady;
        }

        public void TryStart()
        {
            WriteALog("TryStart");
            if (!isLeader)
            {
                return;
            }
            CmdStartGame();
        }

        [Command]
        void CmdStartGame()
        {
            WriteALog("CmdStartGame");
            Room.lobbyStartGame();
        }
    }
}
