//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.ViveShare;
using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    public ShooterPlayerStatusManager playerManager;

    public GameObject bulletPrefab;

    public float coolTime = 0.25f;
    public float bulletSpeed = 4.0f;
    public float fireOffset = 0.4f;

    public Collider[] wallCollider;

    private float lastFireTime;

    private void OnEnable()
    {
        ViveShare_Event.bulletSpawnEvent.Listen(Spawn);
    }

    private void OnDisable()
    {
        ViveShare_Event.bulletSpawnEvent.Remove(Spawn);
    }

    private void Update()
    {
        if (ViveInput.GetPressDownEx<HandRole>(HandRole.RightHand, ControllerButton.Trigger))
        {
            if (Time.time - lastFireTime < coolTime)
            {
                return;
            }

            Pose pose = VivePose.GetPose(HandRole.RightHand);
            Vector3 position = pose.pos;
            Vector3 forward = pose.rot * Vector3.forward;
            Spawn(position, forward, 0);

            lastFireTime = Time.time;
        }
    }

    void Spawn(Vector3 pt, Vector3 dir, int sourcePlayer)
    {
        // offset bullet a little to avoid colliding with camera
        Vector3 offsetPos = pt + dir.normalized * fireOffset;

        // calculate bullet velocity
        Vector3 bullectVelocity = bulletSpeed * dir.normalized;

        // calculate prefab rotation
        Vector3 rotationAxis = Vector3.Cross(Vector3.forward, dir);
        float rotationAngle = Vector3.Angle(Vector3.forward, dir);
        Quaternion quat = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        // spawn the bullet and set its color
        int counter = playerManager.playerStatusDic[sourcePlayer].bulletCounter++;
        string objID = "[" + sourcePlayer + "] Bullet " + counter++;
        GameObject spawnedObj = ViveShare_Server.Instance.Spawn(bulletPrefab, objID, offsetPos, quat);

        spawnedObj.GetComponent<Renderer>().material.SetColor("_Color", ShooterPlayerColorTable.colorTable[sourcePlayer]);
        spawnedObj.GetComponent<Renderer>().material.SetColor("_EmissionColor", ShooterPlayerColorTable.colorTable[sourcePlayer]);

        // set bullet param for clients
        ViveShare_Event.setBulletParamEvent.InvokeRemote(objID, bullectVelocity, ShooterPlayerColorTable.colorTable[sourcePlayer]);

        // attach bullet controller to spawned bullet
        BulletStatus status = spawnedObj.AddComponent<BulletStatus>();
        status.SetSource(sourcePlayer);
        status.SetPlayerStatusManager(playerManager);
        status.SetWallCollider(wallCollider);

        // set bullet property
        Rigidbody physics = spawnedObj.GetComponent<Rigidbody>();
        physics.velocity = bullectVelocity;
        physics.AddForce(bullectVelocity, ForceMode.Force);
    }
}
