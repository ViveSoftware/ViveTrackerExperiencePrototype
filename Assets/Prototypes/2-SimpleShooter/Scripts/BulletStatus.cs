//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using HTC.UnityPlugin.ViveShare;

public class BulletStatus : MonoBehaviour
{
    private int source = 0;

    private float startTime;
    private static float lifeTime = 2.0f;

    private ShooterPlayerStatusManager playerStatusMgr;
    private Collider[] wallCollider;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // if lifetime ended, despawn this bullet
        if (Time.time - startTime > BulletStatus.lifeTime)
        {
            ViveShare_Server.Instance.UnSpawn(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ShooterPlayerAvatar>() != null)
        {
            playerStatusMgr.AddBulletHitScore(source);
            ViveShare_Server.Instance.UnSpawn(gameObject);
            return;
        }

        foreach (Collider collider in wallCollider)
        {
            if(collider == other)
            {
                ViveShare_Server.Instance.UnSpawn(gameObject);
                return;
            }
        }
    }

    public void SetSource(int val)
    {
        source = val;
    }

    public void SetPlayerStatusManager(ShooterPlayerStatusManager mgr)
    {
        playerStatusMgr = mgr;
    }

    public void SetWallCollider(Collider[] collider)
    {
        wallCollider = collider;
    }
}
