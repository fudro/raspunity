import socket
import sys
import os
import serial
import time
import binascii


def setup_serial():
    global ser
    ser = serial.Serial('/dev/ttyUSB0', 9600, timeout=1)


def create_socket():
    global host
    global port
    global sock
    try:
        host = "192.168.1.4"       # Enter IP address of Raspberry Pi
        port = 9999
        sock = socket.socket()
        sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)  # SO_REUSEADDR allows the same socket to be closed and reopened without errors
        print("Python version: ")
        print(sys.version)
        print("\n")
    except socket.error as msg:
        print("Socket Creation Error: " + str(msg))


def bind_socket():
    try:
        print("Binding to Port: " + str(port))
        sock.bind((host, port))

        print("Listening for Connections...")
        sock.listen(5)     # The number of connections that can be queued up

    except socket.error as msg:
        print("Socket Binding Error: " + str(msg) + "\n" + "Retrying...")
        bind_socket()   # Repeat binding process if error
        

def socket_accept():    # After a connection is "heard" by the listen() function, accept it
    conn, address = sock.accept()      # conn represents the connection. address is a LIST that contains the IP address at index 0 and the port at index 1
    print("Connection Established! |" + " IP: " + address[0] + " | Port: " + str(address[1]))
    
    global data         # Bytes received from Unity
    global cmd_active   # Flag to tell if a command is currently active
    cmd_active = False  # Initialize flag as false once connection is made
    
    while True:
        if(cmd_active):
            message = ser.readline()    # Check for data received from MegaPi
            if(message):
                print("Received from MegaPi:")
                #Display each received byte in three formats: ASCII Number, Hex, and Char
                for char in message:
                        asciiValue = ord(char)
                        #cmd_elements = ["NUM", str(asciiValue), "HEX:", binascii.b2a_hex(char), "CHAR:", char]
                        #print('\t'.join(cmd_elements))
                        print(str(asciiValue))
                print("\n")
                #TODO: send command completion notification back to Unity client.
                # Send received data back to Unity client 
                # Non-sensor commands echo the sent command. Sensor commands return the sensor values.
                conn.sendall(message)  
                cmd_active = False  # Reset active command flag once reply from MegaPi is received
        else:
            data = conn.recv(1024)  #receive raw bytes from Unity Client
            #data = conn.recv(1024).decode("utf-8")  # Use this format to receive bytes as string
            conn.sendall(data)  # echo received data back to Unity client 
            # Use binascii module to convert the first byte of the data to an ASCII number in .
            # Use index notation to get the byte at a specific start index (inclusive) and stop index (non-inclusive). The byte returned is in HEX format.
            # In HEX format each byte is 2 alphanumeric digits. The entire digit sequence is zero indexed, so index[0:2] retrieves the two digits at indices 0 and 1.
            if int(binascii.b2a_hex(data)[0:2], 16) == 255:  # CMD ID 255 is reserved for startup confirmation.
                    print("Ready!\n")
            elif int(binascii.b2a_hex(data)[0:2], 16) < 255:        # If not the startup confirmation...
                    print(int(binascii.b2a_hex(data)[0:2], 16))     # Convert and display the first element of the array (the command ID)
                    cmd_active = True   # Initiaize flag as True once data is received
                    check_command(binascii.b2a_hex(data)[0:2])      # Check the command ID


def check_command(cmd):     # Check if the command should be run on the Raspberry Pi or the MegaPi
    # Check for specific commands to run on Raspberry Pi
    if int(cmd, 16) <= 10:  # Convert to INT from HEX and compare. First 10 command IDs are reserved for Raspberry Pi.
        print("Run Command on Raspberry Pi!")
        if int(cmd, 16) == 10:  #CMD 10: Launch gStreamer
                print("Launching Camera Feed!")
                os.system("gst-launch-1.0 -v v4l2src device=/dev/video0 num-buffers=-1 ! video/x-raw, width=640, height=480, framerate=30/1 ! videoconvert ! jpegenc ! rtpjpegpay ! udpsink host=192.168.1.20 port=5200")
    # Check for specific commands to run on MegaPi
    elif int(cmd, 16) > 10:
        print("Sending to Mega Pi:")
        for char in data:   # Print each byte in the data array in three formats: ASCII Number, HEX, and CHAR
                asciiValue = ord(char)
                #data_elements = ["NUM", str(asciiValue), "HEX:", binascii.b2a_hex(char), "CHAR:", char]
                #print('\t'.join(data_elements))
                print(str(asciiValue))
        print("\n")
        ser.write(data)     # Send data to MegaPi
    else:
        print("Command Not Recognized!")

def main():
    setup_serial()
    create_socket()
    bind_socket()
    socket_accept()


main()
