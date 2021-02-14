using UnityEngine;
using System.Linq;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.SinglePlayer
{
    public class Inventory : IInteractable
    {
        #region Variables
        /* publics */

        /* privates */
        public Item item;
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

        Color color;
        Renderer meshRenderer;

        Color[] originalColours;

        /* Sync variables that are kept in sync on Client/Server */
        #region SyncVariables
        bool empty = false;

        int itemID;

        #endregion
        #endregion
        /* All methods called by the above SyncVariables */
        #region SyncVarMethods

        // void SetItem(int oldValue, int newValue)
        // {
        //     RL.writeTraceEntry();
        //     if (newValue == CONSTANTS.NO_ITEM)
        //     {
        //         item = null;
        //     }
        //     else
        //     {
        //         item = Item.GetItem(newValue);
        //     }
        // }
        #endregion
        /* Generic Code starts here i.e. Both Server/Client */
        #region Generic

        public Item getItem()
        {
            return item;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            RL.writeTraceEntry();
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            originalColours = meshRenderer.materials.Select(x => x.color).ToArray();
            RL.writeTraceExit(null);
        }

        public override void InRangeToBeTouched()
        {
            RL.writeTraceEntry();
            foreach (Material mat in meshRenderer.materials)
            {
                mat.color *= color;
            }
            RL.writeTraceExit(null);
        }

        public override void LeftRangeToBeTouched()
        {
            RL.writeTraceEntry();
            for (int i = 0; i < originalColours.Length; i++)
            {
                meshRenderer.materials[i].color = originalColours[i];
            }
            RL.writeTraceExit(null);
        }


        #endregion
        /* Code for Server only runs here i.e. Commands and relevants */
        #region Server

        public void initialize(int newItem)
        {
            RL.writeTraceEntry();
            if (newItem == CONSTANTS.NO_ITEM)
            {
                empty = true;
                item = null;
                itemID = CONSTANTS.NO_ITEM;
            }
            else
            {
                Item i = Item.GetItem(newItem);
                if (i != null)
                {
                    item = i;
                    empty = false;
                    itemID = newItem;
                }
                else
                {
                    RL.writeError("Something tried to set the item of inventory " + this.name + " to " + newItem + " which returned null");
                }
            }
            RL.writeTraceExit(null);
        }

        public override void Interact(PlayerController interactor)
        {
            RL.writeTraceEntry(interactor);
            if (isBeingInteracted())
            {
                RL.writeWarning("Inventory already being fondled");
                return;
            }
            base.Interact(interactor);
            rummage(interactor);
            RL.writeTraceExit(null);
        }

        void rummage(PlayerController interactor)
        {
            RL.writeTraceEntry(interactor);
            if (empty)
            {
                RL.writeInfo(RobLogger.LogLevel.STANDARD, "Inventory empty!");
                return;
            }

            interactor.AddItem(item);

            SetCont(CONSTANTS.NO_ITEM);
            RL.writeTraceExit(null);
        }

        public void SetCont(int newItem)
        {
            RL.writeTraceEntry(newItem);
            if (newItem == CONSTANTS.NO_ITEM)
            {
                empty = true;
                item = null;
                itemID = CONSTANTS.NO_ITEM;
            }
            else
            {
                Item i = Item.GetItem(newItem);
                if (i != null)
                {
                    empty = false;
                    item = i;
                    itemID = newItem;
                }
                else
                {
                    RL.writeError("Something tried to set the item of inventory " + this.name + " to " + newItem + " which returned null");
                }
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