//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.ViveShare;

public class ShooterPlayerStatusManager : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject playerAvatarPrefab;

    public int hitPlayerScore = 10;

    public class PlayerStatus
    {
        public PlayerStatus()
        {
            avatar = null;
            bulletCounter = 0;
            score = 0;
        }

        public GameObject avatar;
        public int bulletCounter;
        public int score;
    }
    public List<PlayerStatus> playerStatusList = new List<PlayerStatus>();
    public Dictionary<int, PlayerStatus> playerStatusDic = new Dictionary<int, PlayerStatus>();

    void Start()
    {
        ViveShare_Event.server_initSuccessEvent.Listen(OnServerStarted);
        ViveShare_Event.server_clientConnectedEvent.Listen(OnClientConnect);
        ViveShare_Event.server_clientDisconnectedEvent.Listen(OnClientDisconnect);
        ViveShare_Event.gameEndEvent.Listen(OnGameEnd);
    }

    public void OnServerStarted()
    {
        int arraySize = ViveShare_Server.Instance.GetAnticiplatedPlayerNum() + 1;

        // remove old player status
        playerStatusList.Clear();
        playerStatusDic.Clear();

        // create player status for host
        PlayerStatus hostStatus = new PlayerStatus();

        playerStatusList.Add(hostStatus);
        playerStatusDic.Add(0, hostStatus);
    }

    public void OnClientConnect(int id)
    {
        PlayerStatus playerStatus = new PlayerStatus();

        if (playerAvatarPrefab != null)
        {
            GameObject playerObj = Instantiate(playerAvatarPrefab);
            playerObj.name = "Player " + id + " Avatar";

            ShooterPlayerAvatar avatar = playerObj.GetComponent<ShooterPlayerAvatar>();
            avatar.id = id;

            int colorIdx = id % ShooterPlayerColorTable.colorTable.Length;
            playerObj.GetComponentInChildren<Renderer>().material.color = ShooterPlayerColorTable.colorTable[colorIdx];

            VivePoseTracker poseTracker = playerObj.GetComponent<VivePoseTracker>();
            poseTracker.viveRole.SetEx<TrackerRole>(ViveShare_Server.Instance.GetPlayerTrackerRole(id));

            playerStatus.avatar = playerObj;
            playerStatus.avatar.transform.parent = cameraRig.transform;
        }

        PlayerStatus oldPlayerStatus;
        if (playerStatusDic.TryGetValue(id, out oldPlayerStatus) == true)
        {
            playerStatusList.Remove(oldPlayerStatus);
            playerStatusDic.Remove(id);
        }

        playerStatusList.Add(playerStatus);
        playerStatusDic.Add(id, playerStatus);
    }

    public void OnClientDisconnect(int id)
    {
        PlayerStatus status;
        if (playerStatusDic.TryGetValue(id, out status) == true)
        {
            Destroy(status.avatar);
        }
    }

    public void OnGameEnd()
    {
        for (int i = 0; i < playerStatusList.Count; i++)
        {
            Destroy(playerStatusList[i].avatar);
        }
    }

    public void AddBulletHitScore(int playerId)
    {
        PlayerStatus status;
        if (playerStatusDic.TryGetValue(playerId, out status) == true)
        {
            status.score += hitPlayerScore;
        }
    }
}
