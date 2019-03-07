using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
namespace ClientSocketManager
{
    public class Conn
    {

        const int BUFFER_SIZE = 999999;
        public string userName;
        public Socket socket;

        public byte[] recvBuff = new byte[BUFFER_SIZE];
        public int recvCount = 0;

        public byte[] sendBuff = new byte[BUFFER_SIZE];
        public int sendCount = 0;
    }

}