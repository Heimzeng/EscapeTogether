using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel :PanelBase {
    private Text roomnumber;
    private Dropdown stagelist;
    private Text p1username;
    private Text p2username;

    private Dropdown friendslist2;
    private Button inviteBtn;
    private Button startBtn;
    private Button chooseBtn;
    private Button returnbefore;
    private int start = 0;
    private int invite = 0;
    Thread listeningThread;


    bool oneTime = true;
    bool oneTimeForRoomNum = true;

    private int maxstage = 1;
    private int choosestage = 1;
    private string inviteusername = "空";

    private string p2username_temp = "空";

    private static List<string> username_temp2 = new List<string>();

    #region
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
        
        username_temp2 = new List<string>();
        username_temp2.Add("me");
    }
    public override void OnShowing()
    {
        base.OnShowing();    
        
        Transform skinTrans = skin.transform;
        roomnumber = GameObject.Find("RoomNumber").GetComponent<Text>();
        stagelist = GameObject.Find("StageList").GetComponent<Dropdown>();
       
        p1username = GameObject.Find("P1username").GetComponent<Text>();
       
        p2username = GameObject.Find("P2username").GetComponent<Text>();
      
        friendslist2 = GameObject.Find("FriendsList2").GetComponent<Dropdown>();
        inviteBtn = GameObject.Find("Invite").GetComponent<Button>();
        startBtn = GameObject.Find("Start").GetComponent<Button>();
        chooseBtn = GameObject.Find("Choose").GetComponent<Button>();
        returnbefore = GameObject.Find("Return").GetComponent<Button>();

        p1username.text = GameManagers.instance.getPlayerid();

        inviteBtn.onClick.AddListener(OnInviteClick);
        startBtn.onClick.AddListener(OnStartClick);
        chooseBtn.onClick.AddListener(OnChooseClick);
        returnbefore.onClick.AddListener(OnReturnClick);

       
    }
 
    #endregion


    public void OnInviteClick()
    {
        //获取当前好友列表选中的的username
        List<Dropdown.OptionData> droplist = friendslist2.options;
        int count = friendslist2.value;
        Dropdown.OptionData data = droplist[count];
        inviteusername = data.text;
        Debug.Log(data.text);
        Client.Instance.inviteFriend(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayerid(), data.text);
        invite = 1;

    }

    

    public void OnChooseClick()
    {
        //获取关卡数
        List<Dropdown.OptionData> droplist = stagelist.options;
        int count = stagelist.value;
        Dropdown.OptionData data = droplist[count];
        choosestage = count+1;
        Debug.Log(choosestage);
        Client.Instance.setRoomLevel(GameManagers.Instance.getRoomNum(), choosestage);
    }

    public void OnStartClick()
    {
        Client.Instance.startGame(GameManagers.Instance.getRoomNum());
        if (p1username.text != "空" && p2username.text != "空")
        {
            GameManagers.Instance.changeGameState(true);
            StartCoroutine(GameManagers.instance.LoadScence());
        }
        else
        {
            Debug.Log("人数不足");
            GameManagers.Instance.changeGameState(true);
        }

    }

    public void OnReturnClick()
    {
        PanelMgr.instance.OpenPanel<HomePanel>("");
        Client.Instance.deletePlayer(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayernum());
        Close();
    }

    public override void Update()
    {
        if(start == 0) 
            Client.Instance.getRoom(GameManagers.Instance.getRoomNum());
        start++;
        if (start == 100)
            start = 0;

        if (GameManagers.Instance.GetIsRoomReady())
        {
            if (oneTime)
            {
                oneTime = false;
                Debug.Log("add");
                Client.Instance.addPlayer(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayerid());
            }
        }      
        //从服务器获取好友列表和最大关卡数，用username_temp和maxstage去存。

        if (GameManagers.Instance.getFriend() != null)
        {
            username_temp2 = GameManagers.Instance.getFriend();
        }
        friendslist2.ClearOptions();
        friendslist2.AddOptions(username_temp2);
        stagelist.ClearOptions();
        List<string> maxstage_temp = new List<string>();
        for(int i = 1; i <= maxstage; i++)
        {
            maxstage_temp.Add("第" + i + "关");
        }
        stagelist.AddOptions(maxstage_temp);

        
        roomnumber.text = GameManagers.Instance.getRoomNum() + "";


        if (GameManagers.Instance.GetIsRoomReady())
        {
            Room thisRoom = GameManagers.Instance.GetRoom();
            Debug.Log(thisRoom.Players.Count + "player in this room" + thisRoom.RoomNum);
            Debug.Log(GameManagers.Instance.getRoomNum());
            int min = thisRoom.Players.Count >= 4 ? 4 : thisRoom.Players.Count;
            Debug.Log("Client be invited: player count :" + thisRoom.Players.Count);
            for (int i = 0; i < 2; i++)
            {
                int k = i + 1;
                if(i < min)
                    GameObject.Find("P" + k + "username").GetComponent<Text>().text = thisRoom.Players[i].Name;
                else
                    GameObject.Find("P" + k + "username").GetComponent<Text>().text = "空";

            }

            //GameManagers.Instance.SetIsRoomReady(false);
        }
        
        if (GameManagers.Instance.getGameState())
        {
            //跳转到游戏界面
            StartCoroutine(GameManagers.instance.LoadScence());
        }

        /*
        if (p2username.text == "空")
        {
            // 请求服务器给我列表函数。
            // p2username_temp = xxx;
            p2username.text = p2username_temp;
        }

        if(start == 1)
        {
            //告诉server我开始游戏了，让server广播给房间内的玩家
            
        }

        if(invite == 1)
        {
            
            //告诉server我邀请了某个玩家
        }
        */
    }
}
