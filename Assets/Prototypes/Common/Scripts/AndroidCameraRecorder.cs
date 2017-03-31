//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.UI;

public class AndroidCameraRecorder : MonoBehaviour
{
    // android camera stream param
    public int width = 1920;
    public int height = 1080;
    public int fps = 30;

    // camera device and data
    private WebCamTexture webcamTexture;

    // camera texture renderer
    public RawImage backgroundImage;

    private void Start()
    {
        // try to get webcam devices and create webcam texture from the device
        WebCamDevice[] webCamDevices = WebCamTexture.devices;
        if (webCamDevices.Length > 0)
        {
            // request camera stream
            webcamTexture = new WebCamTexture(webCamDevices[0].name, width, height, fps);
            webcamTexture.Play();
        }
    }

    private void Update()
    {
        if (backgroundImage != null && backgroundImage.gameObject.activeSelf)
        {
            backgroundImage.texture = webcamTexture;
        }
    }
}
