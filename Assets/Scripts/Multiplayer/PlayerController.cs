using UnityEngine;
using Mirror;
using System.Collections.Generic;
using SFG.WitchHunt.NetworkSystem;

namespace SFG.WitchHunt.MultiPlayer
{
    public class PlayerController : SFGNetworkBehaviour
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
        InventoryUI invUI;

        bool UIUp = false;
        bool Interacting = false;

        List<IInteractable> interactablesInRange = new List<IInteractable>();
        IInteractable closestInteractable;

        /* Sync variables that are kept in sync on Client/Server */
        #region SyncVariables
        [SyncVar(hook = ("updateItem"))]
        int heldItemID;

        [SyncVar]
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

            invUI = FindObjectOfType<InventoryUI>();

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

            if (cam == null)
            {
                cam = Camera.main.transform;
                //Setup follow cam
                Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
                if (c != null)
                {
                    c.m_LookAt = this.transform;
                    c.m_Follow = this.transform;
                }
                else
                {
                    cam = null;
                }
            }

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

        // /// <summary>
        // /// This function is called when the MonoBehaviour will be destroyed.
        // /// </summary>
        // void OnDestroy()
        // {
        //     if (isLocalPlayer)
        //     {
        //         if (Camera.main != null)
        //         {
        //             Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
        //             SpawnMarker s = SpawnMarker.GetAll(SpawnMarker.SpawnType.PLAYER)[0];
        //             c.m_LookAt = s.transform;
        //             c.m_Follow = s.transform;
        //         }
        //     }
        // }
        #endregion

        /* Code for Server only runs here i.e. Commands and relevants */
        #region Server

        [Command]
        void CmdCancelInteract()
        {
            closestInteractable.EndInteraction();
        }

        [Command]
        void CmdAttempInteract()
        {
            closestInteractable = null;
            foreach (IInteractable i in interactablesInRange)
            {
                RL.writeInfo("Server beleives IInteractable " + i.name + " is in range.");
            }

            float minDist = Mathf.Infinity;
            foreach (IInteractable g in interactablesInRange)
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
                                closestInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
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

        [Command]
        void CmdTakeItem()
        {
            if (closestInteractable == null)
            {
                return;
            }
            Inventory inven = closestInteractable.GetComponent<Inventory>();
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
                inven.SetCont(CONSTANTS.NO_ITEM);
            }
        }

        [Command]
        void CmdDestroyItem()
        {
            if (closestInteractable == null)
            {
                return;
            }
            Inventory inven = closestInteractable.GetComponent<Inventory>();
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

        // public override void OnStartLocalPlayer()
        // {
        //     if (!isLocalPlayer)
        //     {
        //         return;
        //     }

        //     cam = Camera.main.transform;
        //     //Setup follow cam
        //     Cinemachine.CinemachineFreeLook c = Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>();
        //     c.m_LookAt = this.transform;
        //     c.m_Follow = this.transform;

        //     base.OnStartLocalPlayer();
        // }

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
            foreach (IInteractable g in interactablesInRange)
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
                                closestInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
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

        [TargetRpc]
        void TargetStartInteract()
        {
            // Start rummaging, wait for x amount of time then do it
            UIUp = true;
            Interacting = true;
            invUI.STARTINVENTORY(closestInteractable.GetComponent<Inventory>(), this);
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            IInteractable inte = other.GetComponent<IInteractable>();

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
            IInteractable inte = other.GetComponent<IInteractable>();

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

        #region FromRoomCode

        private Lobby room;
        private Lobby Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as Lobby;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            DontDestroyOnLoad(gameObject);

            Room.GamePlayers.Add(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.playerName = displayName;
        }

        #endregion
    }
}
