using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;    //System.Diagnostics and UnityEngine both use "Debug" so this directive declares that Debug statements should be treated as UnityEngine.Debug.Log

public class ButtonSend : MonoBehaviour
{
    public GameObject cube;
    [SerializeField]
    //private string command;       //Declare string variable if the field accepts bytes as a string
    private byte[] buttonBuffer;    //Serialize field in editor for a 6 element byte array
    private bool buttonState;       //Create flag to track state of button (press once to activate, press again to deactivate)
    RaspUnityClient client;


    // Start is called before the first frame update
    void Start()
    {
        buttonState = false;
        client = cube.GetComponent<RaspUnityClient>();

        //command = "127, 0, 0, 0, 0, 0";
        //buttonBuffer = new byte[] { 10, 0, 0, 69, 0, 0 };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sendMessage()
    {
        //byte[] buttonBuffer = new byte[] { 10, 0, 0, 69, 0, 0 };        //Use this format to send raw bytes
        //byte[] buttonBuffer = Encoding.UTF8.GetBytes(command);          //Use this format to encode message as string
        client.s.Write(buttonBuffer, 0, buttonBuffer.Length);
    }

    public void toggleGStreamer()
    {
        if (buttonState == false)
        {
            client.s.Write(buttonBuffer, 0, buttonBuffer.Length);       //Send command to Raspberry Pi. Button is hardcoded in Unity editor to send command to start gStreamer
            System.Diagnostics.Process.Start("C:/Users/fudro/Desktop/rpi_gstream.bat");     //Run a batch file to start gStreamer from Unity

            buttonState = true;
            Debug.Log("button ON!");
        }
        else if(buttonState == true)
        {
            buttonState = false;
            Debug.Log("button OFF!");
        }
    }
}
