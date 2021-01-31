using UnityEngine;
using Mirror;


[RequireComponent(typeof(ColourWhenNear))]
public class IInteractable : NetworkBehaviour
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

    // Called when the object is no longer interacted
    public void OnDefocused()
    {
        beingInteracted = false;
    }
    #endregion

    /* Code for Server only runs here i.e. Commands and relevants */
    #region Server

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
