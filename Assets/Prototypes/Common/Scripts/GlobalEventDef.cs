//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

public static partial class ViveShare_Event
{
    // event definition
    public static ViveEvent<string, Vector3, Color> setBulletParamEvent = new ViveEvent<string, Vector3, Color>("SET_BULLET_PARAM");
    public static ViveEvent<Vector3, Vector3, int> bulletSpawnEvent = new ViveEvent<Vector3, Vector3, int>("BULLET_SPAWN");

    public static ViveEvent<int> numberOfPlayerEvent = new ViveEvent<int>("NUMBER_OF_PLAERS");
    public static ViveEvent<int> scoreBroadcastEvent = new ViveEvent<int>("SCORE_BROADCAST");
}
