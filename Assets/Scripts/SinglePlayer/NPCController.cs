﻿using UnityEngine;

namespace SFG.WitchHunt.SinglePlayer
{
    public class NPCController : MonoBehaviour
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

        RobLogger RL;

        /* Sync variables that are kept in sync on Client/Server */
        #region SyncVariables

        #endregion
        #endregion
        /* All methods called by the above SyncVariables */
        #region SyncVarMethods

        #endregion
        /* Generic Code starts here i.e. Both Server/Client */
        #region Generic

        void Awake()
        {
            RL = RobLogger.GetRobLogger();
        }

        public void initialize(bool mf, string fn, string ln, int a)
        {
            initialized = true;
            firstname = fn;
            lastname = ln;
            male = mf;
            age = a;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            cc = GetComponent<CharacterController>();
            if (!initialized)
            {
                RL.writeWarning("NPC not initialized");
            }
        }

        // Update is called once per frame
        void Update()
        {
            /* Only server should move! */
            MoveNPC();
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

        #endregion
        #region DebugFunctions

        #endregion
    }
}