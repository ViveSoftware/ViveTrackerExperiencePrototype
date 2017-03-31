//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using HTC.UnityPlugin.ViveShare;

public class MobileBulletSpawn : MonoBehaviour
{
    public float coolTime = 0.25f;

    private float lastTapTime;

    private void OnEnable()
    {
        ViveShare_Event.setBulletParamEvent.Listen(OnSetBulletParam);
    }

    private void OnDisable()
    {
        ViveShare_Event.setBulletParamEvent.Remove(OnSetBulletParam);
    }

    public void OnBulletSpawnBtnPressed()
    {
        if (Time.time - lastTapTime < coolTime)
        {
            return;
        }

        ViveShare_Event.bulletSpawnEvent.InvokeRemote(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            ViveShare_Client.Instance.GetClientId());

        lastTapTime = Time.time;
    }

    private void OnSetBulletParam(string id, Vector3 vel, Color color)
    {
        GameObject go = GameObject.Find(id);
        if (go == null) { return; }

        go.GetComponent<Renderer>().material.SetColor("_Color", color);
        go.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

        Rigidbody rigidBody = go.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.velocity = vel;
            rigidBody.AddForce(vel, ForceMode.Force);
        }
    }
}
