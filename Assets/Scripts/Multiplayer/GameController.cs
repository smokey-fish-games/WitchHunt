using Mirror;
using UnityEngine;
using SFG.WitchHunt.NetworkSystem;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.MultiPlayer
{
    public class GameController : SFGNetworkBehaviour
    {
        #region Variables
        /* publics */
        public GameObject boxPrefab;
        public GameObject npcPrefab;

        /* privates */
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

        GameObject[] ListOfBoxSpawns;


        /* Sync variables that are kept in sync on Client/Server */
        #region SyncVariables



        #endregion
        #endregion

        /* All methods called by the above SyncVariables */
        #region SyncVarMethods


        #endregion
        /* Generic Code starts here i.e. Both Server/Client */
        #region Generic

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            RL.writeTraceEntry();
            RL.writeInfo(RobLogger.LogLevel.VERBOSE, "(name=" + gameObject.name + ",netId=" + this.netId + ",isServer=" + isServer + ",isServerOnly=" + isServerOnly + ",isClient=" + isClient + ",isClientOnly=" + isClientOnly + ",isLocalPlayer=" + isLocalPlayer + ")");

            /* Server setup */
            if (isServer || isServerOnly)
            {
                // Place box
                SpawnBoxes(10);
                SpawnNPCS(10);
            }
            RL.writeTraceExit(null);
        }

        #endregion

        /* Code for Server only runs here i.e. Commands and relevants */
        #region Server

        void SpawnBoxes(int number)
        {
            RL.writeTraceEntry(number);
            bool oneSpawned = false;
            if (number < 1)
            {
                RL.writeError("Bad number of boxes asked to be spawned " + number);
                return;
            }

            SpawnMarker[] allBoxSpawns = SpawnMarker.GetAll(SpawnMarker.SpawnType.BOX);

            if (number > allBoxSpawns.Length)
            {
                RL.writeError("Asked to spawn more boxes than markers. Will only spawn " + allBoxSpawns.Length);
                number = allBoxSpawns.Length;
            }

            CONSTANTS.Shuffle(allBoxSpawns);

            for (int i = 0; i < number; i++)
            {
                GameObject box = (GameObject)Instantiate(boxPrefab, allBoxSpawns[i].transform.position, Quaternion.identity);

                Inventory inv = box.GetComponent<Inventory>();
                if (oneSpawned)
                {
                    inv.initialize(CONSTANTS.NO_ITEM);
                }
                else
                {
                    oneSpawned = true;
                    inv.initialize(CONSTANTS.ITEM_SPELLBOOK);
                }
                NetworkServer.Spawn(box);
            }
            RL.writeTraceExit(null);
        }

        void SpawnNPCS(int number)
        {
            RL.writeTraceEntry(number);
            if (number < 1)
            {
                RL.writeError("Bad number of NPCS asked to be spawned " + number);
                return;
            }

            SpawnMarker[] allNPCSpawns = SpawnMarker.GetAll(SpawnMarker.SpawnType.NPC);

            if (number > allNPCSpawns.Length)
            {
                RL.writeError("Asked to spawn more boxes than markers. Will only spawn " + allNPCSpawns.Length);
                number = allNPCSpawns.Length;
            }

            CONSTANTS.Shuffle(allNPCSpawns);

            for (int i = 0; i < number; i++)
            {
                GameObject npc = (GameObject)Instantiate(npcPrefab, allNPCSpawns[i].transform.position, Quaternion.identity);
                npc.name = "NPC" + i;
                npc.GetComponent<NPCController>().NPCname = "NPC" + i;
                NetworkServer.Spawn(npc);
            }
            RL.writeTraceExit(null);
        }

        #endregion
        /* Code for Client only runs here i.e. ClientRPC and relevants */
        #region Client

        #endregion
        #region DebugFunctions

        #endregion
    }
}