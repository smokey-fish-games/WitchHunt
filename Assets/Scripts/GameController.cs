using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class GameController : NetworkBehaviour
{
    #region Variables
    /* publics */
    public GameObject boxPrefab;

    /* privates */
    RobLogger RL;
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
        if (isServer)
        {
            RL.writeInfo("GameController Server hello!!!");
        }
        if (isClient)
        {
            RL.writeInfo("GameController Client hello!!!");
        }
        if (isLocalPlayer)
        {
            RL.writeInfo("GameController LocalPlayer hello!!!");
        }

        /* Server setup */
        if (isServer)
        {
            // Place box
            SpawnBoxes(1);
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        RL = RobLogger.GetRobLogger();
    }

    #endregion

    /* Code for Server only runs here i.e. Commands and relevants */
    #region Server

    void SpawnBoxes(int number)
    {
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
    }

    #endregion
    /* Code for Client only runs here i.e. ClientRPC and relevants */
    #region Client

    #endregion
    #region DebugFunctions

    #endregion
}
