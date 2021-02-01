using UnityEngine;
using System.Linq;
using Mirror;

public class Inventory : IInteractable
{
    #region Variables
    /* publics */

    /* privates */
    Item item;
    RobLogger RL;

    Color color;
    Renderer meshRenderer;

    Color[] originalColours;

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

    public Item getItem()
    {
        return item;
    }
    void Awake()
    {
        RL = RobLogger.GetRobLogger();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        originalColours = meshRenderer.materials.Select(x => x.color).ToArray();
    }

    public override void InRangeToBeTouched()
    {
        foreach (Material mat in meshRenderer.materials)
        {
            mat.color *= color;
        }
    }

    public override void LeftRangeToBeTouched()
    {
        for (int i = 0; i < originalColours.Length; i++)
        {
            meshRenderer.materials[i].color = originalColours[i];
        }
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

    public void SetCont(int newItem)
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
