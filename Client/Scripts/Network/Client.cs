using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Google.Protobuf;
using Tutorial;
using System;

public class Client : MonoBehaviour {

    public static Client Instance { get; set; }

    public string host;
    public int port;
    IPAddress ip;
    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    Thread listeningThread;
    bool canSend = true;

    void Start()
    {
        ip = IPAddress.Parse(host);
        ConnectToServer(clientSocket, ip);
       // ConnectToServer(listenSocket, ip);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    
    void OnDestroy()
    {
        clientSocket.Close();
    }


    public bool ConnectToServer(Socket socket, IPAddress ip)
    {
        try
        {
            socket.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
            Debug.Log("Connect to server successfully.");
            return true;
        }
        catch(SocketException e)
        {
            Debug.Log(e.SocketErrorCode);
            return false;
        }
    }

    public bool GetFriends(string username)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "getfriends";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = username;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string friends = res.User.Username;
            string[] friendslist = friends.Split(';');
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool login(string username, string password)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "login";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = username;
            toSend.User.Password = password;
            clientSocket.Send(toSend.ToByteArray());
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string type = res.Type;
            string result = res.Result;
            if (result == "s" && type == "login")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }


    public bool regist(string username, string password)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "regist";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = username;
            toSend.User.Password = password;
            clientSocket.Send(toSend.ToByteArray());
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string result = res.Result;
            if (result == "s")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
        
    }

    public bool findFriend(string friendName)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "findfriend";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = friendName;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string result = res.Type;
            if (result == "s")
            {
                return true;
            }
            else
            {
                return false;
            }
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool addFriend(string name, string friendName)
    {

        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "addfriend";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = name;
            toSend.User.Password = friendName;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string result = res.Type;
            if (result == "s")
            {
                return true;
            }
            else
            {
                return false;
            }
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool confirmAddFriend(string name, string friendName)
    {

        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "confirmAddFriend";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = name;
            toSend.User.Password = friendName;
            clientSocket.Send(toSend.ToByteArray());
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool getMaxLevel(string name)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "getMaxLevel";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = name;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string result = res.Type;
            if (result == "s")
            {
                string maxLevel = res.User.Password;
                return maxLevel;
            }
            else
            {
                return null;
            }
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool getFrendState(string name)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "getFriendState";
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = name;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            string result = res.Type;
            if (result == "s")
            {
                return true;
            }
            else
            {
                return false;
            }
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool newRoom()
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "newRoom";
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            int roomNum = res.Room.RoomNum;
            return roomNum;
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool addPlayer(int roomNum, string name)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "addPlayer";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = name;
            clientSocket.Send(toSend.ToByteArray());
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool deletePlayer(int roomNum, int playerNum)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "deletePlayer";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            toSend.Room.Level = playerNum;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            if(res.Type == "s")
            {
                return true;
            }
            else
            {
                return false;
            }*/
            return true;

        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }



    public bool setRoomLevel(int roomNum, int level)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "setRoomLevel";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            toSend.Room.Level = level;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            if (res.Type == "s")
            {
                return true;
            }
            else
            {
                return false;
            }*/
            return true;

        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool updatePlayerState(int roomNum, int playerNum, int state)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "updatePlayerState";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            toSend.Room.Level = playerNum;
            Player player = new Player();
            player.State = state;
            toSend.Room.Players.Add(player);
            clientSocket.Send(toSend.ToByteArray()); 
  
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool updatePlayerPosition(int roomNum, int playerNum, Position position, int state)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "updatePlayerPosition";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            toSend.Room.Level = playerNum;
            Player player = new Player();
            player.Position = position;
            player.State = state;
            toSend.Room.Players.Add(player);
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            Debug.Log("a room is:" + bytes + "bytes.");
            ToSend res = ToSend.Parser.ParseFrom(real);
            if (res.Type == "s")
            {
                List<Player> players = new List<Player>();
                int count = res.Room.Players.Count;
                for (int i = 0; i < count; i++)
                {
                    players.Add(res.Room.Players[i]);
                }
                return players;
            }
            else
            {
                return null;
            }
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool updatePlayerMovement(int roomNum, int playerNum, Movement movement, int state)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "updatePlayerMovement";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            toSend.Room.Level = playerNum;
            Player player = new Player();
            player.Movement = movement;
            player.State = state;
            toSend.Room.Players.Add(player);
            clientSocket.Send(toSend.ToByteArray());
            Debug.Log("udpm");
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }


    public bool inviteFriend(int roomNum, string userName, string friendName)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "inviteFriend";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = userName;
            toSend.User.Password = friendName;
            clientSocket.Send(toSend.ToByteArray());
            /*
            byte[] recvBytes = new byte[9999];
            int bytes;
            bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息 
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            */
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool giveItem(int roomNum, string userName, string friendName, int itemNum)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "GiveItem";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            IndividualComfirm user = new IndividualComfirm();
            toSend.User = user;
            toSend.User.Username = userName;
            toSend.User.Password = friendName;
            toSend.Room.State = itemNum;
            try
            {
                clientSocket.Send(toSend.ToByteArray());
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool startGame(int roomNum)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "startGame";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            clientSocket.Send(toSend.ToByteArray());
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool UpdateItem(int roomNum, string name, int itemNum)
    {
        if (clientSocket.Connected)
        {
            Debug.Log("updateitem:" + name + " take" +  itemNum);
            ToSend toSend = new ToSend();
            toSend.Type = "UpdateItem";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            Player p = new Player();
            p.Name = name;
            p.State = itemNum;
            toSend.Room.Players.Add(p);
            clientSocket.Send(toSend.ToByteArray());
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool getRoom(int roomNum)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "getRoom";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            clientSocket.Send(toSend.ToByteArray());
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }

    public bool EndGame(int roomNum)
    {
        if (clientSocket.Connected)
        {
            ToSend toSend = new ToSend();
            toSend.Type = "EndGame";
            Room room = new Room();
            toSend.Room = room;
            toSend.Room.RoomNum = roomNum;
            clientSocket.Send(toSend.ToByteArray());
            return true;
        }
        else
        {
            Debug.Log("Fail to connect to server.");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(clientSocket, ip);
            if (clientSocket.Connected)
            {
                Debug.Log("Reconnected to server.");
            }
            else
            {
                Debug.Log("Connot reconnect to server.");
            }
            return false;
        }
    }



    public ToSend getServerRespone()
    {
        Debug.Log("start listening for invitation");
        byte[] recvBytes = new byte[99999];
        Thread.Sleep(1);
        try
        {
            int bytes = clientSocket.Receive(recvBytes);
            byte[] real = new byte[bytes];
            System.Buffer.BlockCopy(recvBytes, 0, real, 0, bytes);
            ToSend res = ToSend.Parser.ParseFrom(real);
            return res;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return null;
    }

    

    
}
