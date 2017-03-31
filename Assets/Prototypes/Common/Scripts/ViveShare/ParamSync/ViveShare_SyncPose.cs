//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

namespace HTC.UnityPlugin.ViveShare
{
    public class ViveShare_SyncPose : ViveShare_SyncBase
    {
        // transform from network
        private float currTimeStamp = 0;
        private Vector3 currPosition = Vector3.zero;
        private Vector3 currEulerRotation = Vector3.zero;

        //-----------------------------------------------------------------------------
        // state update
        //-----------------------------------------------------------------------------

        public override void OnEnable()
        {
            base.OnEnable();

            currPosition = transform.position;
            currEulerRotation = transform.eulerAngles;
        }

        public void FixedUpdate()
        {
            // update pose if this object has no authority
            if (identity.hasAuthority == false)
            {
                transform.position = currPosition;
                transform.eulerAngles = currEulerRotation;
            }
        }

        //-----------------------------------------------------------------------------
        // network message
        //-----------------------------------------------------------------------------

        public override object[] GenerateParamList()
        {
            object[] param = { transform.position, transform.eulerAngles };
            return param;
        }

        public override void SetParams(float timeStamp, object[] paramList)
        {
            // record newly received state
            currTimeStamp = timeStamp;
            currPosition = (Vector3)paramList[0];
            currEulerRotation = (Vector3)paramList[1];
        }
    }
}
