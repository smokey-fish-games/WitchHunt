using UnityEngine;


public class IInteractableSP : MonoBehaviour
{
    #region Variables
    /* publics */

    /* privates */
    bool beingInteracted = false;

    #endregion

    /* All methods called by the above SyncVariables */
    void SetInteracted(bool oldValue, bool newValue)
    {
        beingInteracted = newValue;
    }
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
