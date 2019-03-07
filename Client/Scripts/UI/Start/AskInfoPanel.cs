using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AskInvitePanel : PanelBase {
    private Text inviteinfo;
    private Button no;
    private Button yes;

    #region
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "AskInvitePanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        inviteinfo = GameObject.Find("InviteInfo").GetComponent<Text>();
        no = GameObject.Find("CancelBtn").GetComponent<Button>();
        yes = GameObject.Find("ConfirmBtn").GetComponent<Button>();

        if(GameManagers.Instance.kindofask == 1)
        {
            inviteinfo.text = GameManagers.Instance.inviteuser + "请求添加您为好友";
        }
        if(GameManagers.Instance.kindofask == 2)
        {
            inviteinfo.text = GameManagers.Instance.inviteuser + "邀请您加入一局游戏";
        }

        no.onClick.AddListener(OnNoClick);
        yes.onClick.AddListener(OnYesClick);

    }

    #endregion

    public void OnNoClick()
    {
        GameManagers.Instance.kindofask = 0;
        GameManagers.Instance.inviteuser = "空";
        Close();
    }

    public void OnYesClick()
    {
        if (GameManagers.Instance.kindofask == 2)
        {
            GameManagers.Instance.setRoomNum(GameManagers.Instance.roomnumber);
            PanelMgr.instance.ClosePanel("HomePanel");
            PanelMgr.instance.OpenPanel<RoomPanel>("");
            GameManagers.Instance.kindofask = 0;
            GameManagers.Instance.inviteuser = "空";
            //Client.Instance.addPlayer(GameManagers.Instance.roomnumber, GameManagers.Instance.getPlayerid());
            Close();
        }
        if(GameManagers.Instance.kindofask == 1)
        {
            Client.Instance.confirmAddFriend(GameManagers.Instance.getPlayerid(), GameManagers.Instance.inviteuser);
            GameManagers.Instance.kindofask = 0;
            GameManagers.Instance.inviteuser = "空";
            Close();
        }
        

    }

}
