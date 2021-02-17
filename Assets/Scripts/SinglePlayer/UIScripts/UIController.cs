using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFG.WitchHunt.SinglePlayer.UI
{
    public class UIController : MonoBehaviour
    {
        InventoryUI inventoryUI;
        DialogUI dialogUI;

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

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            RL.writeTraceEntry();
            inventoryUI = GetComponentInChildren<InventoryUI>();
            if (inventoryUI == null)
            {
                RL.writeError("unable to find inventoryUI!");
            }
            dialogUI = GetComponentInChildren<DialogUI>();
            if (dialogUI == null)
            {
                RL.writeError("unable to find dialogUI!");
            }
            RL.writeTraceExit(null);
        }

        public void OpenUI(IInteractable interactable, PlayerController callback)
        {
            RL.writeTraceEntry(interactable, callback);
            inventoryUI.Close();
            dialogUI.Close();

            if (interactable is Inventory)
            {
                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Opening inventory UI");
                inventoryUI.STARTINVENTORY(interactable as Inventory, callback);
            }
            else if (interactable is NPCController)
            {
                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Opening Dialog UI");
                dialogUI.STARTDIALOG(interactable as NPCController, callback);
            }
            else
            {
                RL.writeError("Unable to open UI for interactable object: " + interactable);
            }
            RL.writeTraceExit(null);
        }
    }
}
