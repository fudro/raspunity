using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;    //System.Diagnostics and UnityEngine both use "Debug" so this directive declares that Debug statements should be treated as UnityEngine.Debug.Log

public class ButtonSend : MonoBehaviour
{
    public GameObject robot;
    [SerializeField]                //Use [Serialize Field] to create editable fields within the editor.
    //private string command;       //Declare a string variable if the field accepts bytes as a string
    private byte[] buttonBuffer;    //Declare a byte array if the field accepts individual values for each byte. This elements of the array must be assigned here, at Start, or within the editor.
    private bool buttonState;       //Create flag to track state of button (press once to activate, press again to deactivate)
    RaspUnityClient client;         //Creat a reference to the communication client script on the robot object


    // Start is called before the first frame update
    void Start()
    {
        buttonState = false;        //Initialize with button "not activated"
        client = robot.GetComponent<RaspUnityClient>();

        //command = "127, 0, 0, 0, 0, 0";                       //used to send data as a string
        //buttonBuffer = new byte[] { 10, 0, 0, 69, 0, 0 };     //Used to send data as an array of bytes. Can also be defined in the sending function.
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sendMessage()
    {
        //byte[] buttonBuffer = new byte[] { 10, 0, 0, 69, 0, 0 };        //Use this format to send raw bytes
        //byte[] buttonBuffer = Encoding.UTF8.GetBytes(command);          //Use this format to encode message as string
        client.s.Write(buttonBuffer, 0, buttonBuffer.Length);           //"buttonBuffer" must be defined for each specific Button using the fields in the editor.
    }

    public void toggleGStreamer()       //A specific Button is hardcoded in Unity editor to send command to start gStreamer
    {
        if (buttonState == false)
        {
            client.s.Write(buttonBuffer, 0, buttonBuffer.Length);       //Send command to Raspberry Pi.
            System.Diagnostics.Process.Start("C:/Users/fudro/Desktop/rpi_gstream.bat");     //Run a batch file to start gStreamer from Unity. MUST match the actual location of the batch file.

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
