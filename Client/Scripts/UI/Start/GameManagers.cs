using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class GameManagers : MonoBehaviour {
    public static GameManagers instance;

    public List<GameObject> m_PlayerPrefabs;
    public List<PlayerManagers> m_Players = new List<PlayerManagers>();

    public Texture LoadingImg;

    private static int m_playerNum;
    private static string m_playerID;
    private static int m_RoomNum;
    private static int m_state = 0;
    private static Room m_Room = new Room();
    private List<string> m_Friends = new List<string>();
    private string m_chosenLevel = "Test_4";
    private int m_topLevel;
    private static WaitForSeconds m_StartWait;
    private static WaitForSeconds m_EndWait;
    private static bool isLogged = false;
    private static AsyncOperation m_async = null;
    private static bool isUpdate = true;
    private static bool Loaded = false;
    private static bool isBack = false;
    private static bool isGameStart = false;
    public static bool isExist = false;
    public static bool isToUpdateFriends = false;
    public static bool isRoomReady = false;
    private bool isEnded = false;
    private bool[] isItemFound = new bool[10];
    private bool isreceiveitem = false;
    private int receiveitemnum;

    private List<string> itemsName = new List<string>();

    bool oneTime = true;

    public Thread listeningThread;

    public Thread requestThread;

    public int askornot = 0;
    public int kindofask = 0;
    public string inviteuser = "空";
    public int roomnumber;

    public string charactor = "Ken";

    public void Awake()
    {
        instance = this;
        if (oneTime)
        {
            listeningThread = new Thread(receiveAndHandle);
            listeningThread.IsBackground = true;
            oneTime = false;
        }
        // DontDestroyOnLoad(Instance);
    }

    public static GameManagers Instance
    {
        get
        {
            return instance;
        }
    }


    // Use this for initialization
    private void Start () {
        if (isLogged == false)
        {
            PanelMgr.instance.OpenPanel<LoginPanel>("");
        }
        DontDestroyOnLoad(this);
        
    }

   private void Update()
    {
        if (!isUpdate)
        {
            if (m_async.isDone)
            {
               
                if (m_async.isDone)
                {
                    itemsName = new List<string>();
                    itemsName = GameObject.Find("ItemsList").GetComponent<ItemsList>().items;
                    InitIsItemFound();
                    SpawnAllPlayers();
                    m_StartWait = new WaitForSeconds(10.0f);
                    m_EndWait = new WaitForSeconds(4.0f);          

                    StartCoroutine(GameLoop());

                    requestThread = new Thread(Request);
                    requestThread.IsBackground = true;
                    requestThread.Start();
                }
                isUpdate = true;
            }
            
        }

        if (isBack)
        {
            PanelMgr.instance.OpenPanel<HomePanel>("");
            isBack = false;
        }

    }

    public IEnumerator LoadScence()
    {
        m_async = Application.LoadLevelAsync(m_chosenLevel);
        Debug.Log("complete");
        Loaded = true;
        isUpdate = false;
        isEnded = false;

        yield return m_async;
   }

    private void OnGUI()
    {
        float time = 0;
        float width = 0;
        if (m_async != null && m_async.isDone == false)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), LoadingImg);

            time += Time.deltaTime;
            if (time >= 0.01f)
            {
                time = 0;
                width += (m_async.progress + 100);
                Texture probar = Texture2D.whiteTexture;
                if (width > Screen.width / 5 * 4) width = 0;
                GUI.DrawTexture(new Rect(Screen.width / 5, 200, width, 30), probar);
            }
            GUI.Label(new Rect(Screen.width / 5 + 120, 200, 100, 20), (float)m_async.progress * 100 + "%");
        }
     

    }

    void OnDisable()
    {
        listeningThread.Abort();
        requestThread.Abort();
        Debug.Log("disable");
    }

    public void SpawnAllPlayers()
    {
        
        for (int i = 0; i < m_Room.Players.Count; i++)
        {
            GameObject tempSpp = new GameObject();
            tempSpp = GameObject.Find("Spawnpoint" + (i+1));
            PlayerManagers temPlayer = new PlayerManagers();
            temPlayer.m_Instance = Instantiate(m_PlayerPrefabs[i], tempSpp.transform.position, tempSpp.transform.rotation) as GameObject;
            temPlayer.m_PlayerID = m_Room.Players[i].Name;
            Debug.Log(i + m_Room.Players[i].Name);
            temPlayer.m_PlayerNum = i; 
            if (m_Room.Players[i].Name == m_playerID)
            {
                temPlayer.m_Inputname = "Self";
                setPlayernum(i);
                Debug.Log(m_playerID + " self is floor" + i);
            }

            else
            {
                temPlayer.m_Inputname = "others";
                temPlayer.KilltheCamera();
            }
                
            temPlayer.Setup();
            m_Players.Add(temPlayer);
        }

        DisablePlayers();

        if (getPlayernum() == 0)
        {
            charactor = "Ken";
        }
        if(getPlayernum() == 1)
        {
            charactor = "Reji";
        }
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameStarting());
        yield return StartCoroutine(GamePlaying());
        yield return StartCoroutine(GameEnding());
    }

    private IEnumerator GameStarting()
    {
        yield return m_StartWait;
    }

    private IEnumerator GamePlaying()
    {
        while(!isEnded)
        {
            Debug.Log("Player " + m_playerID + " is playing");
            if (isreceiveitem)
            {
                isreceiveitem = false;
                Inventory.Instance.AddItem(ImageShowingSystem.Instance.items[receiveitemnum]);
            }
            updateOPs();
            //updateRoom();
            yield return null;
        }
    }

    public void EndGame()
    {
        isEnded = true;
        Loaded = false;
        Debug.Log("end?" + isEnded);
    }

    private void updateOPs()
    {
        Debug.Log("updating pos");
        for(int i = 0; i < m_Players.Count; i++)
        {
            Debug.Log(i+ m_Players[i].m_PlayerID);
            Debug.Log("load" + i + " " + m_Room.Players[i].State);
            if(m_Room.Players[i].Name != m_playerID && (m_Room.Players[i].State == -1 || m_Room.Players[i].State == 1))
            {
                
                Position respos = m_Room.Players[i].Position;
                if (respos.X < 0.5) break;
                Vector3 newpos = new Vector3(respos.X, respos.Y, respos.Z);
                m_Players[i].getMovement().MoveByTrans(newpos);
                m_Players[i].getRoate().RoateByX(respos.W);

                Debug.Log("updating " + m_Players[i].m_PlayerID);
                if (m_Room.Players[i].State == 1) m_Players[i].getMovement().jump();
            }
        }
        for(int i = 0; i < itemsName.Count; i++)
        {
            string belongs = m_Room.Items[i].Belongs;
            Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbb" + belongs);
            /*
            if (i >= 8)
            {
                if(belongs != "" && belongs != instance.getPlayerid())
                {
                    if(i == 8)
                    {
                        //GameObject.Find("drawer_02").GetComponent<MObject>().isDone = true;
                    }
                    else if(i == 9)
                    {
                        GameObject.Find("case_04").GetComponent<MObject>().isDone = true;
                        GameObject.Find("case_04").GetComponent<MObject>().isLockBox = false;
                    }
                }
            }
            else if (belongs != "")
            {
                if(belongs != instance.getPlayerid())
                {
                    Debug.Log(belongs + "takes"+ itemsName[i]);
                    try
                    {
                        GameObject.Find(itemsName[i]).active = false;
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }*/
            if (belongs != "")
            {
                Debug.Log(belongs + "takes" + itemsName[i]);
                int type = 0;
                try
                {
                    if(GameObject.Find(itemsName[i]).tag == "Interactable Object" || GameObject.Find(itemsName[i]).tag == "UseDeleteAndGet")
                    {
                        type = 1;
                    }
                    else if(GameObject.Find(itemsName[i]).tag == "KeyItems")
                    {
                        type = 2;
                    }
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
                Debug.Log(belongs + "takes" + "a type " + type);
                if (type == 1)
                {
                    Debug.Log(belongs + "takes" + itemsName[i]);
                    try
                    {
                        GameObject.Find(itemsName[i]).active = false;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
                else if(type == 2)
                {
                    Debug.Log(belongs + "solve" + itemsName[i]);
                    try
                    {
                        GameObject.Find(itemsName[i]).GetComponent<MObject>().isDone = true;
                        instance.ItemFound(GameObject.Find(itemsName[i]).GetComponent<MObject>().relativeItemIndex);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }
            
        }
        if(m_Room.State == 1)
        {
            EndGame();
        }
    }

    private IEnumerator GameEnding()
    {
        Text endText = GameObject.Find("DialogSystem").GetComponent<DialogSystem>().dialogPanel.transform.Find("Text").GetComponent<Text>();
        endText.text = "we finally fuck the hell room and ESC to back";
        GameObject.Find("DialogSystem").GetComponent<DialogSystem>().dialogPanel.SetActive(true);
        Debug.Log("out");
        DisablePlayers();
        isBack = true;
        if (Input.GetMouseButton(0))
        {
            GameManagers.Instance.SetIsRoomReady(false);
            Application.LoadLevelAsync("UI");
        }


        yield return m_EndWait;
    }

    public void DisablePlayers()
    {
        m_Players[m_playerNum].PlayerDisable();
    }

    public void EnablePlayers()
    {
        m_Players[m_playerNum].PlayerEnable();
        
    }

    public void setPlayerid(string id)
    {
        m_playerID = id;
    }
    public string getPlayerid()
    {
        return m_playerID;
    }

    public void setPlayernum(int num)
    {
        m_playerNum = num;
    }

    public int getPlayernum()
    {
        return m_playerNum;
    }

    public void setRoomNum(int rnum)
    {
        m_RoomNum = rnum;
    }
    
    public int getRoomNum()
    {
        return m_RoomNum;
    }

    public void addFriend(string fname)
    {
        m_Friends.Add(fname);
    }

    public void setFriend(List<string> list)
    {
        m_Friends = list;
    }

    public List<string> getFriend()
    {
        return m_Friends;
    }

    public void blockFriend(string fname)
    {
        m_Friends.Remove(fname);
    }

    public void setLevel(int level)
    {
        m_chosenLevel = "level" + level;
    }

    public string getCurrentLevel()
    {
        return m_chosenLevel;
    }

    public int getTopLevel()
    {
        return m_topLevel;
    }

    public void setRoom(Room room)
    {
        m_Room = room;
    }

    public void changeGameState(bool state)
    {
        isGameStart = state;
    }

    public bool getGameState()
    {
        return isGameStart;
    }

    public Room GetRoom()
    {
        return m_Room;
    }


    public bool GetIsRoomReady()
    {
        return isRoomReady;
    }

    public void SetIsRoomReady(bool isR)
    {
        isRoomReady = isR;
    }

    public bool GetIsExist()
    {
        return isExist;
    }

    public void SetIsExist(bool isE)
    {
        isExist = isE;
    }

    public bool GetIsToUpdateFriends()
    {
        return isToUpdateFriends;
    }

    public void SetIsToUpdateFriends(bool state)
    {
        isToUpdateFriends = state;
    }

    public bool islogged(bool flag)
    {
        return isLogged = flag;
    }


    public void InitIsItemFound()
    {
        for (int i = 0; i < itemsName.Count; i++)
        {
            isItemFound[i] = false;
        }
    }

    public void ItemFound(int i)
    {
        isItemFound[i] = true;
    }

    public void ItemnotFound(int i)
    {
        isItemFound[i] = false;
    }

    public bool IsItemFound(int i)
    {
        return isItemFound[i];
    }

    private void addItem(int index)
    {
        isreceiveitem = true;
        receiveitemnum = index;
    }


    public static void Request()
    {
        while (true)
        {
            Thread.Sleep(500);
            try
            {
                PlayerMovements tempMove = instance.m_Players[instance.getPlayernum()].getMovement();
                PlayerRotateView tempRoate = instance.m_Players[instance.getPlayernum()].getRoate();
               

                Vector3 pos = tempMove.nowpos;
                Position sendpos = new Position();
                sendpos.X = pos.x;
                sendpos.Y = pos.y;
                sendpos.Z = pos.z;
                sendpos.W = tempRoate.roate.y;

                if (Loaded)
                {
                    m_state = -1;
                    Debug.Log("update state");
                }
                Debug.Log("whiletrue");
                if (tempMove.isjumping) m_state = 1;
                // Client.Instance.updatePlayerMovement(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayernum(), movement, m_state);
                Client.Instance.updatePlayerPosition(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayernum(), sendpos, m_state);
                Debug.Log(GameManagers.Instance.getRoomNum());
                Client.Instance.getRoom(GameManagers.Instance.getRoomNum());
                
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }
    }

  
    public static void receiveAndHandle()
    {
        while (true)
        {
            Thread.Sleep(10);
            //Client.Instance.getRoom(GameManagers.Instance.getRoomNum());
            try
            {
                ToSend res = Client.Instance.getServerRespone();
                if (res != null)
                {
                    string type = res.Type;
                    string result = res.Result;
                    Debug.Log("Client:" + type);
                    if (type == "getfriends")
                    {
                        if (result == "s")
                        {
                            string friends = res.User.Username;
                            string[] friendslist2 = friends.Split(';');
                            List<String> username_temp = new List<string>();
                            for (int i = 0; i < friendslist2.Length; i++)
                            {
                                username_temp.Add(friendslist2[i]);
                            }
                            GameManagers.Instance.setFriend(username_temp);
                        }
                    }
                    else if (type == "findfriend")
                    {
                        isExist = true;
                    }
                    else if (type == "Tgetfriends")
                    {
                        isToUpdateFriends = true;
                    }
                    else if (type == "addfriend")
                    {

                    }
                    else if (type == "getMaxLevel")
                    {
                        if (result == "s")
                        {
                            string maxLevel = res.User.Password;
                        }
                    }
                    else if (type == "getFriendState")
                    {

                    }
                    else if (type == "newRoom")
                    {
                        int roomNum = res.Room.RoomNum;
                        isRoomReady = true;
                        GameManagers.Instance.setRoomNum(roomNum);
                    }
                    else if (type == "addPlayer")
                    {
                        /*服务器未完善监测
                        int playerNum = res.Room.RoomNum;
                        GameManagers.Instance.setPlayernum(playerNum);
                        PlayerNum = playerNum;*/
                    }
                    else if (type == "startGame")
                    {

                    }
                    else if (type == "startGaming")
                    {
                        GameManagers.Instance.changeGameState(true);
                    }   
                    else if (type == "updateRoomPlayers")
                    {
                        Room room = res.Room;
                        GameManagers.instance.setRoomNum(res.Room.RoomNum);
                        GameManagers.Instance.setRoom(room);
                        Debug.Log(type + " " + "player count" + " " + res.Room.Players.Count);
                        for (int i = 0; i < room.Players.Count; i++)
                        {
                            if(room.Players[i].Name == GameManagers.Instance.getPlayerid())
                            {
                                GameManagers.Instance.setPlayernum(i);                                
                            }
                            Debug.Log("player" + i + "is" + room.Players[i].Name);
                        }
                    }
                    else if (type == "deletePlayer")
                    {
                        Debug.Log("Sb leave");
                    }
                    else if (type == "inviteFriend")
                    {

                    }
                    else if (type == "invitation")
                    {
                        string whoInvite = res.User.Username;
                        int roomNum = res.Room.RoomNum;
                        Debug.Log(whoInvite + " invites you to room:" + roomNum);
                        GameManagers.Instance.roomnumber = roomNum;
                        GameManagers.Instance.kindofask = 2;
                        GameManagers.Instance.askornot = 1;
                        GameManagers.Instance.inviteuser = whoInvite;
                        isRoomReady = true;
                    }
                    else if (type == "setRoomLevel")
                    {

                    }
                    else if (type == "updatePlayerPosition")
                    {
                        Room room = res.Room;
                        GameManagers.Instance.setRoom(room);
                        Debug.Log(type + " " + "player count" + " " + room.Players.Count);
                        for (int i = 0; i < room.Players.Count; i++)
                        {
                            if (room.Players[i].Name == GameManagers.Instance.getPlayerid())
                            {
                                 GameManagers.Instance.setPlayernum(i);
                            }
                            Debug.Log("player" + i + "is" + room.Players[i].Name);
                        }
                    }
                    else if (type == "addFriendRequest")
                    {
                        string whoRequest = res.User.Username;
                        string me = res.User.Password;
                        GameManagers.Instance.kindofask = 1;
                        GameManagers.Instance.askornot = 1;
                        GameManagers.Instance.inviteuser = whoRequest;

                    }
                    else if (type == "confirmAddFriend")
                    {

                    }
                    
                    else if (type == "ReceiveItem")
                    {
                        string from = res.User.Username;
                        int itemNum = res.Room.RoomNum;
                        //do something
                        Instance.ItemFound(itemNum);
                        Instance.addItem(itemNum);                       

                        Debug.Log("receive item: " + itemNum + " from user: " + from);
                    }
                    else
                    {
                        Debug.Log(type);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}
