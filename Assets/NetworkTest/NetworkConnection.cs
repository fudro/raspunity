using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;


public class NetworkConnection : MonoBehaviour {
    Thread mThread;
    TcpClient client;
    NetworkStream s;
    Vector3 pos, oldPos = Vector3.zero;     //Stores decoded position data received over wifi
    public float speed = 10;                //How fast the target object moves
    public float divBy = 255;               //Divide each axis value by 255 to get normalized value between 0-1. Position vector has three 8-bit values (0-255) for each axis x, y, z. 
    public string host = "192.168.1.4";     //enter ip address of a raspberry pi that is on the same network as teh Unity device (PC, Mobile)
    public int port = 1234;                 //create a port number

    void Start() {
        /*try { 
            client = new TcpClient(host, port);
        } catch { }
        if (IsConnected(client)) {
            s = client.GetStream();
        }
        InvokeRepeating("pinger", 0f, 5f);*/
        ThreadStart ts = new ThreadStart(Client);       //Initialize ThreadStart and assign function to be run on the thread
        mThread = new Thread(ts);                       //Initialize thread with ThreadStart
        mThread.Start();                                //Start thread
    }

    void Client() {     //Create fuctino to be run on the thread
        //while (!IsConnected(client)) { }
        try {
            client = new TcpClient(host, port);         //Initialize client with host ip address (string) and port number (int)
            s = client.GetStream();                     //Initiate Socket steam
            byte[] byteBuffer = Encoding.UTF8.GetBytes("Connected to client");  //Encode message to send to host 
            s.Write(byteBuffer, 0, byteBuffer.Length);      //Send message to host (e.g. Raspberry Pi) in format s.Write(byteArrayToSend, ArrayStartIndex, NumBytesToSend)
            while (true) {
                byte[] buffer = new byte[client.ReceiveBufferSize];     //Creat byte array to hold received data

                int bytesRead = s.Read(buffer, 0, client.ReceiveBufferSize);            //Store the number of bytes received from the stream as an Int32
                string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);    //Decode the bytes as a string
                //Debug.Log(dataReceived);
                pos = StringToVector3(dataReceived);        //Covert the string to a vector with three 8-bit values for each axis
                Debug.Log(pos);
            }
            s.Close();
        } 
        finally {
            client.Close();
        }
    }

    void Update() {
        transform.position = Vector3.Lerp(pos, transform.position, Time.deltaTime * speed);
    }

    public Vector3 StringToVector3(string sVector) {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")")) {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');       //Split the string based on a character separator
        //string[] sArray = sVector.Split(' ');     //Spaces or other characters can be used for ease of input entry

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]))/divBy;          //divide each axis by 255 to get normalized value between 0-1

        Debug.Log(sArray[0]);
        Debug.Log(sArray[1]);
        Debug.Log(sArray[2]);

        return result;
    }
    /*
    void pinger() {
        if (!IsConnected(client)) { 
            client = new TcpClient(host, port);
            s = client.GetStream();
        }
        byte[] ping = Encoding.UTF8.GetBytes("ping");
        s.Write(ping, 0, ping.Length);
        Debug.Log("ping");
    }

    public bool IsConnected(TcpClient _tcpClient) {
        try {
            if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected) {
                /* pear to the documentation on Poll:
                    * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                    * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                    * -or- true if data is available for reading; 
                    * -or- true if the connection has been closed, reset, or terminated; 
                    * otherwise, returns false
                    */

        // Detect if client disconnected
        /*
        if (_tcpClient.Client.Poll(0, SelectMode.SelectRead)) {
                    byte[] buff = new byte[1];
                    if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0) {
                        // Client disconnected
                        return false;
                    } else {
                        return true;
                    }
                }

                return true;
            } else {
                return false;
            }
        } catch {
            return false;
        }
    }
    */
}