using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PanelLayer
{
    Panel,
    Tips

}
public class PanelMgr : MonoBehaviour {

    public static PanelMgr instance;

    private GameObject Canvas;
    public Dictionary<string, PanelBase> dict;
    public Dictionary<PanelLayer, Transform> layerDict;

    //初始化
    public void Awake()
    {
        instance = this;
        dict = new Dictionary<string, PanelBase>();
        InitLayer();
    }
    private void InitLayer()
    {
        Canvas = GameObject.Find("Canvas");
        if(Canvas == null)
        {
            Debug.Log("Canvas is Null");
        }
        layerDict = new Dictionary<PanelLayer, Transform>();
        foreach(PanelLayer pl in Enum.GetValues(typeof(PanelLayer))){
            string name = pl.ToString();
            Transform trans = Canvas.transform.Find(name);
            layerDict.Add(pl, trans);
        }
    }

    


    public void OpenPanel<T>(string skinPath,params object[] args) where T : PanelBase
    {
        string name = typeof(T).ToString();
        if (dict.ContainsKey(name))
            return;
        PanelBase panel = Canvas.AddComponent<T>();
        panel.Init(args);
        dict.Add(name, panel);
        //皮肤
        skinPath = (skinPath != "" ? skinPath : panel.skinPath);
        GameObject skin = Resources.Load<GameObject>(skinPath);
        if (skin== null)
            Debug.Log("skin missed");
        panel.skin = (GameObject)Instantiate(skin);

        Transform skinTrans = panel.skin.transform;
        PanelLayer layer = panel.layer;
        Transform parent = layerDict[layer];
        skinTrans.SetParent(parent,false);

        panel.OnShowing();

        panel.OnShowed();

    }

    public void ClosePanel(string name)
    {
        PanelBase panel = (PanelBase)dict[name];
        if (panel == null)
           return;

        panel.OnClosing();
        dict.Remove(name);
        panel.OnClosed();
        GameObject.Destroy(panel.skin);
        Component.Destroy(panel);


    }

}
