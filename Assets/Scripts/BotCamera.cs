using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotCamera : MonoBehaviour
{
    static WebCamTexture cam;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (var i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);
        
        if (cam == null)
        {
            cam = new WebCamTexture();
        }
        GetComponent<Renderer>().material.mainTexture = cam;

        if(!cam.isPlaying)
        {
            cam.Play();
        }
        
        
        WebCamTexture webcamTexture = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
