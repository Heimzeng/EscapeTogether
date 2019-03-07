using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Data;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using Mono.Data.Sqlite;
using System.Runtime.Serialization;
using Google.Protobuf;
using Tutorial;
using ClientSocketManager;

public class Server : MonoBehaviour
{
    public static Server Instance { get; set; }

    static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static byte[] result = new byte[2048];
    private static byte[] toSend = new byte[2048];
    public static string dataPath;
    public static List<Room> rooms;
    public static List<Conn> conns;
    public static List<Thread> threads;
    public static Thread listenThread;
    public string host;
    public int port;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
        dataPath = Application.dataPath.ToString();
        SocketServie();
        rooms = new List<Room>();
        conns = new List<Conn>();
        threads = new List<Thread>();
        Debug.Log(rooms.Count);
    }
    

    void ServerStart()
    {
        dataPath = Application.dataPath.ToString();
        SocketServie();
        rooms = new List<Room>();
        conns = new List<Conn>();
        threads = new List<Thread>();
        Debug.Log(rooms.Count);
    }

    public void SocketServie()
    {
        server.Bind(new IPEndPoint(IPAddress.Parse(host), port));
        server.Listen(100);//设定最多100个排队连接请求   
        listenThread = new Thread(ListenClientConnect);//通过多线程监听客户端连接
        listenThread.IsBackground = true;
        listenThread.Start();
        Debug.Log("Server start");
    }

    private static void ListenClientConnect()
    {
        while (true)
        {
            Socket clientSocket = server.Accept();
            Conn conn = new Conn();
            conn.socket = clientSocket;
            int position = conns.Count;
            conns.Add(conn);
            Debug.Log(conns.Count + "clients connected");
            Debug.Log("Client:" + clientSocket.RemoteEndPoint.ToString());
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.IsBackground = true;
            int po = threads.Count;
            threads.Add(receiveThread);
            threads[po].Start(position);
        }
    }

    public static void ReceiveMessage(object positionObject)
    {

        int position = (int)positionObject;
        Conn conn = conns[position];
        Socket myClientSocket = conn.socket;
        while (true)
        {
            Thread.Sleep(1);
            ToSend parameters;
            try
            {
                conn.recvCount = myClientSocket.Receive(conn.recvBuff);
                if (conn.recvCount == 0)
                    return;
                byte[] real = new byte[conn.recvCount];
                System.Buffer.BlockCopy(conn.recvBuff, 0, real, 0, conn.recvCount);
                parameters = ToSend.Parser.ParseFrom(real);
                //Debug.Log(parameters.Type);
                ToSend res = handleMessage(parameters);
                if (parameters.Type == "login" && res.Result == "s")
                {
                    conn.userName = parameters.User.Username;
                    Debug.Log("conns user add");
                }
                //conn.sendBuff = res.ToByteArray();
                myClientSocket.Send(res.ToByteArray(), res.ToByteArray().Length, 0);  //返回信息给客户端
                Debug.Log(conn.recvCount + "bytes received.");
                Debug.Log(res.ToByteArray().Length + "bytes sent.");
                conn.recvCount = 0;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                conns.RemoveAt(position);
            }
        }
        
    }

    public static ToSend handleMessage(ToSend parameters)
    {
        string type = parameters.Type;
        Debug.Log("Client Operation:" + type);
        if (type == "regist")
        {
            string name = parameters.User.Username;
            string password = parameters.User.Password;
            bool saveSuccessful = SaveToDataBase(name, password);
            if (saveSuccessful)
            {
                ToSend res = new ToSend();
                res.Type = type;
                res.Result = "s";
                return res;
            }
            else
            {
                ToSend res = new ToSend();
                res.Type = type;
                res.Result = "f";
                return res;
            }
        }
        else if (type == "login")
        {
            string name = parameters.User.Username;
            string password = parameters.User.Password;

            bool loginSuccessful = checkPassword(name, password);
            if (loginSuccessful)
            {
                ToSend res = new ToSend();
                res.Type = type;
                res.Result = "s";
                return res;
            }
            else
            {
                ToSend res = new ToSend();
                res.Type = type;
                res.Result = "f";
                return res;
            }

        }

        else if(type == "getfriends")
        {
            string name = parameters.User.Username;
            string friends = getFriends(name);
            ToSend res = new ToSend();
            res.Type = type;
            if(friends != null)
            {
                res.Result = "s";
            }
            else
            {
                res.Result = "f";
            }
            IndividualComfirm user = new IndividualComfirm();
            res.User = user;
            res.User.Username = friends;
            return res;
        }

        else if(type == "findfriend")
        {
            string name = parameters.User.Username;
            bool isexist = findUser(name);
            ToSend res = new ToSend();
            if (isexist)
            {
                res.Type = type;
                res.Result = "s";
                IndividualComfirm user = new IndividualComfirm();
                res.User = user;
                res.User.Username = name;
            }
            else
            {
                res.Type = type;
                res.Result = "f";
            }
            return res;
        }

        else if (type == "addfriend")
        {
            string name = parameters.User.Username;
            string friendName = parameters.User.Password;
            int index = getUserIndex(friendName);
            if(index != -1)
            {
                ToSend res = new ToSend();
                res.Type = "addFriendRequest";
                IndividualComfirm user = new IndividualComfirm();
                res.User = user;
                res.User.Username = name;
                res.User.Password = friendName;
                conns[index].socket.Send(res.ToByteArray());
            }
            else
            {
                //好友不在线解决方法
                //未实现
                //思路
                //用一个buffer储存，参数为name和ToSend
                //用户登录时获取一次
            }
            ToSend res2 = new ToSend();
            res2.Type = type;
            res2.Result = "s";
            return res2;
        }

        else if(type == "confirmAddFriend")
        {
            string name = parameters.User.Username;
            string friendName = parameters.User.Password;
            bool isAdded = addFriend(name, friendName);
            bool isAdded2 = addFriend(friendName, name);
            ToSend res = new ToSend();
            res.Type = type;
            if (isAdded || isAdded2)
            {
                res.Result = "s";
                int index = getUserIndex(name);
                if(index != -1)
                {
                    string friends = getFriends(name);
                    ToSend toSend = new ToSend();
                    toSend.Type = "Tgetfriends";
                    if (friends != null)
                    {
                        toSend.Result = "s";
                        
                    }
                    else
                    {
                        toSend.Result = "f";
                    }
                    IndividualComfirm user = new IndividualComfirm();
                    res.User = user;
                    res.User.Username = friends;
                    Debug.Log("confirm:" + friends);
                    conns[index].socket.Send(toSend.ToByteArray());
                }
                index = getUserIndex(friendName);
                if (index != -1)
                {
                    string friends = getFriends(friendName);
                    ToSend toSend = new ToSend();
                    toSend.Type = "Tgetfriends";
                    if (friends != null)
                    {
                        toSend.Result = "s";
                    }
                    else
                    {
                        toSend.Result = "f";
                    }
                    IndividualComfirm user = new IndividualComfirm();
                    res.User = user;
                    res.User.Username = friends;
                    Debug.Log("confirm:" + friends);
                    conns[index].socket.Send(toSend.ToByteArray());
                }
            }
            else
            {
                res.Result = "f";
            }
            return res;
        }

        else if (type == "getMaxLevel"){
            string name = parameters.User.Username;
            string maxLevel = getMaxLevel(name) + "";
            ToSend res = new ToSend();
            res.Type = type;
            if (maxLevel == null)
            {
                res.Result = "f";
            }
            else
            {
                res.Result = "s";
                IndividualComfirm user = new IndividualComfirm();
                res.User = user;
                res.User.Password = maxLevel;
            }
            return res;
        }
        
        else if(type == "getFriendState")
        {
            string name = parameters.User.Username;
            ToSend res = new ToSend();
            res.Type = type;
            if (getUserState(name))
            {
                res.Result = "s";
            }
            else
            {
                res.Result = "f";;
            }
            return res;
        }

        else if(type == "newRoom")
        {
            int roomNum = newRoom();
            ToSend res = new ToSend();
            res.Type = type;
            res.Result = "s";
            Room room = new Room();
            res.Room = room;
            res.Room.RoomNum = roomNum;
            for(int i = 0; i < 10; i++)
            {
                ItemToPickUp item = new ItemToPickUp();
                item.Belongs = "";
                rooms[roomNum].Items.Add(item);
            }
            
            return res;
        }

        else if (type == "addPlayer")
        {
            int roomNum = parameters.Room.RoomNum;
            string name = parameters.User.Username;
            int playerNum = addPlayer(roomNum, name);
            for (int i = 0; i <= playerNum; i++)
            {
                int index = getUserIndex(rooms[roomNum].Players[i].Name);
                Socket socket = conns[index].socket;
                if(index != -1)
                {
                    ToSend toSend = new ToSend();
                    toSend.Type = "updateRoomPlayers";
                    toSend.Result = "s";
                    Room roomToSend = rooms[roomNum];
                    toSend.Room = roomToSend;
                    socket.Send(toSend.ToByteArray());
                    Debug.Log("update send to player " + i);
                }
            }
            ToSend res = new ToSend();
            res.Type = type;
            res.Result = "s";
            Room room = new Room();
            res.Room = room;
            res.Room.RoomNum = playerNum;
            return res;
        }

        else if(type == "startGame")
        {
            int roomNum = parameters.Room.RoomNum;
            rooms[roomNum].State = 0;
            int playerCount = rooms[roomNum].Players.Count;
            for (int i = 0; i < playerCount; i++)
            {
                int index = getUserIndex(rooms[roomNum].Players[i].Name);
                if (index != -1)
                {
                    ToSend toSend = new ToSend();
                    toSend.Type = "startGaming";
                    toSend.Result = "s";
                    Room roomToSend = rooms[roomNum];
                    toSend.Room = roomToSend;
                    conns[index].socket.Send(toSend.ToByteArray());
                }
            }
            ToSend res = new ToSend();
            res.Type = type;
            res.Result = "s";
            return res;
        }

        else if (type == "deletePlayer")
        {
            int roomNum = parameters.Room.RoomNum;
            int playerNum = parameters.Room.Level;
            bool isRemoved = removePlayer(roomNum, playerNum);
            ToSend res = new ToSend();
            res.Type = type;
            res.Result = "s";
            return res;
        }

        else if(type == "inviteFriend")
        {
            int roomNum = parameters.Room.RoomNum;
            string userName = parameters.User.Username;
            string friendName = parameters.User.Password;
            int index = getUserIndex(friendName);
            Debug.Log("The " + index + " one in conns");
            //Debug.Log("user0:"+conns[0].userName);
            //Debug.Log("user1:" + conns[1].userName);
            if (index != -1)
            {
                ToSend res = new ToSend();
                res.Type = "invitation";
                IndividualComfirm user = new IndividualComfirm();
                res.User = user;
                res.User.Username = userName;
                Room room = new Room();
                res.Room = room;
                res.Room.RoomNum = roomNum;
                conns[index].socket.Send(res.ToByteArray());
                Debug.Log(userName + " invites " + friendName + " to room: " + roomNum);  
            }
            
            ToSend res2 = new ToSend();
            res2.Type = type;
            res2.Result = "s";
            
            return res2;
        }

        else if(type == "setRoomLevel")
        {
            int roomNum = parameters.Room.RoomNum;
            int level = parameters.Room.Level;
            int count = rooms[roomNum].Players.Count;
            bool pass = true;
            for(int i = 0; i < count; i++)
            {
                if(getMaxLevel(rooms[roomNum].Players[i].Name) < level)
                {
                    pass = false;
                    break;
                }
            }
            ToSend res = new ToSend();
            res.Type = type;
            if (pass)
            {
                rooms[roomNum].Level = level;
                res.Result = "s";
            }
            else
            {
                res.Result = "f";
            }
            return res;
        }

        else if(type == "updatePlayerState")
        {
            int roomNum = parameters.Room.RoomNum;
            int playerNum = parameters.Room.Level;
            int state = parameters.Room.Players[0].State;
            updatePlayerState(roomNum, playerNum, state);
            ToSend res = new ToSend();
            res.Type = "updateRoomPlayers";
            res.Result = "s";
            res.Room = rooms[roomNum];
            return res;
        }

        else if(type == "updatePlayerPosition")
        {
            int roomNum = parameters.Room.RoomNum;
            int playerNum = parameters.Room.Level;
            Position position = parameters.Room.Players[0].Position;
            int state = parameters.Room.Players[0].State;
            updatePlayerPosition(roomNum, playerNum, position, state);
            ToSend res = new ToSend();
            res.Type = type;
            res.Room = rooms[roomNum];
            Debug.Log(res.Room.Players.Count);
            res.Result = "s";
            return res;
        }

        else if (type == "updatePlayerMovement")
        {
            int roomNum = parameters.Room.RoomNum;
            int playerNum = parameters.Room.Level;
            Movement movement = parameters.Room.Players[0].Movement;
            int state = parameters.Room.Players[0].State;
            Room room = updatePlayerMovement(roomNum, playerNum, movement, state);
            ToSend res = new ToSend();
            res.Type = type;
            res.Room = room;
            res.Result = "s";
            return res;
        }

        else if(type == "getRoom")
        {
            int roomNum = parameters.Room.RoomNum;
            ToSend res = new ToSend();
            res.Type = "updateRoomPlayers";
            res.Result = "s";
            res.Room = rooms[roomNum];
            return res;

        }

        else if(type == "UpdateItem")
        {
            int roomNum = parameters.Room.RoomNum;
            string name = parameters.Room.Players[0].Name;
            int itemNum = parameters.Room.Players[0].State;
            updateItem(roomNum, name, itemNum);
            ToSend res = new ToSend();
            res.Type = "UpdateItem";
            res.Result = "s";
            res.Room = rooms[roomNum];
            return res;
        }

        else if(type == "EndGame")
        {
            int roomNum = parameters.Room.RoomNum;
            ToSend res = new ToSend();
            res.Type = type;
            res.Result = "s";
            EndGame(roomNum);
            return res;
        }

        else
        {
            ToSend res = new ToSend();
            res.Type = type;
            res.Result = "s";
            return res;
        }
    }

    public static bool SaveToDataBase(string name, string password)
    {
        Debug.Log("[" + name + "] try to regist with the password [" + password + "]");
        string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
        SqliteConnection dbconn;
        dbconn = new SqliteConnection(userDB);
        dbconn.Open(); //Open connection to the database.
        SqliteCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT ID, PASSWORD " + "FROM user " + "WHERE NAME = " + "'" + name + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        bool isExist;
        isExist = reader.Read();
        Debug.Log(isExist);
        bool res = false;
        if (isExist)
        {
            res =  false;
        }
        else
        {
            string seleteMaxId = "Select Max(ID) From user";
            SqliteCommand seleteMaxIdcmd = dbconn.CreateCommand();
            seleteMaxIdcmd.CommandText = seleteMaxId;
            IDataReader reader2 = seleteMaxIdcmd.ExecuteReader();
            reader2.Read();
            int maxId = reader2.GetInt16(0);
            reader2.Close();
            reader2 = null;
            seleteMaxIdcmd.Dispose();
            seleteMaxIdcmd = null;

            int thisId = maxId + 1;
            //一定要大写
            string insertNewUser = "INSERT INTO user (ID,NAME,PASSWORD,ENERGY,MAXLEVEL,FRIENDS) Values ('" + thisId + "','" + name + "','" + password + "'," + "'9','1','"+ "" +"'" + ")";
            Debug.Log(insertNewUser);
            SqliteCommand insertNewUsercmd = dbconn.CreateCommand();
            insertNewUsercmd.CommandText = insertNewUser;
            insertNewUsercmd.ExecuteNonQuery();
            Debug.Log("User [" + name + "] added.");
            insertNewUsercmd.Dispose();
            insertNewUsercmd = null;
            res = true;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return res;
    }

    public static bool showAllUsers()
    {
        string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
        SqliteConnection dbconn;
        dbconn = new SqliteConnection(userDB);
        dbconn.Open(); //Open connection to the database.
        SqliteCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT ID, NAME, PASSWORD " + "FROM user";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int value = reader.GetInt32(0);
            string username = reader.GetString(1);
            string passw = reader.GetString(2);

            Debug.Log("value = " + value + "  username = " + username + "  password = " + passw);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return true;
    }

    public static bool checkPassword(string name, string password)
    {
        string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
        SqliteConnection dbconn;
        dbconn = new SqliteConnection(userDB);
        dbconn.Open(); //Open connection to the database.
        SqliteCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT ID, PASSWORD " + "FROM user " + "WHERE NAME = " + "'" + name + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        bool isExist;
        isExist = reader.Read();
        //Debug.Log(isExist);
        bool res = false;
        if (isExist)
        {
            int id = reader.GetInt16(0);
            string pass = reader.GetString(1);
            if (pass == password)
            {
                res = true;
            }
            else
            {
                res = false;
            }
        }
        else
        {
            res = false;
        }
        
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return res;
    }

    public static string getFriends(string name)
    {
        
        string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
        SqliteConnection dbconn;
        dbconn = new SqliteConnection(userDB);
        dbconn.Open(); //Open connection to the database.
        SqliteCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT FRIENDS " + "FROM user " + "WHERE NAME = " + "'" + name + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        bool isExist;
        isExist = reader.Read();
        string friends = null;
        if (isExist)
        {
            friends = reader.GetString(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return friends;
        
    }

    public static bool findUser(string name)
    {
        string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
        SqliteConnection dbconn;
        dbconn = new SqliteConnection(userDB);
        dbconn.Open(); //Open connection to the database.
        SqliteCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT ID " + "FROM user " + "WHERE NAME = " + "'" + name + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        bool isExist;
        isExist = reader.Read();
        Debug.Log(isExist);
        bool res = false;
        if (isExist)
        {
            res = true;
        }
        else
        {
            res = false;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return res;
    }

    public static bool addFriend(string name, string friendName)
    {
        bool res;
        if (findUser(friendName))
        {
            string friends = getFriends(name);
            if (friends.Contains(friendName))
            {
                res = false;
            }
            else
            {
                string newFriends = friends + ";" + friendName;
                string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
                SqliteConnection dbconn;
                dbconn = new SqliteConnection(userDB);
                dbconn.Open(); //Open connection to the database.
                SqliteCommand dbcmd = dbconn.CreateCommand();
                string updateFriends = "UPDATE user " + "SET FRIENDS = " + "'" + newFriends + "' " + "WHERE NAME = " + "'" + name + "'";
                Debug.Log(updateFriends);
                dbcmd.CommandText = updateFriends;
                dbcmd.ExecuteNonQuery();
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                res = true;
            }
        }
        else
        {
            res = false;
        }
        return res;
    }

    public static bool getUserState(string name)
    {
        int count = conns.Count;
        for (int i = 0;i < count; i++)
        {
            if(conns[i].userName == name)
            {
                return true;
            }
        }
        return false;
    }

    public static int getUserIndex(string name)
    {
        int count = conns.Count;
        for (int i = 0; i < count; i++)
        {
            if (conns[i].userName == name)
            {
                return i;
            }
        }
        return -1;
    }

    public static int getMaxLevel(string name)
    {
        string userDB = "URI=file:" + dataPath + "/user.s3db"; //Path to database.
        SqliteConnection dbconn;
        dbconn = new SqliteConnection(userDB);
        dbconn.Open(); //Open connection to the database.
        SqliteCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT MAXLEVEL " + "FROM user " + "WHERE NAME = " + "'" + name + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        bool isExist;
        isExist = reader.Read();
        int maxl = 0;
        if (isExist)
        {
            maxl = reader.GetInt16(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        return maxl;
    }


    void OnDestroy()
    {
        int count = threads.Count;
        for (int i = 0; i < count; i++)
        {
            threads[i].Abort();
            Debug.Log("thread " + i + "is closed");
        }
        listenThread.Abort();
        print("Script was destroyed");
    }

    public static int newRoom()
    {
        Room room = new Room();
        room.RoomNum = rooms.Count;
        rooms.Add(room);
        return room.RoomNum;
    }

    public static int addPlayer(int roomNum, string name)
    {
        Player add = new Player();
        int position = rooms[roomNum].Players.Count;
        add.Name = name;
        rooms[roomNum].Players.Add(add);
        return position;
    }

    public static bool setPlayerType(int roomNum, int playerIndex, int type)
    {
        rooms[roomNum].Players[playerIndex].PlayerType = type;
        return true;
    }

    public static bool removePlayer(int roomNum, int playerIndex)
    {
        rooms[roomNum].Players.RemoveAt(playerIndex);
        return true;
    }

    public static bool updateRoomLevel(int roomNum, int level)
    {
        rooms[roomNum].Level = level;
        return true;
    }

    public static bool updatePlayerPosition(int roomNum, int playerIndex, Position newPosition, int state)
    {
        rooms[roomNum].Players[playerIndex].Position = newPosition;
        rooms[roomNum].Players[playerIndex].State = state;
        return true;
        
    }

    public static Room updatePlayerMovement(int roomNum, int playerIndex, Movement newMovement, int state)
    {
        rooms[roomNum].Players[playerIndex].Movement = newMovement;
        rooms[roomNum].Players[playerIndex].State = state;
        //Debug.Log("ff"+rooms[roomNum].Players[playerIndex].Movement.RoateX);
        return rooms[roomNum];

    }

    public static bool updatePlayerState(int roomNum, int playerIndex, int state)
    {
        rooms[roomNum].Players[playerIndex].State = state;
        return true;
    }

    public static bool updateItem(int roomNum, string name, int itemNum)
    {
        if (rooms[roomNum].Items[itemNum].Belongs == "")
        {
            rooms[roomNum].Items[itemNum].Belongs = name;
        }
        return true;
    }

    public static bool EndGame(int roomNum)
    {
        rooms[roomNum].State = 1;
        return true;
    }

}


