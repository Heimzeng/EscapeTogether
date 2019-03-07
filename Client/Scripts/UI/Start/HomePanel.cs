using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : PanelBase {
    private Button creatroombtn;
    private static Dropdown friendslist;
    private Text username;
    private InputField addusername;
    private Button add;

    private int n = 3;
    private static List<string> username_temp = new List<string>();

    bool oneTime = true;

    Thread listeningThread;

    #region
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "HomePanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        if (oneTime)
        {
            Client.Instance.GetFriends(GameManagers.Instance.getPlayerid());
        }

        

        Transform skinTrans = skin.transform;
        creatroombtn = GameObject.Find("CreatRoomBtn").GetComponent<Button>();
        username = GameObject.Find("Username").GetComponent<Text>();
        friendslist = GameObject.Find("FriendsList").GetComponent<Dropdown>();
        addusername = GameObject.Find("Addusername").GetComponent<InputField>();
        add = GameObject.Find("Add").GetComponent<Button>();


        username.text = GameManagers.instance.getPlayerid();
        creatroombtn.onClick.AddListener(OnCreatRoomClick);
        add.onClick.AddListener(OnAddClick);

        //string empty = "空";//测试
        //username_temp.Add("456");
        //username_temp.Add(empty);
        //username_temp.Add(empty);

        //Update();

    }
    #endregion
    public void OnCreatRoomClick()
    {
        Client.Instance.newRoom();
        PanelMgr.instance.OpenPanel<RoomPanel>("");
        Close();
    }
    public void OnAddClick()
    {
        //把inputfield里面的username发给服务器，添加好友。
        GameManagers.Instance.SetIsExist(false);
        Client.Instance.findFriend(addusername.text);
        Thread.Sleep(100);
        if (GameManagers.Instance.GetIsExist())
        {
            Client.Instance.addFriend(GameManagers.Instance.getPlayerid(), addusername.text);
        }
        else
        {
            Debug.Log("user not exist.");
        }
        
        
    }
    public override void Update()
    {
        //从服务器中接收到好友列表（username_temp)
        username_temp = GameManagers.Instance.getFriend();
        friendslist.ClearOptions();
        friendslist.AddOptions(username_temp);
        if(GameManagers.Instance.askornot != 0){
            PanelMgr.instance.OpenPanel<AskInvitePanel>("");
            GameManagers.Instance.askornot = 0;
        }

        if (GameManagers.Instance.GetIsToUpdateFriends())
        {
            Client.Instance.GetFriends(GameManagers.Instance.getPlayerid());
            GameManagers.Instance.SetIsToUpdateFriends(false);
        }
    }


}
