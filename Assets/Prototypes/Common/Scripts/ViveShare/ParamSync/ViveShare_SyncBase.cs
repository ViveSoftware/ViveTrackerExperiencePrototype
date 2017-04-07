//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

namespace HTC.UnityPlugin.ViveShare
{
    [RequireComponent(typeof(ViveShare_SyncIdentity))]
    [DisallowMultipleComponent]
    public class ViveShare_SyncBase : MonoBehaviour
    {
        // identity object
        protected ViveShare_SyncIdentity identity;

        // sync message send interval
        public float sendInterval = 0.1f;
        private float lastSendTime = 0.0f;

        public virtual void OnEnable()
        {
            identity = GetComponent<ViveShare_SyncIdentity>();
        }

        //-----------------------------------------------------------------------------
        // network message
        //-----------------------------------------------------------------------------

        public bool GenerateMessage(out ViveShareMessage outMsg)
        {
            outMsg = null;

            if (identity.hasAuthority && (Time.time - lastSendTime > sendInterval))
            {
                object[] paramList = GenerateParamList();

                ViveShare_ParamMessage paramMsg = new ViveShare_ParamMessage();
                paramMsg.id = identity.id;
                paramMsg.timeStamp = Time.time;
                paramMsg.objects = paramList;

                outMsg = new ViveShareMessage();
                outMsg.type = ViveShareMsgType.SyncParam;
                outMsg.body = paramMsg;

                lastSendTime = Time.time;
                return true;
            }

            return false;
        }

        public virtual object[] GenerateParamList() { return null; }
        public virtual void SetParams(float timeStamp, object[] paramList) { }
    }
}
