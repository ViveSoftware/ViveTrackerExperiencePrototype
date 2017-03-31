//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using HTC.UnityPlugin.Vive;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_Server : SingletonBehaviour<ViveShare_Server>
    {
        private class ConnectionData
        {
            public ConnectionData(int id)
            {
                connectionId = id;
                isReady = false;
                trackerRole = TrackerRole.Invalid;
                trackerObject = null;
            }

            public int connectionId;
            public bool isReady;
            public TrackerRole trackerRole;
            public GameObject trackerObject;
        }

        // network configuration
        public int portNumber = 2498;
        private int anticipatedClientNum = 0;

        // camera root 
        public GameObject cameraRig;

        // log configuraion
        [SerializeField]
        private LogFilter.FilterLevel logLevel = (LogFilter.FilterLevel)LogFilter.Info;

        // server state
        private ViveNetworkState state = ViveNetworkState.Init;

        // connection data
        private List<ConnectionData> connectionDataList;
        private Dictionary<int, ConnectionData> connectionDataMap;

        // recording spawned objects
        private List<GameObject> spawnedObjList = new List<GameObject>();

        //-----------------------------------------------------------------------------
        // server life cycle
        //-----------------------------------------------------------------------------

        public bool StartServer(int clientNum)
        {
            // can only start server when it is inactive
            if (IsServerActive())
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Server] Server has started already!");
                return false;
            }

            // try to start server on given port number
            if (!NetworkServer.Listen(portNumber))
            {
                ViveShare_Log.LogError(logLevel, "[ViveShare_Server] Failed to start server on port " + portNumber);
                ViveShare_Event.server_initFailedEvent.Invoke();
                return false;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Server started on port " + portNumber + " , anticipated client = " + clientNum);

            // initialize connection data
            anticipatedClientNum = clientNum;
            connectionDataList = new List<ConnectionData>();
            connectionDataMap = new Dictionary<int, ConnectionData>();

            RegisterMessageHandlers();

            // registor event callback types
            ViveShare_Event.RegistorEventEntry();

            ViveShare_Event.server_initSuccessEvent.Invoke();

            state = ViveNetworkState.Wait_Ready;

            return true;
        }

        public void StartGame()
        {
            // can only start game when in Wait_GameStart state
            if (state != ViveNetworkState.Wait_GameStart)
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Server] Cannot start game -- either the game has started or not all clients are ready");
                return;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Game start");

            // tell clients to start game
            EmptyMessage msg = new EmptyMessage();
            NetworkServer.SendToAll(ViveShareMsgType.GameStarted, msg);

            ViveShare_Event.gameStartEvent.Invoke();

            state = ViveNetworkState.Gaming;
        }

        public void EndGame()
        {
            // can only start game when game is running
            if (state != ViveNetworkState.Gaming)
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Server] Cannot stop game if game has not started");
                return;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Game end");

            RemoveSpawnedObjects();

            // tell clients to stop game
            EmptyMessage msg = new EmptyMessage();
            NetworkServer.SendToAll(ViveShareMsgType.GameEnd, msg);

            ViveShare_Event.gameEndEvent.Invoke();

            state = ViveNetworkState.Wait_Shutdown;
        }

        public void ShutdownServer()
        {
            // can't shut down server if it has not started
            if (state == ViveNetworkState.Init)
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Server] Cannot shut down server since it has not been started");
                return;
            }

            // can't shut down server if game is still running
            if (state == ViveNetworkState.Gaming)
            {
                ViveShare_Log.LogWarning(logLevel, "[ViveShare_Server] Cannot shut down server when game is still running");
                return;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Server shut down");

            // clear resources
            anticipatedClientNum = 0;

            foreach (NetworkConnection conn in NetworkServer.connections)
            {
                if (conn != null)
                {
                    RemoveConnectionData(conn.connectionId);
                }
            }

            // tell clients to disconnect
            EmptyMessage msg = new EmptyMessage();
            NetworkServer.SendToAll(MsgType.Disconnect, msg);

            NetworkServer.Shutdown();

            ViveShare_Event.server_shutdownEvent.Invoke();

            state = ViveNetworkState.Init;
        }

        //-----------------------------------------------------------------------------
        // networked operation
        //-----------------------------------------------------------------------------

        private void FixedUpdate()
        {
            if (state == ViveNetworkState.Gaming || state == ViveNetworkState.Wait_Shutdown)
            {
                List<ViveShareMessage> msgList = ViveShare_MessageHandler.Instance.GenerateMessagesList();
                for (int i = 0; i < msgList.Count; i++)
                {
                    NetworkServer.SendToAll(msgList[i].type, msgList[i].body);
                }
            }
        }

        public GameObject Spawn(GameObject asset, string objID, Vector3 position, Quaternion rotation)
        {
            // create message
            ViveShare_MessageHandler.Instance.AddSpawnMessage(asset.name, objID, position, rotation);

            // spawn object for server side
            GameObject spawnedObj = Instantiate(asset, position, rotation) as GameObject;
            spawnedObj.name = objID;
            spawnedObjList.Add(spawnedObj);

            return spawnedObj;
        }

        public void UnSpawn(GameObject obj)
        {
            // only spawned object can be destoryed
            if (!spawnedObjList.Contains(obj))
                return;

            // create message
            ViveShare_MessageHandler.Instance.AddDespawnMessage(obj.name);

            // destroy object for server side
            spawnedObjList.Remove(obj);
            Destroy(obj);
        }

        //-----------------------------------------------------------------------------
        // network message handlers
        //-----------------------------------------------------------------------------

        internal void RegisterMessageHandlers()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnectToServer);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnectFromServer);
            NetworkServer.RegisterHandler(ViveShareMsgType.TrackerRegistryRequest, OnTrakcerResitryRequest);
            NetworkServer.RegisterHandler(MsgType.Ready, OnClientReady);

            NetworkServer.RegisterHandler(ViveShareMsgType.SyncParam, OnSyncParam);
            NetworkServer.RegisterHandler(ViveShareMsgType.Event, OnEvent);
        }

        internal void OnClientConnectToServer(NetworkMessage netMsg)
        {
            // reject client if max number of player connected or game already started
            if (state > ViveNetworkState.Wait_GameStart || GetConnectedPlayerNum() == anticipatedClientNum)
            {
                ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Connection attempt (" + netMsg.conn.connectionId + ") rejected");

                netMsg.conn.Disconnect();
                return;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Client (" + netMsg.conn.connectionId + ") connected");

            // add a connection data container for this connection
            ConnectionData connectionData = new ConnectionData(netMsg.conn.connectionId);
            connectionDataList.Add(connectionData);

            connectionDataMap.Add(netMsg.conn.connectionId, connectionData);
        }

        internal void OnClientDisconnectFromServer(NetworkMessage netMsg)
        {
            // if this disconnection event is due to rejecting illegal connection, do nothing
            if (GetConnectionData(netMsg.conn.connectionId) == null)
            {
                return;
            }

            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Client (" + netMsg.conn.connectionId + ") disconnected");

            RemoveConnectionData(netMsg.conn.connectionId);

            ViveShare_Event.server_clientDisconnectedEvent.Invoke(netMsg.conn.connectionId);

            // if is in Wait_GameStart state, reset state (since someone left the game before game started)
            if (state == ViveNetworkState.Wait_GameStart)
            {
                ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Someone left the game before started, revert to Wait_Ready state");
                state = ViveNetworkState.Wait_Ready;
            }
        }

        internal void OnClientReady(NetworkMessage netMsg)
        {
            ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Client (" + netMsg.conn.connectionId + ") is ready");

            // set ready state for this connection
            ConnectionData connData;
            if (connectionDataMap.TryGetValue(netMsg.conn.connectionId, out connData))
            {
                connData.isReady = true;

                ViveShare_Event.server_clientReadyEvent.Invoke(netMsg.conn.connectionId);
            }

            // check whether all clients are ready
            if (AllClientsReady())
            {
                ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] All clients are ready");

                if (state == ViveNetworkState.Wait_Ready)
                {
                    state = ViveNetworkState.Wait_GameStart;
                }

                ViveShare_Event.server_allClientsReadyEvent.Invoke();
            }
        }

        internal void OnSyncParam(NetworkMessage netMsg)
        {
            ViveShare_ParamMessage msg = netMsg.ReadMessage<ViveShare_ParamMessage>();
            ViveShare_MessageHandler.Instance.DeliverSyncParamMessage(msg.id, msg.timeStamp, msg.objects);

            // broadcast message to other clients
            NetworkServer.SendToAll(ViveShareMsgType.SyncParam, msg);
        }

        public void OnEvent(NetworkMessage netMsg)
        {
            ViveShare_ParamMessage msg = netMsg.ReadMessage<ViveShare_ParamMessage>();
            ViveShare_Event.InvokeEventByID(msg.id, msg.objects);
        }

        //-----------------------------------------------------------------------------
        // tracker assignment
        //-----------------------------------------------------------------------------

        internal void OnTrakcerResitryRequest(NetworkMessage netMsg)
        {
            // parse message
            ViveTrackerRegistryRequestMsg msg = netMsg.ReadMessage<ViveTrackerRegistryRequestMsg>();

            // check connectivity
            int connId = netMsg.conn.connectionId;
            ConnectionData connData = GetConnectionData(connId);
            if (connData == null)
            {
                return;
            }

            // if need to assign a tracker to client
            string trackerPoseObjName = "";
            if (msg.needTrackerPose)
            {
                TrackerRole trackerRole = TryToBindTracker(msg.serialNumber, msg.trackerNumber);

                // if failed, tell client to disconnect
                if (trackerRole == TrackerRole.Invalid)
                {
                    ViveShare_Log.LogError(logLevel, "[ViveShare_Server] Failed to assign tracker for client (" + netMsg.conn.connectionId + ")");
                    netMsg.conn.Disconnect();
                    return;
                }
                // if succeeded, save assigned role and initialize sync pose object
                else
                {
                    ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] Assign tracker (" + trackerRole.ToString() + ") to client (" + netMsg.conn.connectionId + ")");

                    connData.trackerRole = trackerRole;
                    trackerPoseObjName = trackerRole.ToString();

                    // create pose tracker and attach sync param object to it
                    GameObject trackerObj = new GameObject(trackerPoseObjName);
                    VivePoseTracker poseTracker = trackerObj.AddComponent<VivePoseTracker>();
                    poseTracker.viveRole.SetEx<TrackerRole>(trackerRole);

                    ViveShare_SyncIdentity syncId = trackerObj.AddComponent<ViveShare_SyncIdentity>();
                    syncId.id = trackerPoseObjName;
                    syncId.hasAuthority = true;
                    syncId.Register();

                    trackerObj.AddComponent<ViveShare_SyncPose>();

                    if (cameraRig != null)
                    {
                        trackerObj.transform.parent = cameraRig.transform;
                    }

                    connData.trackerObject = trackerObj;
                }
            }

            // return "finished" message
            ViveTrackerRegistryReturnMsg returnMsg = new ViveTrackerRegistryReturnMsg();
            returnMsg.syncObjName = trackerPoseObjName;
            returnMsg.clientId = connId;

            NetworkServer.SendToClient(netMsg.conn.connectionId, ViveShareMsgType.TrackerRegistryFinished, returnMsg);

            // invoke callbacks -- we only think the connection is established at this moment
            ViveShare_Event.server_clientConnectedEvent.Invoke(netMsg.conn.connectionId);
        }

        TrackerRole TryToBindTracker(string serialNumber, int trackerNumber)
        {
            TrackerRole role = TrackerRole.Invalid;

            // try to bind tracker from serial number
            if (!String.IsNullOrEmpty(serialNumber))
            {
                role = ViveRole.GetMap<TrackerRole>().GetBoundRoleByDevice(serialNumber);
            }

            // if failed, try to bind tracker from tracker number
            if (trackerNumber > 0 && trackerNumber < Enum.GetNames(typeof(TrackerRole)).Length)
            {
                role = (TrackerRole)trackerNumber;
            }

            // check repeted role
            if (role != TrackerRole.Invalid)
            {
                for (int i = 0; i < connectionDataList.Count; i++)
                {
                    if (connectionDataList[i].trackerRole == role)
                    {
                        role = TrackerRole.Invalid;
                        break;
                    }
                }
            }

            return role;
        }

        //-----------------------------------------------------------------------------
        // utilities
        //-----------------------------------------------------------------------------

        public bool IsServerActive()
        {
            return NetworkServer.active;
        }

        public int GetAnticiplatedPlayerNum()
        {
            return anticipatedClientNum;
        }

        public int GetConnectedPlayerNum()
        {
            return connectionDataList.Count;
        }

        public bool IsPlayerReady(int id)
        {
            bool result = false;

            ConnectionData data;
            if (connectionDataMap.TryGetValue(id, out data))
            {
                result = data.isReady;
            }

            return result;
        }

        public TrackerRole GetPlayerTrackerRole(int id)
        {
            TrackerRole result = TrackerRole.Invalid;

            ConnectionData data;
            if (connectionDataMap.TryGetValue(id, out data))
            {
                result = data.trackerRole;
            }

            return result;
        }

        public void RemoveSpawnedObjects()
        {
            for (int i = 0; i < spawnedObjList.Count; i++)
            {
                Destroy(spawnedObjList[i]);
            }
            spawnedObjList.Clear();
        }

        private ConnectionData GetConnectionData(int id)
        {
            ConnectionData data = null;
            connectionDataMap.TryGetValue(id, out data);
            return data;
        }

        private void RemoveConnectionData(int id)
        {
            ConnectionData data;
            if (connectionDataMap.TryGetValue(id, out data))
            {
                ViveShare_Log.LogInfo(logLevel, "[ViveShare_Server] RemoveConnectionData for client (" + id + ")");

                // destroy tracker object
                if (data.trackerObject != null)
                {
                    ViveShare_SyncIdentity syncId = data.trackerObject.GetComponent<ViveShare_SyncIdentity>();
                    syncId.UnRegister();

                    Destroy(data.trackerObject);
                }

                // destroy data
                connectionDataList.Remove(data);
                connectionDataMap.Remove(id);
            }
        }

        private bool AllClientsReady()
        {
            if (connectionDataList.Count < anticipatedClientNum)
                return false;

            bool allReady = true;
            for (int i = 0; i < connectionDataList.Count; i++)
            {
                if (connectionDataList[i].isReady == false)
                {
                    allReady = false;
                    break;
                }
            }

            return allReady;
        }
    }
}
