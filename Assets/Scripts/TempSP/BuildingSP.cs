using UnityEngine;
using System.Collections.Generic;

public class BuildingSP : MonoBehaviour
{
    List<NPCControllerSP> occupants;
    SpawnMarker[] NPCSpawners;

    public void initialize(List<NPCControllerSP> occ)
    {
        occupants = occ;
        NPCSpawners = GetComponentsInChildren<SpawnMarker>();
    }

    public void moveOccupantsToSpawns()
    {
        for (int i = 0; i < occupants.Count; i++)
        {
            NPCControllerSP thisone = occupants[i];
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
