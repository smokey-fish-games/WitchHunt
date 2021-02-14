using UnityEngine;
using System.Collections.Generic;

namespace SFG.WitchHunt.SinglePlayer
{
    public class Building : MonoBehaviour
    {
        List<NPCController> occupants;
        SpawnMarker[] NPCSpawners;

        public void initialize(List<NPCController> occ)
        {
            occupants = occ;
            NPCSpawners = GetComponentsInChildren<SpawnMarker>();
        }

        public void moveOccupantsToSpawns()
        {
            for (int i = 0; i < occupants.Count; i++)
            {
                NPCController thisone = occupants[i];
                if (i < NPCSpawners.Length)
                {
                    thisone.transform.position = NPCSpawners[i].transform.position;
                }
                else
                {
                    Debug.LogError("building " + this.name + " has too few NPC Spawners");
                    thisone.transform.position = NPCSpawners[i % NPCSpawners.Length].transform.position;
                }
            }
        }
    }
}