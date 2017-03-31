//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.ViveShare;
using UnityEngine.Events;

public class ShooterMobileResultUIController : MonoBehaviour
{
    public Text resultText;
    public ShooterResultReceiver resultReceiver;

    public void Update()
    {
        if (resultReceiver.playerNum > 0 && resultReceiver.scoreCoutner == resultReceiver.playerNum)
        {
            string resultStr = "Host: " + resultReceiver.scoreList[0].ToString() + "\n";
            for (int i = 1; i < resultReceiver.playerNum; i++)
            {
                resultStr += "P" + i + ": " + resultReceiver.scoreList[i].ToString() + "\n";
            }

            resultText.text = resultStr;
        }
    }
}
