//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.ViveShare;

public class PlayTimeHandler : MonoBehaviour
{
    public float timeLimit = 10.0f;
    private float gameStartTime = 0.0f;
    private float timeEndPoint = 0.0f;

    public Text visText;

    public bool hasAuthority = false;

    void OnEnable()
    {
        gameStartTime = Time.time;
        timeEndPoint = gameStartTime + timeLimit;
    }

    void FixedUpdate()
    {
        if (Time.time <= timeEndPoint)
        {
            int min = Mathf.RoundToInt(timeEndPoint - Time.time) / 60;
            int sec = Mathf.RoundToInt(timeEndPoint - Time.time) % 60;

            string minString = min.ToString().PadLeft(2, '0');
            string secString = sec.ToString().PadLeft(2, '0');

            visText.text = minString + ":" + secString;
        }
        else
        {
            visText.text = "00:00";

            if(hasAuthority)
            {
                ViveShare_Server.Instance.EndGame();
            }
        }
    }
}
