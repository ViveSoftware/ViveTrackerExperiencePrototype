//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

public class GameProgressController : MonoBehaviour
{
    public GameObject[] lobbyGameObjects;
    public GameObject[] gameplayGameObjects;
    public GameObject[] resultGameObjects;

    private bool isInResultScene = false;

    void Start ()
    {
        // set event liseners
        ViveShare_Event.gameStartEvent.Listen(OnGameStart);
        ViveShare_Event.gameEndEvent.Listen(OnGameEnd);
        ViveShare_Event.client_disconnectEvent.Listen(OnClientDisconnect);
        ViveShare_Event.server_shutdownEvent.Listen(GoBackToLobby);
    }

    public void OnGameStart()
    {
        foreach (GameObject obj in lobbyGameObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in gameplayGameObjects)
        {
            obj.SetActive(true);
        }
    }

    public void OnGameEnd()
    {
        foreach (GameObject obj in gameplayGameObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in resultGameObjects)
        {
            obj.SetActive(true);
        }

        isInResultScene = true;
    }

    private void OnClientDisconnect()
    {
        if (!isInResultScene)
        {
            foreach (GameObject obj in gameplayGameObjects)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in lobbyGameObjects)
            {
                obj.SetActive(true);
            }
        }
    }

    public void GoBackToLobby()
    {
        foreach (GameObject obj in resultGameObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in lobbyGameObjects)
        {
            obj.SetActive(true);
        }
    }
}
