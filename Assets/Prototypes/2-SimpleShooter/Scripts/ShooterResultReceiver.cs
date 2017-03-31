//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System;
using UnityEngine;

public class ShooterResultReceiver : MonoBehaviour
{
    [NonSerialized]
    public int playerNum = 0;

    [NonSerialized]
    public int scoreCoutner = 0;

    [NonSerialized]
    public int[] scoreList;

    void Start()
    {
        ViveShare_Event.gameEndEvent.Listen(OnGameStart);

        ViveShare_Event.numberOfPlayerEvent.Listen(OnGettingPlayerNum);
        ViveShare_Event.scoreBroadcastEvent.Listen(OnGettingScore);
    }

    public void OnGameStart()
    {
        playerNum = 0;
        scoreCoutner = 0;
    }

    public void OnGettingPlayerNum(int num)
    {
        playerNum = num;
        scoreList = new int[num];
    }

    public void OnGettingScore(int score)
    {
        scoreList[scoreCoutner] = score;
        scoreCoutner++;
    }
}
