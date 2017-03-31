//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Net;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.ViveShare;

public class MobileLobbyUIController : MonoBehaviour
{
    // ui elements
    public InputField ipInput;
    public InputField portInput;
    public Button gameSettingButton;
    public Text gameSettingButtonText;

    // network config
    public string ip = "10.116.81.66";
    public int port = 2498;
    public int playerNum = 1;

    private ViveNetworkState state = ViveNetworkState.Init;

    void OnEnable()
    {
        ipInput.text = ip;
        portInput.text = port.ToString();

        ViveShare_Event.client_connectFailedEvent.Listen(OnClientConnectFailed);
        ViveShare_Event.client_connectSuccessEvent.Listen(OnClientConnected);

        gameSettingButtonText.text = "Connect to Host";
        state = ViveNetworkState.Init;
    }

    private void OnDisable()
    {
        ViveShare_Event.client_connectFailedEvent.Remove(OnClientConnectFailed);
        ViveShare_Event.client_connectSuccessEvent.Remove(OnClientConnected);
    }

    public void OnIpInputFinished()
    {
        IPAddress address;
        if (IPAddress.TryParse(ipInput.text, out address))
        {
            ip = address.ToString();
        }
    }

    public void OnPortInputFinished()
    {
        int parseResult;
        if (int.TryParse(portInput.text, out parseResult))
        {
            port = parseResult;
        }
    }

    public void OnSelectPlayerId(int val)
    {
        playerNum = val + 1;
    }

    public void OnGameSettingButtonPressed()
    {
        if (state == ViveNetworkState.Init)
        {
            ViveShare_Client.Instance.serverAddr = ip;
            ViveShare_Client.Instance.portNumber = port;
            ViveShare_Client.Instance.SetTrackerPoseRequestParam(true, playerNum);

            if (ViveShare_Client.Instance.StartClient() == true)
            {
                gameSettingButtonText.text = "Connecting...";
                state = ViveNetworkState.Wait_Connect;
            }
        }
    }

    public void OnClientConnectFailed()
    {
        gameSettingButtonText.text = "Connect to Host";

        state = ViveNetworkState.Init;
    }

    public void OnClientConnected()
    {
        ViveShare_Client.Instance.SetClientReady();
        gameSettingButtonText.text = "Preparing Game";
        state = ViveNetworkState.Wait_GameStart;
    }
}
