using UnityEngine;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.SinglePlayer
{
    public class NPCController : IInteractable
    {
        public enum RELATION
        {
            PARENT,
            SIBLING,
            CHILD,
            FRIEND,
            ENEMY,
            ACQUAINTANCE,
            UNKNOWN
        }

        bool initialized = false;

        public bool male;
        public string firstname;
        public string lastname;
        public string fullName;
        public int age;

        public int attitude;

        #region Variables
        /* publics */

        /* privates */
        CharacterController cc;

        float speed = 6f;
        float turnSmoothTime = 0.1f;
        float turnSmoothVelocity;

        float FramesToGo = 0;
        float hoz;
        float ver;

        float MAX_SECONDS = 4f;
        float MIN_SECONDS = 1f;

        DialogSystem dialogSystem = new DialogSystem();

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

        /* Sync variables that are kept in sync on Client/Server */
        #region SyncVariables

        #endregion
        #endregion
        /* All methods called by the above SyncVariables */
        #region SyncVarMethods

        #endregion
        /* Generic Code starts here i.e. Both Server/Client */
        #region Generic

        public void initialize(bool mf, string fn, string ln, int a)
        {
            RL.writeTraceEntry(mf, fn, ln, a);
            initialized = true;
            firstname = fn;
            lastname = ln;
            fullName = firstname + " " + lastname;
            this.gameObject.name = fullName;
            male = mf;
            age = a;
            RL.writeTraceExit(null);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            RL.writeTraceEntry();
            cc = GetComponent<CharacterController>();
            if (!initialized)
            {
                RL.writeWarning("NPC not initialized");
            }
            RL.writeTraceExit(null);
        }

        // Update is called once per frame
        void Update()
        {
            /* Only server should move! */
            if (isBeingInteracted())
            {
                // It's rude to walk away from someone talking to you...
            }
            else
            {
                MoveNPC();
            }
        }

        #endregion
        /* Code for Server only runs here i.e. Commands and relevants */
        #region Server

        void MoveNPC()
        {
            if (!cc.isGrounded)
            {
                cc.Move(new Vector3(0, -1, 0) * speed * Time.deltaTime);
            }

            if (FramesToGo <= 0f)
            {
                hoz = Random.Range(-1f, 1f);
                ver = Random.Range(-1f, 1f);
                FramesToGo = Random.Range(MIN_SECONDS, MAX_SECONDS);
                // RL.writeInfo("NPC(" + name + ") moves for " + FramesToGo + "seconds - (" + hoz + "," + ver + ")");
            }
            else
            {
                FramesToGo -= Time.deltaTime;
            }

            Vector3 dir = new Vector3(hoz, -0.5f, ver).normalized;

            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                cc.Move(moveDir * speed * Time.deltaTime);
            }
        }

        #endregion
        /* Code for Client only runs here i.e. ClientRPC and relevants */
        #region Client

        public override void EndInteraction()
        {
            RL.writeTraceEntry();
            base.EndInteraction();
            ResetDialogs();
            RL.writeTraceExit(null);
        }

        public string GetDialogQuestion(int branch)
        {
            RL.writeTraceEntry(branch);
            string toBack = dialogSystem.GetDialogTreeQuestion(branch);
            RL.writeTraceExit(toBack);
            return toBack;
        }

        public string GetDialogResponse(int branch)
        {
            RL.writeTraceEntry(branch);
            DialogSystem.dialogResponse TheBack = dialogSystem.GetDialogTreeResponse(branch);
            attitude += TheBack.attitudeChange;
            if (attitude > 100)
            {
                attitude = 100;
            }
            else if (attitude < -100)
            {
                attitude = -100;
            }
            string toBack = TheBack.answer;
            RL.writeTraceExit(toBack);
            return toBack;
        }

        public void ResetDialogs()
        {
            RL.writeTraceEntry();
            dialogSystem.ResetDialogs();
            RL.writeTraceExit(null);
        }

        #endregion
        #region DebugFunctions

        #endregion
    }
}