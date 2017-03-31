//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.ViveShare;

public class PCLobbyUIController : MonoBehaviour
{
    public Dropdown playerNumDropdown;
    public Button gameSettingButton;
    public Text gameSettingButtonText;

    public Color buttonNormalColor;
    public Color buttonStartGameColor;

    private int playerNum = 1;
    private ViveNetworkState state = ViveNetworkState.Init;

    public void OnEnable()
    {
        ViveShare_Event.server_initSuccessEvent.Listen(OnServerStarted);
        ViveShare_Event.server_allClientsReadyEvent.Listen(OnClientsReady);
        ViveShare_Event.server_clientDisconnectedEvent.Listen(OnClientDisconnect);

        state = ViveNetworkState.Init;

        gameSettingButton.interactable = true;

        var colors = gameSettingButton.colors;
        colors.normalColor = buttonNormalColor;
        colors.highlightedColor = buttonNormalColor;
        gameSettingButton.colors = colors;

        gameSettingButtonText.text = "Start as Host";
    }

    public void OnDisable()
    {
        ViveShare_Event.server_initSuccessEvent.Remove(OnServerStarted);
        ViveShare_Event.server_allClientsReadyEvent.Remove(OnClientsReady);
        ViveShare_Event.server_clientDisconnectedEvent.Remove(OnClientDisconnect);
    }

    public void OnPlayerNumberChanaged(int optionIdx)
    {
        playerNum = optionIdx + 1;
    }

    public void OnGameSettingButtonPressed()
    {
        if (state == ViveNetworkState.Init)
        {
            ViveShare_Server.Instance.StartServer(playerNum);
        }
        else if (state == ViveNetworkState.Wait_GameStart)
        {
            ViveShare_Server.Instance.StartGame();
        }
    }

    public void OnServerStarted()
    {
        //// ++ Unity bug -- GameObject mysteriously destoryed when reload scene
        //gameSettingButton = FindObjectOfType<Button>();
        //gameSettingButtonText = gameSettingButton.GetComponentInChildren<Text>();
        //// -- Unity bug

        gameSettingButton.interactable = false;
        gameSettingButtonText.text = "Wait Clients...";

        state = ViveNetworkState.Wait_Ready;
    }

    public void OnClientsReady()
    {
        gameSettingButton.interactable = true;

        var colors = gameSettingButton.colors;
        colors.normalColor = buttonStartGameColor;
        colors.highlightedColor = buttonStartGameColor;
        gameSettingButton.colors = colors;

        gameSettingButtonText.text = "Start Game";

        state = ViveNetworkState.Wait_GameStart;
    }

    public void OnClientDisconnect(int id)
    {
        gameSettingButton.interactable = false;
        gameSettingButtonText.text = "Wait Clients...";

        state = ViveNetworkState.Wait_Ready;
    }
}
