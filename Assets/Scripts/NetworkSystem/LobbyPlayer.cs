using UnityEngine.UI;
using UnityEngine;
using Mirror;
using TMPro;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.NetworkSystem
{
    public class LobbyPlayer : SFGNetworkBehaviour
    {
        [SyncVar(hook = nameof(updateTextField))]
        public string myDisName = "Player";
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
            RL.writeTraceEntry(oldValue, newValue);
            myDisName = newValue;
            nameEnter.text = myDisName;
            RL.writeTraceExit(null);
        }

        public void SetName(string newName)
        {
            RL.writeTraceEntry(newName);
            CmdSyncChar(newName, isLeader);
            RL.writeTraceExit(null);
        }

        public void ClickReadyBut()
        {
            RL.writeTraceEntry();
            CmdChangeReadyFlag();
            RL.writeTraceExit(null);
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
            RL.writeTraceEntry();

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
            RL.writeTraceExit(null);
        }

        public override void OnStartClient()
        {
            RL.writeTraceEntry();
            Room.otherPlayers.Add(this);
            // reparent
            this.gameObject.transform.SetParent(room.lobbyPlayerParent.transform, false);
            RL.writeTraceExit(null);
        }

        public override void OnStopClient()
        {
            RL.writeTraceEntry();
            Room.otherPlayers.Remove(this);
            RL.writeTraceExit(null);
        }

        [Command]
        private void CmdSyncChar(string name, bool lead)
        {
            RL.writeTraceEntry(name, lead);
            myDisName = name;
            isLeader = lead;
            RL.writeTraceExit(null);
        }

        [Command]
        public void CmdChangeReadyFlag()
        {
            RL.writeTraceEntry();
            IsReady = !IsReady;
            RL.writeTraceExit(null);
        }

        public void TryStart()
        {
            RL.writeTraceEntry();
            if (!isLeader)
            {
                RL.writeWarning("Someone other than the leader tried to start the game!");
                return;
            }
            CmdStartGame();
            RL.writeTraceExit(null);
        }

        [Command]
        void CmdStartGame()
        {
            RL.writeTraceEntry();
            Room.lobbyStartGame();
            RL.writeTraceExit(null);
        }
    }
}
