//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_MessageHandler : Singleton<ViveShare_MessageHandler>
    {
        // for parameter sync objects
        private List<ViveShare_SyncIdentity> syncIdList = new List<ViveShare_SyncIdentity>();
        private Dictionary<string, ViveShare_SyncIdentity> syncIdDictionary = new Dictionary<string, ViveShare_SyncIdentity>();

        // for networked operations
        private Queue<ViveShareMessage> messageQueue = new Queue<ViveShareMessage>();

        //-----------------------------------------------------------------------------
        // message aggregation
        //-----------------------------------------------------------------------------

        public List<ViveShareMessage> GenerateMessagesList()
        {
            List<ViveShareMessage> list = new List<ViveShareMessage>();

            // dump message from queue
            while (messageQueue.Count > 0)
            {
                ViveShareMessage msg = messageQueue.Dequeue();
                list.Add(msg);
            }

            // append sync param information to list
            for (int i = 0; i < syncIdList.Count; i++)
            {
                if(syncIdList[i].hasAuthority == false)
                    continue;

                ViveShareMessage msg;
                ViveShare_SyncBase syncObj = GetSyncObjFromSyncId(syncIdList[i]);
                if (syncObj != null && syncObj.GenerateMessage(out msg))
                {
                    list.Add(msg);
                }
            }

            return list;
        }

        //-----------------------------------------------------------------------------
        // networked operation
        //-----------------------------------------------------------------------------

        public void AddEventMessage(string eventID, object[] args)
        {
            ViveShare_ParamMessage eventMsg = new ViveShare_ParamMessage();
            eventMsg.id = eventID;
            eventMsg.timeStamp = Time.time;
            eventMsg.objects = args;

            ViveShareMessage msg = new ViveShareMessage();
            msg.type = ViveShareMsgType.Event;
            msg.body = eventMsg;

            messageQueue.Enqueue(msg);
        }

        public void AddSpawnMessage(string assetName, string objID, Vector3 position, Quaternion rotation)
        {
            ViveNetworkSpawnMessage spawnMsg = new ViveNetworkSpawnMessage();
            spawnMsg.assetName = assetName;
            spawnMsg.objectID = objID;
            spawnMsg.position = position;
            spawnMsg.eulerRotation = rotation.eulerAngles;

            ViveShareMessage viveMsg = new ViveShareMessage();
            viveMsg.type = ViveShareMsgType.Spawn;
            viveMsg.body = spawnMsg;

            messageQueue.Enqueue(viveMsg);
        }

        public void AddDespawnMessage(string objID)
        {
            ViveShareMessage viveMsg = new ViveShareMessage();
            viveMsg.type = ViveShareMsgType.UnSpawn;
            viveMsg.body = new StringMessage(objID);

            messageQueue.Enqueue(viveMsg);
        }

        //-----------------------------------------------------------------------------
        // param sync objects
        //-----------------------------------------------------------------------------

        public void AddSyncIdentity(ViveShare_SyncIdentity syncId)
        {
            syncIdList.Add(syncId);
            syncIdDictionary.Add(syncId.id, syncId);
        }

        public void RemoveSyncIdentity(ViveShare_SyncIdentity syncId)
        {
            syncIdList.Remove(syncId);
            syncIdDictionary.Remove(syncId.id);
        }

        public void DeliverSyncParamMessage(string id, float timeStamp, object[] paramList)
        {
            ViveShare_SyncIdentity syncId;
            if(syncIdDictionary.TryGetValue(id, out syncId) == false || syncId.hasAuthority == true)
            {
                return;
            }

            ViveShare_SyncBase syncObj = GetSyncObjFromSyncId(syncId);
            if (syncObj != null)
            {
                syncObj.SetParams(timeStamp, paramList);
            }
        }

        public ViveShare_SyncBase GetSyncObjFromSyncId(ViveShare_SyncIdentity syncId)
        {
            return syncId.gameObject.GetComponent<ViveShare_SyncBase>();
        }
    }
}
