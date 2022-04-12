using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;                  //Allows for text encoding
using UnityEngine;
using System.Threading;
using System;


//Utility function to create subarrays from larger byte arrays
public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }
}


public class RaspUnityClient : MonoBehaviour {
    Thread mThread;
    TcpClient client;
    public NetworkStream s; 
    public string   host = "192.168.1.4";           //Enter ip address of a Raspberry Pi. IMPORTANT: Raspberry Pi must be on the same network as the Unity device (PC, Mobile)
    public int      port = 9999;                    //Create a port number
    public float    speed = 10;                     //Set how fast the Unity gameobject moves in the scene
    public float    divBy = 255;                    //Normalize gameobject position vector by dividing each axis value by 255 to get normalized value between 0-1.
    Vector3         pos, oldPos = Vector3.zero;     //Store decoded position data values received over wifi. Position vector has three 8-bit (one byte) values from 0-255 for each axis x, y, z.

    void Start() {
        ThreadStart ts = new ThreadStart(Client);       //Initialize ThreadStart and assign function to be run on the thread
        mThread = new Thread(ts);                       //Initialize thread with the created ThreadStart object
        mThread.Start();                                //Start thread
    }

    void Client() {     //Create fuction to be run on the thread
        try {
            client = new TcpClient(host, port);         //Initialize client with host ip address (string) and port number (int)
            s = client.GetStream();                     //Initiate Socket steam
            //byte[] byteBuffer = Encoding.UTF8.GetBytes("Connected to client");  //Encode string message to be sent to host 
            byte[] byteBuffer = new byte[] {255, 255, 255, 255, 255, 255};        //Use this format to send raw bytes. Initialize with each byte  set to 255
            //byte[] byteBuffer = Encoding.UTF8.GetBytes("127, 0, 0, 0, 0, 0");     //Use this format to send bytes encoded as a string
            s.Write(byteBuffer, 0, byteBuffer.Length);      //Send message to host by writing to the stream in format: s.Write(byteArrayToSend, ArrayStartIndex, NumBytesToSend)
            while (true) {          //create loop to continuously receive data
                byte[] buffer = new byte[client.ReceiveBufferSize];     //Create byte array to hold received data

                int bytesRead = s.Read(buffer, 0, client.ReceiveBufferSize);            //Store the number of bytes received from the stream - returned by s.Read()
                Debug.Log(bytesRead);       //display the number of bytes received
                if (bytesRead <= 0)         //If no bytes received or if data is invalid...break loop
                {
                    Debug.Log("No Data Received!");
                    break;
                }
                //string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);    //Ohterwise... decode the bytes as a string
                //Debug.Log(dataReceived);        //display the decoded string
                byte[] array = buffer.SubArray(0, 6);       //Create subarray of received buffer to only include the first 6 bytes
                Debug.Log(BitConverter.ToString(array));        //Display the received data as bytes separated by dashes
                
                //Debug.Log(String.Join(",", array));       //Display subarray as 6 string values separated by commas

                //               pos = StringToVector3(dataReceived);        //Covert the string to a vector with three 8-bit values for each axis
                //               Debug.Log(pos);
            }
            s.Close();  //Close the stream whem no more data is received
        } 
        finally {       //if client is unable to connect, close client
            Debug.Log("Connection Closed!");
            client.Close();
        }
    }

    void Update() {
        transform.position = Vector3.Lerp(pos, transform.position, Time.deltaTime * speed);     //Update position of gameobject in the scene
    }

    public Vector3 StringToVector3(string sVector) {        //Convert received position data to a Vector3
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")")) {     //Remove parantheses from string data
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items in the received data
        string[] sArray = sVector.Split(',');       //Split the string based on a character separator. Create a new array with a separate element for each vector component.
        //string[] sArray = sVector.Split(' ');     //Spaces or other characters can be used for ease of input entry

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]))/divBy;          //Divide entire vector3 by 255 to get normalized values between 0-1 for each vector component.
                                                    //Each axis value is one byte. Decrease "divBy" to get larger axis values for position vector.
                                                    //When dividing by less than 255, convert "divBy" value to a float by adding a decimal place and the suffix "f" (e.g. 51.0f)

        Debug.Log(sArray[0]);       //display the Vector3 values
        Debug.Log(sArray[1]);
        Debug.Log(sArray[2]);

        return result;
    }

    void OnApplicationQuit()
    {
        try
        {
            mThread.Abort();        //Close thread upon quitting the application to prevent the thread from continuing to recieve information over the socket after the program has been stopped.
            Debug.Log("Quitting Application!");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}