using UnityEngine;
using System.Collections.Generic;

public class GameControllerSP : MonoBehaviour
{
    #region Variables
    /* publics */
    public GameObject boxPrefab;
    public LevelGeneratorSP lgen;

    /* privates */
    RobLogger RL;
    GameObject[] ListOfBoxSpawns;
    #endregion

    /* Generic Code starts here i.e. Both Server/Client */
    #region Generic

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // Place box
        SpawnBoxes(FindObjectsOfType<BuildingSP>().Length);
        lgen.GenerateNPCS();
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        RL = RobLogger.GetRobLogger();
    }

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
            GameObject box = (GameObject)Instantiate(boxPrefab, allBoxSpawns[i].transform.position, Quaternion.identity, allBoxSpawns[i].transform.parent.transform);

            InventorySP inv = box.GetComponent<InventorySP>();
            if (oneSpawned)
            {
                inv.initialize(CONSTANTS.NO_ITEM);
            }
            else
            {
                oneSpawned = true;
                inv.initialize(CONSTANTS.ITEM_SPELLBOOK);
            }
        }
    }
    #endregion
    #region DebugFunctions

    #endregion
}
