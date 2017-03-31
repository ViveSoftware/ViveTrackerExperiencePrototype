//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

public static partial class ViveShare_Event
{
    public static ViveEvent server_initSuccessEvent = new ViveEvent("VIVE_SERVER_INIT_SUCCESS");
    public static ViveEvent server_initFailedEvent = new ViveEvent("VIVE_SERVER_INIT_FAILED");
    public static ViveEvent<int> server_clientConnectedEvent = new ViveEvent<int>("VIVE_SERVER_CLIENT_CONNECCTED");
    public static ViveEvent<int> server_clientDisconnectedEvent = new ViveEvent<int>("VIVE_SERVER_CLIENT_DISCONNECCTED");
    public static ViveEvent<int> server_clientReadyEvent = new ViveEvent<int>("VIVE_SERVER_CLIENT_READY");
    public static ViveEvent server_allClientsReadyEvent = new ViveEvent("VIVE_SERVER_ALL_CLIENT_READY");
    public static ViveEvent server_shutdownEvent = new ViveEvent("VIVE_SERVER_SHUTDOWN");

    public static ViveEvent client_connectFailedEvent = new ViveEvent("VIVE_CLIENT_CONNECT_FAILED");
    public static ViveEvent client_connectSuccessEvent = new ViveEvent("VIVE_CLIENT_CONNECT_SUCCESS");
    public static ViveEvent client_disconnectEvent = new ViveEvent("VIVE_CLIENT_DISCONNECT");
    public static ViveEvent client_readyEvent = new ViveEvent("VIVE_CLIENT_READY");

    public static ViveEvent gameStartEvent = new ViveEvent("VIVE_GAME_START");
    public static ViveEvent gameEndEvent = new ViveEvent("VIVE_GAME_END");
}
