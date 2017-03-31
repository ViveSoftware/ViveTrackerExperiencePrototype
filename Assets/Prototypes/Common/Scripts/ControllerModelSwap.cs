//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

public class ControllerModelSwap : MonoBehaviour
{
    public GameObject leftHandRectile;
    public GameObject leftHandGuideRay;
    public GameObject rightHandRectile;
    public GameObject rightHandGuideRay;

    void Start ()
    {
        ViveShare_Event.gameStartEvent.Listen(OnGameStart);
        ViveShare_Event.gameEndEvent.Listen(OnGameEnd);
    }

    public void OnGameStart()
    {
        leftHandRectile.SetActive(false);
        leftHandGuideRay.SetActive(false);

        rightHandRectile.SetActive(false);
        rightHandGuideRay.SetActive(false);
    }

    public void OnGameEnd()
    {
        leftHandRectile.SetActive(true);
        leftHandGuideRay.SetActive(true);

        rightHandRectile.SetActive(true);
        rightHandGuideRay.SetActive(true);
    }
}
