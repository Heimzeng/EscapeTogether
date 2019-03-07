using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tutorial;

public class LoginPanel : PanelBase {
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;
    private string id;
    private string pw;
    #region
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelLayer.Panel;
    }
    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
    }
    #endregion

    public void OnLoginClick()
    {
        if(idInput.text == "" || pwInput.text == "")
        {
            Debug.Log("用户名和密码不能为空");
            return;
        }
        id = idInput.text;
        pw = pwInput.text;

        if (Client.Instance.login(id, pw))
        {
            PanelMgr.instance.OpenPanel<TitlePanel>("");
            GameManagers.instance.islogged(true);
            GameManagers.instance.setPlayerid(id);
            GameManagers.Instance.listeningThread.Start();
            Close();
        }
        else
        {
            Debug.Log("Fail to login");
            GameManagers.instance.islogged(false);
        }
        
    }
    public void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        Close();
    }
}
