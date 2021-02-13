using UnityEngine;
using Mirror;
using SFG.NetworkSystem;

public class IInteractable : MyNetworkBehaviour2
{
    #region Variables
    /* publics */

    /* privates */


    /* Sync variables that are kept in sync on Client/Server */
    #region SyncVariables

    [SyncVar(hook = ("SetInteracted"))]
    bool beingInteracted = false;

    #endregion
    #endregion

    /* All methods called by the above SyncVariables */
    #region SyncVarMethods

    void SetInteracted(bool oldValue, bool newValue)
    {
        beingInteracted = newValue;
    }

    #endregion
    /* Generic Code starts here i.e. Both Server/Client */
    #region Generic

    public virtual void Interact(PlayerController interactor)
    {
        beingInteracted = true;
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
        beingInteracted = true;
    }

    public void EndInteraction()
    {
        beingInteracted = false;
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
