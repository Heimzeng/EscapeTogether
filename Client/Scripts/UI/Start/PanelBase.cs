using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour {

    public string skinPath;
    public PanelLayer layer;
    public GameObject skin;

    public object[] args;
    #region 面板的生命周期
    public virtual void Init(params object[] args)
    {
        this.args = args;
    }
    public virtual void OnShowing()
    {

    }
    public virtual void OnShowed()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void OnClosing()
    {

    } 
    public virtual void OnClosed()
    {

    }
    #endregion

    protected virtual void Close()
    {
        string name = this.GetType().ToString();
        PanelMgr.instance.ClosePanel(name);
    }

}
