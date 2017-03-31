//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.Networking;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShareMsgType
    {
        public static short TrackerRegistryRequest = MsgType.Highest + 1;
        public static short TrackerRegistryFinished = MsgType.Highest + 2;
        public static short GameStarted = MsgType.Highest + 3;
        public static short GameEnd = MsgType.Highest + 4;

        public static short SyncParam = MsgType.Highest + 7;
        public static short Spawn = MsgType.Highest + 8;
        public static short UnSpawn = MsgType.Highest + 9;
        public static short Event = MsgType.Highest + 10;
    };

    public class ViveShareMessage
    {
        public short type;
        public MessageBase body;
    }

    public class ViveTrackerRegistryRequestMsg : MessageBase
    {
        public bool needTrackerPose;
        public string serialNumber;
        public int trackerNumber;
    }

    public class ViveTrackerRegistryReturnMsg : MessageBase
    {
        public string syncObjName;
        public int clientId;
    }

    public class ViveNetworkSpawnMessage : MessageBase
    {
        public string assetName;
        public string objectID;
        public Vector3 position;
        public Vector3 eulerRotation;
    }
}
