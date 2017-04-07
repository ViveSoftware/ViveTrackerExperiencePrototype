//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_SyncPose : ViveShare_SyncBase
    {
        private class State
        {
            public float timeStamp;
            public Vector3 position;
            public Quaternion rotation;
        }

        private float lastSyncTime = 0;
        public bool doInterpolate = true;
        private float interpolateStep = 0.0f;

        private List<State> states = new List<State>();

        //-----------------------------------------------------------------------------
        // state update
        //-----------------------------------------------------------------------------

        public void FixedUpdate()
        {
            // return if this object has authority
            if (identity.hasAuthority)
                return;

            if (doInterpolate && states.Count >= 2)
            {
                transform.position = Vector3.Lerp(states[0].position, states[1].position, interpolateStep);
                transform.rotation = Quaternion.Slerp(states[0].rotation, states[1].rotation, interpolateStep);

                interpolateStep += sendInterval / Time.fixedDeltaTime;
                if(interpolateStep >= 1)
                {
                    interpolateStep = 0;
                    states.RemoveAt(0);
                }
            }
        }

        //-----------------------------------------------------------------------------
        // network message
        //-----------------------------------------------------------------------------

        public override object[] GenerateParamList()
        {
            object[] param = { transform.position, transform.rotation };
            return param;
        }

        public override void SetParams(float timeStamp, object[] paramList)
        {
            // ignore out-of-order packets
            if (timeStamp <= lastSyncTime)
            {
                return;
            }

            Vector3 pos = (Vector3)paramList[0];
            Quaternion rot = (Quaternion)paramList[1];

            if(!doInterpolate)
            {
                transform.position = pos;
                transform.rotation = rot;
            }
            else
            {
                State newState = new State();
                newState.timeStamp = timeStamp;
                newState.position = pos;
                newState.rotation = rot;

                states.Add(newState);
            }

            lastSyncTime = timeStamp;
        }
    }
}
