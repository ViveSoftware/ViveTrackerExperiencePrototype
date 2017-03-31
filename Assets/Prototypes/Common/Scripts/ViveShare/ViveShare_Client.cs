//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_Client : SingletonBehaviour<ViveShare_Client>
    {
        // network configuration
        public string serverAddr = "localhost";
        public int portNumber = 2498;
        public float timeoutInterval = 10.0f;
        private int clientId = -1;

        // tracker configuration
        [SerializeField]
        private bool needTrackerPose = true;

        private string serialNumber = "";
        private int trackerNumber = -1; // 1-based value

        // log configuraion
        [SerializeField]
        private LogFilter.FilterLevel logLevel = (LogFilter.FilterLevel)LogFilter.Info;

        // network client
        private NetworkClient networkClient;
        private ViveNetworkState state = ViveNetworkState.Init;

        // recording message send time
        private float messageSendTimeStamp;

        // recording spawned objects
        private List<GameObject> spawnedObjList = new List<GameObject>();

        //-----------------------------------------------------------------------------
        // client life cycle
        //-----------------------------------------------------------------------------

        public bool StartClient()
        {
            // can only try connection when connection has not been established,
            // and previous attempt has failed
            if ((networkClient != null && networkClient.isConnected) || state != ViveNetworkState.Init)
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Client] Cannot retry connection when client is busy");
                return false;
            }

            networkClient = new NetworkClient();

            RegistorMessagHandlers();

            // registor event callback types
            ViveShare_Event.RegistorEventEntry();

            networkClient.Connect(serverAddr, portNumber);

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Try to connect server (" + serverAddr + ") on port " + portNumber);

            state = ViveNetworkState.Wait_Connect;

            return true;
        }

        public void SetClientReady()
        {
            // can only set client as ready when client is in Wait_Ready state
            if (state != ViveNetworkState.Wait_Ready)
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Client] Cannot set ready when client is not in Wait_Ready state");
                return;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Client is ready");

            // send ready message
            EmptyMessage msg = new EmptyMessage();
            networkClient.Send(MsgType.Ready, msg);

            ViveShare_Event.client_readyEvent.Invoke();

            state = ViveNetworkState.Wait_GameStart;
        }

        //-----------------------------------------------------------------------------
        // networked operation
        //-----------------------------------------------------------------------------

        private void FixedUpdate()
        {
            // timeout detection
            if (state == ViveNetworkState.Wait_TrackerRegistry && IsTimeout(Time.time))
            {
                ViveShare_Log.LogError(logLevel, "[ViveShare_Client] Timeout when waiting tracker registry");

                networkClient.Disconnect();
                DisconnectInternal();
            }

            // messaging
            if (state == ViveNetworkState.Gaming || state == ViveNetworkState.Wait_Shutdown)
            {
                List<ViveShareMessage> msgList = ViveShare_MessageHandler.Instance.GenerateMessagesList();
                for (int i = 0; i < msgList.Count; i++)
                {
                    networkClient.Send(msgList[i].type, msgList[i].body);
                }
            }
        }

        //-----------------------------------------------------------------------------
        // network message handlers
        //-----------------------------------------------------------------------------

        internal void RegistorMessagHandlers()
        {
            networkClient.RegisterHandler(MsgType.Connect, OnConnect);
            networkClient.RegisterHandler(MsgType.Disconnect, OnDisconnect);
            networkClient.RegisterHandler(ViveShareMsgType.TrackerRegistryFinished, OnTrackerRegistryFinished);

            networkClient.RegisterHandler(ViveShareMsgType.GameStarted, OnGameStarted);
            networkClient.RegisterHandler(ViveShareMsgType.GameEnd, OnGameEnd);

            networkClient.RegisterHandler(ViveShareMsgType.SyncParam, OnSyncParam);
            networkClient.RegisterHandler(ViveShareMsgType.Spawn, OnSpawn);
            networkClient.RegisterHandler(ViveShareMsgType.UnSpawn, OnUnSpawn);
            networkClient.RegisterHandler(ViveShareMsgType.Event, OnEvent);
        }

        internal void OnConnect(NetworkMessage netMsg)
        {
            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Client connected to server (" + serverAddr + ") on port " + portNumber);

            // send a request for tracker registry to server (even if not needed)
            ViveTrackerRegistryRequestMsg trackerRequestMsg = new ViveTrackerRegistryRequestMsg();
            trackerRequestMsg.needTrackerPose = needTrackerPose;
            trackerRequestMsg.serialNumber = serialNumber;
            trackerRequestMsg.trackerNumber = trackerNumber;
            networkClient.Send(ViveShareMsgType.TrackerRegistryRequest, trackerRequestMsg);

            messageSendTimeStamp = Time.time;

            state = ViveNetworkState.Wait_TrackerRegistry;
        }

        internal void OnDisconnect(NetworkMessage netMsg)
        {
            DisconnectInternal();
        }

        private void DisconnectInternal()
        {
            // detect connection time out
            if (state == ViveNetworkState.Wait_Connect || state == ViveNetworkState.Wait_TrackerRegistry)
            {
                ViveShare_Log.LogError(logLevel, "[ViveShare_Client] Disconnect due to connection timeout");
                ViveShare_Event.client_connectFailedEvent.Invoke();
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Disconnect");

            // reset and remove resources
            if (needTrackerPose)
            {
                ViveShare_SyncPose syncPose = Camera.main.gameObject.GetComponent<ViveShare_SyncPose>();
                if (syncPose != null)
                {
                    Destroy(syncPose);
                }

                ViveShare_SyncIdentity syncId = Camera.main.gameObject.GetComponent<ViveShare_SyncIdentity>();
                if (syncId != null)
                {
                    Destroy(syncId);
                }
            }

            clientId = -1;

            ViveShare_Event.client_disconnectEvent.Invoke();

            state = ViveNetworkState.Init;
        }

        internal void OnTrackerRegistryFinished(NetworkMessage netMsg)
        {
            // parse message
            ViveTrackerRegistryReturnMsg msg = netMsg.ReadMessage<ViveTrackerRegistryReturnMsg>();

            // assign client id
            clientId = msg.clientId;

            // attach sync pose obj to main camera
            ViveShare_SyncIdentity syncId = Camera.main.gameObject.AddComponent<ViveShare_SyncIdentity>();
            syncId.id = msg.syncObjName;
            syncId.hasAuthority = false;
            syncId.Register();

            ViveShare_SyncPose syncPose = Camera.main.gameObject.AddComponent<ViveShare_SyncPose>();

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Client id (" + clientId + ") assigned");
            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Tracker registry (" + msg.syncObjName + ") finished");

            state = ViveNetworkState.Wait_Ready;

            // invoke callbacks --  we only think the connection is established at this moment
            ViveShare_Event.client_connectSuccessEvent.Invoke();
        }

        internal void OnGameStarted(NetworkMessage netMsg)
        {
            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Game start");

            ViveShare_Event.gameStartEvent.Invoke();

            state = ViveNetworkState.Gaming;
        }

        internal void OnGameEnd(NetworkMessage netMsg)
        {
            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Client] Game end");

            RemoveSpawnedObjects();

            ViveShare_Event.gameEndEvent.Invoke();

            state = ViveNetworkState.Wait_Shutdown;
        }

        internal void OnSyncParam(NetworkMessage netMsg)
        {
            ViveShare_ParamMessage msg = netMsg.ReadMessage<ViveShare_ParamMessage>();
            ViveShare_MessageHandler.Instance.DeliverSyncParamMessage(msg.id, msg.timeStamp, msg.objects);
        }

        public void OnSpawn(NetworkMessage netMsg)
        {
            ViveNetworkSpawnMessage msg = netMsg.ReadMessage<ViveNetworkSpawnMessage>();

            GameObject spawnedObj = Instantiate(Resources.Load(msg.assetName)) as GameObject;
            spawnedObj.name = msg.objectID;
            spawnedObj.transform.position = msg.position;
            spawnedObj.transform.eulerAngles = msg.eulerRotation;

            spawnedObjList.Add(spawnedObj);
        }

        public void OnUnSpawn(NetworkMessage netMsg)
        {
            StringMessage msg = netMsg.ReadMessage<StringMessage>();

            int iter;
            for (iter = 0; iter < spawnedObjList.Count; iter++)
            {
                if (spawnedObjList[iter].name == msg.value)
                {
                    break;
                }
            }

            if (iter < spawnedObjList.Count)
            {
                Destroy(spawnedObjList[iter]);
                spawnedObjList.RemoveAt(iter);
            }
        }

        public void OnEvent(NetworkMessage netMsg)
        {
            ViveShare_ParamMessage msg = netMsg.ReadMessage<ViveShare_ParamMessage>();
            ViveShare_Event.InvokeEventByID(msg.id, msg.objects);
        }

        //-----------------------------------------------------------------------------
        // utilities
        //-----------------------------------------------------------------------------

        public bool IsConnected()
        {
            if (networkClient != null)
            {
                return networkClient.isConnected;
            }

            return false;
        }

        public int GetClientId()
        {
            if (!IsConnected())
            {
                return -1;
            }
            else
            {
                return clientId;
            }
        }

        public void SetTrackerPoseRequestParam(bool need, string sn)
        {
            needTrackerPose = need;
            serialNumber = sn;
        }

        public void SetTrackerPoseRequestParam(bool need, int trackerNum)
        {
            needTrackerPose = need;
            trackerNumber = trackerNum;
        }

        public void RemoveSpawnedObjects()
        {
            for (int i = 0; i < spawnedObjList.Count; i++)
            {
                Destroy(spawnedObjList[i]);
            }
            spawnedObjList.Clear();
        }

        private bool IsTimeout(float time)
        {
            return time - messageSendTimeStamp > timeoutInterval;
        }
    }
}
