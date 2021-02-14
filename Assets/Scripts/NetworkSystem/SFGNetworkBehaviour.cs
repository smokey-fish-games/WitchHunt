using Mirror;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG.WitchHunt.NetworkSystem
{
    public class SFGNetworkBehaviour : NetworkBehaviour
    {
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

        /// Virtual function to override to send custom serialization data. The corresponding function to send serialization data is OnDeserialize().
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            RL.writeTraceEntry(writer, initialState);
            bool back = base.OnSerialize(writer, initialState);
            RL.writeTraceExit(back);
            return back;
        }

        /// Virtual function to override to receive custom serialization data. The corresponding function to send serialization data is OnSerialize().
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            RL.writeTraceEntry(reader, initialState);
            base.OnDeserialize(reader, initialState);
            RL.writeTraceExit(null);
        }

        // Don't rename. Weaver uses this exact function name.
        protected override bool SerializeSyncVars(NetworkWriter writer, bool initialState)
        {
            RL.writeTraceEntry(writer, initialState);
            bool back = base.SerializeSyncVars(writer, initialState);
            RL.writeTraceExit(back);
            return back;
        }

        // Don't rename. Weaver uses this exact function name.
        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            RL.writeTraceEntry(reader, initialState);
            base.DeserializeSyncVars(reader, initialState);
            RL.writeTraceExit(null);
        }

        /// This is invoked on clients when the server has caused this object to be destroyed.
        public override void OnStopClient()
        {
            RL.writeTraceEntry();
            base.OnStopClient();
            RL.writeTraceExit(null);
        }

        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        public override void OnStartServer()
        {
            RL.writeTraceEntry();
            base.OnStartServer();
            RL.writeTraceExit(null);
        }

        /// Invoked on the server when the object is unspawned
        public override void OnStopServer()
        {
            RL.writeTraceEntry();
            base.OnStopServer();
            RL.writeTraceExit(null);
        }

        /// Called on every NetworkBehaviour when it is activated on a client.
        public override void OnStartClient()
        {
            RL.writeTraceEntry();
            base.OnStartClient();
            RL.writeTraceExit(null);
        }

        /// Called when the local player object has been set up.
        public override void OnStartLocalPlayer()
        {
            RL.writeTraceEntry();
            base.OnStartLocalPlayer();
            RL.writeTraceExit(null);
        }

        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        public override void OnStartAuthority()
        {
            RL.writeTraceEntry();
            base.OnStartAuthority();
            RL.writeTraceExit(null);
        }

        /// This is invoked on behaviours when authority is removed.
        public override void OnStopAuthority()
        {
            RL.writeTraceEntry();
            base.OnStopAuthority();
            RL.writeTraceExit(null);
        }
    }
}

