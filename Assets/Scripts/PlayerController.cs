using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
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
    float pickupRange = 5f;
    Item heldItem;

    /* Sync variables that are kept in sync on Client/Server */
    #region SyncVariables
    [SyncVar(hook = ("updateItem"))]
    int heldItemID;

    [SyncVar(hook = ("Setname"))]
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

        if (isServer)
        {
            RL.writeInfo("Player Server hello!!!");
        }
        if (isClient)
        {
            RL.writeInfo("Player Client hello!!!");
        }
        if (isLocalPlayer)
        {
            RL.writeInfo("Player LocalPlayer hello!!!");
        }
        else
        {
            this.tag = "Team";
        }

        cc = GetComponent<CharacterController>();

        /* Setup colours */
        allRenderers = GetComponentsInChildren<MeshRenderer>();
        Material m = null;
        if (!isLocalPlayer)
        {
            m = teamMat;
        }
        else
        {
            m = playerMat;
        }

        foreach (MeshRenderer ren in allRenderers)
        {
            ren.material = m;
            ren.UpdateGIMaterials();
        }
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
        if (!isLocalPlayer)
        {
            return;
        }

        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdAttempInteract();
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            if (Camera.main != null)
            {
                Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
                SpawnMarker s = SpawnMarker.GetAll(SpawnMarker.SpawnType.PLAYER)[0];
                c.m_LookAt = s.transform;
                c.m_Follow = s.transform;
            }
        }
    }
    #endregion

    /* Code for Server only runs here i.e. Commands and relevants */
    #region Server

    [Command]
    void CmdAttempInteract()
    {
        IInteractable[] all = FindObjectsOfType<IInteractable>();
        IInteractable closestInteract = null;
        float minDist = Mathf.Infinity;
        foreach (IInteractable g in all)
        {
            if (g.tag == "box")
            {
                // Found the a box Raycast to it
                RaycastHit hit;
                Ray ray = new Ray(transform.position, g.transform.position - transform.position);
                // Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.red, 2f);

                if (Physics.Raycast(ray, out hit, pickupRange))
                {
                    if (hit.collider != null && hit.collider.tag == "box")
                    {
                        // Check if it's closest
                        //Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.yellow, 2f);

                        float dist = Vector3.Distance(g.transform.position, transform.position);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            closestInteract = hit.collider.gameObject.GetComponent<IInteractable>();
                        }
                    }
                }
            }
        }

        if (closestInteract != null)
        {
            RL.writeInfo("Interacting with closestInteract!");
            Debug.DrawRay(transform.position, closestInteract.transform.position - transform.position, Color.green, 2f);

            // Ok do it then!
            closestInteract.Interact(this);
        }
    }

    public void AddItem(Item item)
    {
        //TODO more
        RL.writeInfo("Player " + name + " got an item! " + item);
        heldItemID = item.ID;
    }

    #endregion
    /* Code for Client only runs here i.e. ClientRPC and relevants */
    #region Client

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        cam = Camera.main.transform;
        //Setup follow cam
        Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
        c.m_LookAt = this.transform;
        c.m_Follow = this.transform;

        base.OnStartLocalPlayer();
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


    #endregion
    #region DebugFunctions

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, pickupRange);
    }

    #endregion
}

