using UnityEngine;
using Mirror;

public class Inventory : IInteractable
{
    #region Variables
    /* publics */


    /* privates */
    Item item;
    RobLogger RL;

    /* Sync variables that are kept in sync on Client/Server */
    #region SyncVariables
    [SyncVar]
    bool empty = false;

    [SyncVar(hook = nameof(SetItem))]
    int itemID;

    #endregion
    #endregion
    /* All methods called by the above SyncVariables */
    #region SyncVarMethods

    void SetItem(int oldValue, int newValue)
    {
        if (newValue == CONSTANTS.NO_ITEM)
        {
            item = null;
        }
        else
        {
            item = Item.GetItem(newValue);
        }
    }
    #endregion
    /* Generic Code starts here i.e. Both Server/Client */
    #region Generic

    void Awake()
    {
        RL = RobLogger.GetRobLogger();
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        OnDefocused();
    }

    #endregion
    /* Code for Server only runs here i.e. Commands and relevants */
    #region Server

    public void initialize(int newItem)
    {
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
    }

    public override void Interact(PlayerController interactor)
    {
        RL.writeInfo("Inventory Interact");
        if (isBeingInteracted())
        {
            RL.writeInfo("Inventory already being fondled");
            return;
        }
        base.Interact(interactor);
        rummage(interactor);
    }

    void rummage(PlayerController interactor)
    {
        RL.writeInfo("Rummaging");
        if (empty)
        {
            RL.writeInfo("Inventory empty!");
            return;
        }

        interactor.AddItem(item);

        SetCont(CONSTANTS.NO_ITEM);
    }

    void SetCont(int newItem)
    {
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
    }
    #endregion
    /* Code for Client only runs here i.e. ClientRPC and relevants */
    #region Client

    #endregion
    #region DebugFunctions

    #endregion
}
