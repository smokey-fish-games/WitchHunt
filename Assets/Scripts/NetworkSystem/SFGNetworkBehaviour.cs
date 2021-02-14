using Mirror;

namespace SFG.WitchHunt.NetworkSystem
{
    public class SFGNetworkBehaviour : NetworkBehaviour
    {
        RobLogger RL;

        void WriteALog(string mes)
        {
            string toWrite = string.Empty;
            toWrite += "NetBeha(" + this.netId + ") " + mes;
            if (RL == null)
            {
                RL = RobLogger.GetRobLogger();
            }
            RL.writeInfo(toWrite);
        }

        /// Virtual function to override to send custom serialization data. The corresponding function to send serialization data is OnDeserialize().
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            WriteALog("OnSerialize called");
            return base.OnSerialize(writer, initialState);
        }

        /// Virtual function to override to receive custom serialization data. The corresponding function to send serialization data is OnSerialize().
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            WriteALog("OnDeserialize called");
            base.OnDeserialize(reader, initialState);
        }

        // Don't rename. Weaver uses this exact function name.
        protected override bool SerializeSyncVars(NetworkWriter writer, bool initialState)
        {
            WriteALog("SerializeSyncVars called");
            return base.SerializeSyncVars(writer, initialState);
        }

        // Don't rename. Weaver uses this exact function name.
        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            WriteALog("DeserializeSyncVars called");
            base.DeserializeSyncVars(reader, initialState);
        }

        /// This is invoked on clients when the server has caused this object to be destroyed.
        public override void OnStopClient()
        {
            WriteALog("OnStopClient called");
            base.OnStopClient();
        }

        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        public override void OnStartServer()
        {
            WriteALog("OnStartServer called");
            base.OnStartServer();
        }

        /// Invoked on the server when the object is unspawned
        public override void OnStopServer()
        {
            WriteALog("OnStopServer called");
            base.OnStopServer();
        }

        /// Called on every NetworkBehaviour when it is activated on a client.
        public override void OnStartClient()
        {
            WriteALog("OnStartClient called");
            base.OnStartClient();
        }

        /// Called when the local player object has been set up.
        public override void OnStartLocalPlayer()
        {
            WriteALog("OnStartLocalPlayer called");
            base.OnStartLocalPlayer();
        }

        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        public override void OnStartAuthority()
        {
            WriteALog("OnStartAuthority called");
            base.OnStartAuthority();
        }

        /// This is invoked on behaviours when authority is removed.
        public override void OnStopAuthority()
        {
            WriteALog("OnStopAuthority called");
            base.OnStopAuthority();
        }
    }
}
