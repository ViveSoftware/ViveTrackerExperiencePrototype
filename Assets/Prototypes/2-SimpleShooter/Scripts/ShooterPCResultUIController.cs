//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.ViveShare;

public class ShooterPCResultUIController : MonoBehaviour
{
    public ShooterPlayerStatusManager statusManager;
    public Text resultText;

    void OnEnable()
    {
        ViveShare_Event.numberOfPlayerEvent.InvokeRemote(statusManager.playerStatusList.Count);
        ViveShare_Event.scoreBroadcastEvent.InvokeRemote(statusManager.playerStatusList[0].score);

        string resultStr = "Host: " + statusManager.playerStatusList[0].score.ToString() + "\n";
        for (int i = 1; i < statusManager.playerStatusList.Count; i++)
        {
            ShooterPlayerStatusManager.PlayerStatus status = statusManager.playerStatusList[i];

            ViveShare_Event.scoreBroadcastEvent.InvokeRemote(status.score);
            resultStr += "P" + i + ": " + status.score.ToString() + "\n";
        }

        resultText.text = resultStr;
    }

    public void OnButtonPressed()
    {
        ViveShare_Server.Instance.ShutdownServer();
    }
}
