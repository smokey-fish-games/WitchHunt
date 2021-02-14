using UnityEngine;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.SinglePlayer
{
    public class GameController : MonoBehaviour
    {
        #region Variables
        /* publics */
        public GameObject boxPrefab;
        public LevelGenerator lgen;

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
            // Place box
            SpawnBoxes(FindObjectsOfType<Building>().Length);
            lgen.GenerateNPCS();
            RL.writeTraceExit(null);
        }

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
                GameObject box = (GameObject)Instantiate(boxPrefab, allBoxSpawns[i].transform.position, Quaternion.identity, allBoxSpawns[i].transform.parent.transform);

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
            }
            RL.writeTraceExit(null);
        }
        #endregion
        #region DebugFunctions

        #endregion
    }
}