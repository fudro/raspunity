using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Debug = UnityEngine.Debug;    //System.Diagnostics and UnityEngine both use "Debug" so this directive declares that Debug statements should be treated as UnityEngine.Debug.Log

public class ButtonSend : MonoBehaviour
{
    //public GameObject robot;        //Create a reference for the virtual robot object in the scene
    //[SerializeField]                //Use [Serialize Field] to create editable fields within the editor.
    private byte[] buttonBuffer;    //Declare a byte array for the individual values for each byte. This elements of the array must be assigned here, at Start, or within the editor.
    private Transform panel;        //Create a reference for the attached button's parent panel
    private bool buttonState;       //Create flag to track state of button (press once to activate, press again to deactivate)
    RaspUnityClient client;         //Creat a reference to the communication client script on the robot object
    private int messageLength;      //Total number of bytes for each command
    private Dictionary<string, int> commandList;


    // Start is called before the first frame update
    void Start()
    {
        //Create command ID list
        commandList = new Dictionary<string, int> { { "Arm Gripper",    100 },
                                                    { "Arm Wrist",      101 },
                                                    { "Arm Elbow",      102 },
                                                    { "Arm Shoulder",   103 },
                                                    { "Arm Base",       104 },
                                                    { "Body Gripper",   120 },
                                                    { "Wheels Left",    121 },
                                                    { "Wheels Right",   122 },
                                                    { "IMU",            140 },
                                                    { "Sonar",          141 },
                                                    { "Lidar",          142 } };
        messageLength = 6;          //The number of bytes to send per command
        buttonState = false;        //Initialize with button "not activated"
        //client = robot.GetComponent<RaspUnityClient>();

        panel = this.transform.parent;      //Assign panel
        //getPanelValues();
            
        //Debug.Log(panel);
        //Get panel children by index. Button = 0, Radio Buttons = 1, Slider = 2
        //panel.transform.GetChild(1).gameObject.SetActive(false);
        //panel.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
        //Debug.Log(panel.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.name);
        //Debug.Log(panel.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Toggle>().isOn);
        //Debug.Log(panel.transform.childCount);

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

    public void getPanelValues()
    {
        //Create array buffer for the panel values
        byte[] panelBuffer = new byte[] {  255,
                                           255,
                                           255,
                                           255,
                                           255,
                                           255 };
        /* IMPORTANT:
         * The index for each element is driven by the top to bottom heirarchy of panel children in the Heirarchy Window of the IDE. Topmost child is index zero.
         * The following ordered heirachy is used for each panel: BUTTON, RADIOBUTTON, SLIDER (There can be multiple RadioButton groups. The Slider is always last.)
         * The children for each panel MUST conform to the same generic names for each child type: Button, RadioButton, Slider.
         * The Button label is used as a visual command identifier in the UI, so the button label MUST BE EXACTLY the same as the command name (including spaces, no carriage returns).
         */
        //Update panel values for the associated command. 
        int panelChildren = panel.transform.childCount;
        panelBuffer[0] = (byte)commandList[panel.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text];      //Assign command ID based on name of button
        Debug.Log(panel.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text);
        for (int i = 1; i < panelChildren; i++)     //Start at index 1 to skip the button element at index zero.
        {
            if(panel.transform.GetChild(i).gameObject.name == "RadioButton")        //Check if the element is a radio buttton
            {
                if(panel.transform.GetChild(i).gameObject.transform.GetChild(1).gameObject.GetComponent<Toggle>().isOn) //Check index 1 (first toggle) of radiobutton group (RadioButton label is index 0)
                {
                    panelBuffer[i] = 0; //Set value to zero to indicate that the first option of the toggle group is true
                }
                else if (panel.transform.GetChild(i).gameObject.transform.GetChild(2).gameObject.GetComponent<Toggle>().isOn) //Check index 1 (first toggle) of radiobutton group
                {
                    panelBuffer[i] = 1; //Set value to 1 to indicate that the second option of the toggle group is true
                }
            }
            if(panel.transform.GetChild(i).gameObject.name == "Slider")        //Check if the element is a Slider
            {
                panelBuffer[i] = (byte)panel.transform.GetChild(i).gameObject.GetComponent<Slider>().value;
                Debug.Log("Slider found!");
            }
        }
        Debug.Log(panelBuffer[0]);
        Debug.Log(panelBuffer[1]);
        Debug.Log(panelBuffer[2]);
        Debug.Log(panelBuffer[3]);
        Debug.Log(panelBuffer[4]);
        Debug.Log(panelBuffer[5]);

        //client.s.Write(panelBuffer, 0, panelBuffer.Length);           //"panelBuffer" must be defined for each specific Button using the fields in the editor.
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
