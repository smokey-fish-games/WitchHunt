using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using SFG.WitchHunt.SinglePlayer.UI;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.SinglePlayer
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        /* publics */
        public Material playerMat;
        public Material teamMat;

        /* privates */
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

        MeshRenderer[] allRenderers;
        CharacterController cc;
        Transform cam;
        public CinemachineFreeLook camControl;
        float speed = 6f;
        float turnSmoothTime = 0.1f;
        float turnSmoothVelocity;
        public Item heldItem;
        UIController uis;

        bool UIUp = false;
        bool Interacting = false;

        List<IInteractable> interactablesInRange = new List<IInteractable>();
        IInteractable closestInteractable;

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
            RL.writeTraceEntry(oldItem, newItem);
            heldItemID = newItem;
            heldItem = Item.GetItem(heldItemID);
            RL.writeTraceExit(null);
        }

        // void Setname(string oldValue, string newValue)
        // {
        //     playerName = newValue;
        //     gameObject.name = newValue;
        // }
        #endregion
        /* Generic Code starts here i.e. Both Server/Client */
        #region Generic

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            RL.writeTraceEntry();

            cc = GetComponent<CharacterController>();
            uis = FindObjectOfType<UIController>();

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
            RL.writeTraceExit(null);
        }

        // Update is called once per frame
        void Update()
        {
            if (UIUp)
            {
                camControl.m_XAxis.m_MaxSpeed = 0;
                camControl.m_YAxis.m_MaxSpeed = 0;
                /* UI will manage how to stop */
                return;
            }

            /* Camera Controls */
            if (camControl.m_YAxis.m_MaxSpeed == 0)
            {
                camControl.m_YAxis.m_MaxSpeed = 1;
            }
            if (Input.GetMouseButtonDown(1))
            {
                camControl.m_XAxis.m_MaxSpeed = 2;
            }
            if (Input.GetMouseButtonUp(1))
            {
                camControl.m_XAxis.m_MaxSpeed = 0;
            }


            Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttempInteract();
            }
        }

        #endregion

        /* Code for Server only runs here i.e. Commands and relevants */
        #region Server
        void CmdCancelInteract()
        {
            RL.writeTraceEntry();
            closestInteractable.EndInteraction();
            RL.writeTraceExit(null);
        }

        void CmdAttempInteract()
        {
            RL.writeTraceEntry();
            closestInteractable = null;

            float minDist = Mathf.Infinity;
            foreach (IInteractable g in interactablesInRange)
            {
                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Server beleives IInteractable " + g.name + " is in range.");
                // Found the a box Raycast to it
                RaycastHit hit;
                Ray ray = new Ray(transform.position, g.transform.position - transform.position);
                // Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.red, 2f);

                if (Physics.Raycast(ray, out hit, minDist))
                {
                    if (hit.collider != null)
                    {
                        IInteractable isInter = hit.collider.gameObject.GetComponent<IInteractable>();
                        if (isInter != null)
                        {
                            RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Possible Interaction with: " + isInter.gameObject.name);
                            // Check if it's closest
                            //Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.yellow, 2f);


                            float dist = Vector3.Distance(g.transform.position, transform.position);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                closestInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
                                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "New Closest: " + isInter.gameObject.name);
                            }
                        }
                        else
                        {
                            RL.writeInfo(RobLogger.LogLevel.VERBOSE, "gameobject " + hit.collider.gameObject.name + " is not interactable");
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
            RL.writeTraceExit(null);
        }

        void CmdTakeItem()
        {
            RL.writeTraceEntry();
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
                updateItem(0, it.ID);
                inven.SetCont(CONSTANTS.NO_ITEM);
            }
            RL.writeTraceExit(null);
        }

        void CmdDestroyItem()
        {
            RL.writeTraceEntry();
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
            RL.writeTraceExit(null);
        }

        public void AddItem(Item item)
        {
            RL.writeTraceEntry(item);
            //TODO more
            heldItemID = item.ID;
            updateItem(0, item.ID);
            RL.writeTraceExit(null);
        }

        #endregion
        /* Code for Client only runs here i.e. ClientRPC and relevants */
        #region Client

        public void DestroyItem()
        {
            RL.writeTraceEntry();
            CmdDestroyItem();
            RL.writeTraceExit(null);
        }
        public void TakeItem()
        {
            RL.writeTraceEntry();
            CmdTakeItem();
            RL.writeTraceExit(null);
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
            RL.writeTraceEntry();
            closestInteractable = null;
            float minDist = Mathf.Infinity;
            foreach (IInteractable g in interactablesInRange)
            {
                // Found the a box Raycast to it
                RaycastHit hit;
                Ray ray = new Ray(transform.position, g.transform.position - transform.position);
                // Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.red, 2f);

                if (Physics.Raycast(ray, out hit, minDist))
                {
                    if (hit.collider != null)
                    {
                        IInteractable isInter = hit.collider.gameObject.GetComponent<IInteractable>();
                        if (isInter != null)
                        {
                            RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Possible Interaction with: " + isInter.gameObject.name);
                            // Check if it's closest
                            //Debug.DrawRay(transform.position, g.transform.position - transform.position, Color.yellow, 2f);

                            float dist = Vector3.Distance(g.transform.position, transform.position);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                closestInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
                                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "New Closest: " + isInter.gameObject.name);
                            }
                        }
                        else
                        {
                            RL.writeInfo(RobLogger.LogLevel.VERBOSE, "gameobject " + hit.collider.gameObject.name + " is not interactable");
                        }
                    }
                }
            }

            if (closestInteractable != null)
            {
                if (closestInteractable.isBeingInteracted())
                {
                    /* can't touch it because it's being touched by someone else! */
                    RL.writeInfo(RobLogger.LogLevel.VERBOSE, closestInteractable.name + "is Already being interacted with");
                }
                else
                {
                    RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Initiating interaction with " + closestInteractable.name);
                    /* requesting server checks we can do this baby */
                    CmdAttempInteract();
                }
            }
            RL.writeTraceExit(null);
        }

        void TargetStartInteract()
        {
            RL.writeTraceEntry();
            // Start rummaging, wait for x amount of time then do it
            UIUp = true;
            Interacting = true;
            uis.OpenUI(closestInteractable, this);
            RL.writeTraceExit(null);
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            RL.writeTraceEntry(other);
            IInteractable inte = other.GetComponent<IInteractable>();

            if (inte != null)
            {
                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Player " + name + " Interactable " + other.name + " in range!");
                interactablesInRange.Add(inte);
                inte.InRangeToBeTouched();
            }
            RL.writeTraceExit(null);
        }

        public void CancelUI()
        {
            RL.writeTraceEntry();
            if (Interacting)
            {
                CmdCancelInteract();
            }
            Interacting = false;

            UIUp = false;
            RL.writeTraceExit(null);
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerExit(Collider other)
        {
            RL.writeTraceEntry(other);
            IInteractable inte = other.GetComponent<IInteractable>();

            if (inte != null)
            {
                RL.writeInfo(RobLogger.LogLevel.VERBOSE, "Player " + name + " Interactable " + other.name + " left range!");
                interactablesInRange.Remove(inte);
                inte.LeftRangeToBeTouched();
            }
            RL.writeTraceExit(null);
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
}