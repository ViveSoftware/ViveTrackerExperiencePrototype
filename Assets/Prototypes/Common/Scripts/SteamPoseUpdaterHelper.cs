using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamPoseUpdaterHelper : MonoBehaviour
{
    public GameObject mainCameraObject;

	void Start ()
    {
#if UNITY_5_6
        if (mainCameraObject != null)
        {
            mainCameraObject.AddComponent<SteamVR_UpdatePoses>();
        }
#endif
    }
}
