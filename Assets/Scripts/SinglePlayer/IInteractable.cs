using UnityEngine;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.SinglePlayer
{
    public class IInteractable : MonoBehaviour
    {
        #region Variables
        /* publics */

        /* privates */
        bool beingInteracted = false;
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

        #endregion

        /* All methods called by the above SyncVariables */
        void SetInteracted(bool oldValue, bool newValue)
        {
            RL.writeTraceEntry(oldValue, newValue);
            beingInteracted = newValue;
            RL.writeTraceExit(null);
        }
        /* Generic Code starts here i.e. Both Server/Client */
        #region Generic

        public virtual void Interact(PlayerController interactor)
        {
            RL.writeTraceEntry(interactor);
            beingInteracted = true;
            RL.writeTraceExit(null);
        }
        public bool isBeingInteracted()
        {
            return beingInteracted;
        }

        public virtual void InRangeToBeTouched()
        {
            // nothing todo here
        }

        public virtual void LeftRangeToBeTouched()
        {
            // nothing todo here
        }
        #endregion

        /* Code for Server only runs here i.e. Commands and relevants */
        #region Server

        public void StartInteraction()
        {
            RL.writeTraceEntry();
            beingInteracted = true;
            RL.writeTraceExit(null);
        }

        public void EndInteraction()
        {
            RL.writeTraceEntry();
            beingInteracted = false;
            RL.writeTraceExit(null);
        }

        #endregion
        /* Code for Client only runs here i.e. ClientRPC and relevants */
        #region Client

        #endregion
        #region DebugFunctions

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(3f, 3f, 3f));
        }
        #endregion
    }
}