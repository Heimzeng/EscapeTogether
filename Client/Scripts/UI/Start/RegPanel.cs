using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegPanel : PanelBase {
    private InputField idInput;
    private InputField pwInput;
    private Button regBtn;
    private Button closeBtn;
    private string id;
    private string pw;

    #region
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RegPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();

        regBtn.onClick.AddListener(OnRegClick);
        closeBtn.onClick.AddListener(OnCloseClick);
    }

    #endregion

    public void OnRegClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            Debug.Log("用户名和密码不能为空");
            return;
        }
        id = idInput.text;
        pw = pwInput.text;
        if (Client.Instance.regist(id, pw))
        {
            Debug.Log(id);
            PanelMgr.instance.OpenPanel<LoginPanel>("");
            Debug.Log("Regist successfunlly.");
            Close();
        }
        else
        {
            Debug.Log("Fail to regist");
        }


    }
    public void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

}
