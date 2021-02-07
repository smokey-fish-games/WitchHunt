using UnityEngine;
using System.Collections.Generic;


public class PlayerControllerSP : MonoBehaviour
{
    #region Variables
    /* publics */
    public Material playerMat;
    public Material teamMat;

    /* privates */
    RobLogger RL;
    MeshRenderer[] allRenderers;
    CharacterController cc;
    Transform cam;
    float speed = 6f;
    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Item heldItem;
    InventoryUISP invUI;

    bool UIUp = false;
    bool Interacting = false;

    List<IInteractableSP> interactablesInRange = new List<IInteractableSP>();
    IInteractableSP closestInteractable;

    /* Sync variables that are kept in sync on Client/Server */
    #region SyncVariables
    int heldItemID;

    public string playerName = "DefaultPlayerName";
    #endregion
    #endregion

    /* All methods called by the above SyncVariables */
    #region SyncVarMethods

    void updateItem(int oldItem, int newItem)
    {
        heldItemID = newItem;
        heldItem = Item.GetItem(heldItemID);
    }

    void Setname(string oldValue, string newValue)
    {
        playerName = newValue;
        gameObject.name = newValue;
    }
    #endregion
    /* Generic Code starts here i.e. Both Server/Client */
    #region Generic

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        cc = GetComponent<CharacterController>();

        invUI = FindObjectOfType<InventoryUISP>();

        /* Setup colours */
        allRenderers = GetComponentsInChildren<MeshRenderer>();
        Material m = null;
        m = playerMat;

        foreach (MeshRenderer ren in allRenderers)
        {
            ren.material = m;
            ren.UpdateGIMaterials();
        }

        cam = Camera.main.transform;
        //Setup follow cam
        Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
        c.m_LookAt = this.transform;
        c.m_Follow = this.transform;
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        RL = RobLogger.GetRobLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIUp)
        {
            /* UI will manage how to stop */
            return;
        }

        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttempInteract();
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (Camera.main != null)
        {
            Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
            SpawnMarker s = SpawnMarker.GetAll(SpawnMarker.SpawnType.PLAYER)[0];
            c.m_LookAt = s.transform;
            c.m_Follow = s.transform;
        }
    }
    #endregion

    /* Code for Server only runs here i.e. Commands and relevants */
    #region Server
    void CmdCancelInteract()
    {
        closestInteractable.EndInteraction();
    }

    void CmdAttempInteract()
    {
        closestInteractable = null;
        foreach (IInteractableSP i in interactablesInRange)
        {
            RL.writeInfo("Server beleives IInteractable " + i.name + " is in range.");
        }

        float minDist = Mathf.Infinity;
        foreach (IInteractableSP g in interactablesInRange)
        {
            if (g.tag == "box")
            {
                // Found the a box Raycast to it
                RaycastHit hit;
                Ray ray = new Ray(transform.position, g.transform.position - transform.position);
                // Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.red, 2f);

                if (Physics.Raycast(ray, out hit, minDist))
                {
                    Debug.Log("Hit " + hit.collider.name);
                    if (hit.collider != null && hit.collider.tag == "box")
                    {
                        // Check if it's closest
                        //Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.yellow, 2f);


                        float dist = Vector3.Distance(g.transform.position, transform.position);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            closestInteractable = hit.collider.gameObject.GetComponent<IInteractableSP>();
                        }
                    }
                }
            }
        }

        if (closestInteractable != null)
        {
            if (closestInteractable.isBeingInteracted())
            {
                /* can't touch it because it's being touched by someone else! */
            }
            else
            {
                // Ok do it then!
                closestInteractable.StartInteraction();
                TargetStartInteract();
            }
        }
    }

    void CmdTakeItem()
    {
        if (closestInteractable == null)
        {
            return;
        }
        InventorySP inven = closestInteractable.GetComponent<InventorySP>();
        if (inven == null)
        {
            return;
        }
        Item it = inven.getItem();
        if (it == null)
        {
            return;
        }
        if (it.ID != CONSTANTS.NO_ITEM && it.ID != CONSTANTS.ITEM_DESTROYED)
        {
            heldItemID = it.ID;
            updateItem(0, it.ID);
            inven.SetCont(CONSTANTS.NO_ITEM);
        }
    }

    void CmdDestroyItem()
    {
        if (closestInteractable == null)
        {
            return;
        }
        InventorySP inven = closestInteractable.GetComponent<InventorySP>();
        if (inven == null)
        {
            return;
        }
        Item it = inven.getItem();
        if (it == null)
        {
            return;
        }
        if (it.ID != CONSTANTS.NO_ITEM)
        {
            inven.SetCont(CONSTANTS.ITEM_DESTROYED);
        }
    }

    public void AddItem(Item item)
    {
        //TODO more
        RL.writeInfo("Player " + name + " got an item! " + item);
        heldItemID = item.ID;
        updateItem(0, item.ID);
    }

    #endregion
    /* Code for Client only runs here i.e. ClientRPC and relevants */
    #region Client

    public void DestroyItem()
    {
        CmdDestroyItem();
    }
    public void TakeItem()
    {
        CmdTakeItem();
    }

    void Move()
    {
        if (!cc.isGrounded)
        {
            cc.Move(new Vector3(0, -1, 0) * speed * Time.deltaTime);
        }

        float hoz = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(hoz, 0, ver).normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            cc.Move(moveDir * speed * Time.deltaTime);
        }
    }

    void AttempInteract()
    {
        closestInteractable = null;
        float minDist = Mathf.Infinity;
        foreach (IInteractableSP g in interactablesInRange)
        {
            if (g.tag == "box")
            {
                // Found the a box Raycast to it
                RaycastHit hit;
                Ray ray = new Ray(transform.position, g.transform.position - transform.position);
                // Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.red, 2f);

                if (Physics.Raycast(ray, out hit, minDist))
                {
                    if (hit.collider != null && hit.collider.tag == "box")
                    {
                        // Check if it's closest
                        //Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.yellow, 2f);

                        float dist = Vector3.Distance(g.transform.position, transform.position);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            closestInteractable = hit.collider.gameObject.GetComponent<IInteractableSP>();
                        }
                    }
                }
            }
        }

        if (closestInteractable != null)
        {
            if (closestInteractable.isBeingInteracted())
            {
                /* can't touch it because it's being touched by someone else! */
            }
            else
            {
                /* requesting server checks we can do this baby */
                CmdAttempInteract();
            }
        }
    }

    void TargetStartInteract()
    {
        // Start rummaging, wait for x amount of time then do it
        UIUp = true;
        Interacting = true;
        invUI.STARTINVENTORY(closestInteractable.GetComponent<InventorySP>(), this);
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        IInteractableSP inte = other.GetComponent<IInteractableSP>();

        if (inte != null)
        {
            RL.writeInfo("Player " + name + " Interactable " + other.name + " in range!");
            interactablesInRange.Add(inte);
            inte.InRangeToBeTouched();
        }
    }

    public void CancelUI()
    {
        if (Interacting)
        {
            CmdCancelInteract();
        }
        Interacting = false;

        UIUp = false;
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        IInteractableSP inte = other.GetComponent<IInteractableSP>();

        if (inte != null)
        {
            RL.writeInfo("Player " + name + " Interactable " + other.name + " left range!");
            interactablesInRange.Remove(inte);
            inte.LeftRangeToBeTouched();
        }
    }


    #endregion
    #region DebugFunctions

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 offset = new Vector3(0f, 0f, .25f);
        // close enough
        Gizmos.DrawWireCube(this.transform.position + transform.forward + offset, new Vector3(2f, 2f, 1.5f));
    }

    #endregion
}

