using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TitlePanel : PanelBase {

    private Button startBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "TitlePanel";
        layer = PanelLayer.Panel;

    }
    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        startBtn = GameObject.Find("startBtn").GetComponent<Button>();

        startBtn.onClick.AddListener(OnStartClick);
    }
    #endregion
    public void OnStartClick()
    {
        //开始游戏
        //StartCoroutine(GameManagers.instance.LoadScence()); 
        PanelMgr.instance.OpenPanel<HomePanel>("");
        Close();
    }


}
