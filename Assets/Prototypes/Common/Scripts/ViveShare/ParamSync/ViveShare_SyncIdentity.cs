//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_SyncIdentity : MonoBehaviour
    {
        // object id for sync
        public string id = null;

        // whehter this object is a parameter sender
        public bool hasAuthority = false;

        public virtual void OnEnable()
        {
            if(string.IsNullOrEmpty(id) == false)
            {
                Register();
            }
        }

        public virtual void OnDisable()
        {
            UnRegister();
        }

        public void Register()
        {
            ViveShare_MessageHandler.Instance.AddSyncIdentity(this);
        }

        public void UnRegister()
        {
            ViveShare_MessageHandler.Instance.RemoveSyncIdentity(this);
        }
    }
}
