using UnityEngine;
using System.Collections.Generic;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.SinglePlayer
{
    public class Building : MonoBehaviour
    {
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

        List<NPCController> occupants;
        SpawnMarker[] NPCSpawners;

        public void initialize(List<NPCController> occ)
        {
            RL.writeTraceEntry(occ);
            occupants = occ;
            NPCSpawners = GetComponentsInChildren<SpawnMarker>();
            this.gameObject.name = (occ.Count == 0 ? "Unoccupied" : occ[0].lastname + " House");
            RL.writeTraceExit(null);
        }

        public void moveOccupantsToSpawns()
        {
            RL.writeTraceEntry();
            for (int i = 0; i < occupants.Count; i++)
            {
                NPCController thisone = occupants[i];
                if (i < NPCSpawners.Length)
                {
                    thisone.transform.position = NPCSpawners[i].transform.position;
                }
                else
                {
                    RL.writeError("building " + this.name + " has too few NPC Spawners");
                    thisone.transform.position = NPCSpawners[i % NPCSpawners.Length].transform.position;
                }
            }
            RL.writeTraceExit(null);
        }
    }
}