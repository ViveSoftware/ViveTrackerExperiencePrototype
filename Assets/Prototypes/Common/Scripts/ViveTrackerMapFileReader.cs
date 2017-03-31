//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using HTC.UnityPlugin.Vive;

public class ViveTrackerMapFileReader : MonoBehaviour
{
    public string configPath;

    public void Start()
    {
        ViveRoleBindingsHelper.LoadRoleBindings(configPath);
    }
}
