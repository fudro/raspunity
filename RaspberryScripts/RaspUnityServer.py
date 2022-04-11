import socket
import sys
import os
import binascii


def create_socket():
    try:
        global host
        global port
        global s
        host = "192.168.1.4"       # Enter IP address of Raspberry Pi
        port = 9999
        s = socket.socket()
        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        print("Python version: ")
        print(sys.version)
        print("\n")
    except socket.error as msg:
        print("Socket Creation Error: " + str(msg))


def bind_socket():
    try:
        global host
        global port
        global s

        print("Binding to Port: " + str(port))
        s.bind((host, port))

        print("Listening for Connections...")
        s.listen(5)     # The number of connections that can be queued up

    except socket.error as msg:
        print("Socket Binding Error: " + str(msg) + "\n" + "Retrying...")
        bind_socket()   # Repeat binding process if error
        

def socket_accept():    # After a connection is "heard" by the listen() function, accept it
    conn, address = s.accept()      # conn represents the connection. address is a LIST that contains the IP address at index 0 and the port at index 1
    print("Connection Established! |" + " IP: " + address[0] + " | Port: " + str(address[1]))

    while True:
        data = conn.recv(1024)  #receive raw bytes
        #data = conn.recv(1024).decode("utf-8")  #receive bytes as string
        if not data:        #if reach end of current message
            break           #stop reading
        conn.sendall(data)  #echo received data back to client
        #display received data at a specific start index (inclusive) and stop index (non-inclusive) in hex format
        #print(binascii.b2a_hex(data)[6:8])  #In hex format each byte is 2 digits, with the entire digit sequence being zero indexed, so index[6:8] represents the "fourth" byte received
        print(int(binascii.b2a_hex(data)[0:2], 16))
        get_command(binascii.b2a_hex(data)[0:2])    #check first byte (2 digit hex value)


def get_command(cmd):
    #if cmd.isnumeric():
    if int(cmd, 16) <= 10:
        print("Run Command on Raspberry Pi!")
        os.system("gst-launch-1.0 -v v4l2src device=/dev/video0 num-buffers=-1 ! video/x-raw, width=640, height=480, framerate=30/1 ! videoconvert ! jpegenc ! rtpjpegpay ! udpsink host=192.168.1.20 port=5200")
    elif int(cmd, 16) > 10:
        print("Run Command on Mega Pi!")
    else:
        print("Command Not Recognized!")

def main():
    create_socket()
    bind_socket()
    socket_accept()


main()
