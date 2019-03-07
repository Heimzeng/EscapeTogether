using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MObject : Interactable {
    public string[] dialog;
    public string[] dialog2;
    public string[] dialog3;
    public string[] dialog4;

    public string name;
    public int relativeItemIndex;
    public bool isKeyItem = false;
    public bool isEnd = false;
    public Item relativeItem;
    public bool isDone = false;
    public bool isLockBox = false;

    public override void Interact()
    {
        if (isLockBox)
        {
            if(gameObject.name == "left")
            {
                PanelManage.Instance.Panel.gameObject.GetComponent<LockPanel>().hint = "请输入三位密码";
            }
            else if(gameObject.name == "right")
            {
                PanelManage.Instance.Panel.gameObject.GetComponent<LockPanel>().hint = "请输入六位密码";
            }
            else if(gameObject.name == "middle")
            {
                PanelManage.Instance.Panel.gameObject.GetComponent<LockPanel>().hint = "请输入八位密码";
            }
            PanelManage.Instance.Show();
        }
        else if(isDone)
        {
            if (GameManagers.Instance.charactor == "Ken")
            {
                DialogSystem.Instance.AddNewDialog(dialog3, name);
            }

            if (GameManagers.Instance.charactor == "Reji")
            {
                DialogSystem.Instance.AddNewDialog(dialog4, name);
            }
        }
        else if (isKeyItem)
        {
            if (GameManagers.Instance.IsItemFound(relativeItemIndex))
            {
                //播放物品动画

                //通知服务器
                if(gameObject.tag == "KeyItems")
                {
                    Client.Instance.UpdateItem(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayerid(), gameObject.GetComponent<MItem>().id);
                    
                }
                //删除Item
                Inventory.Instance.RemoveItem(relativeItem);
                isDone = true;
                if (GameManagers.Instance.charactor == "Ken")
                {
                    DialogSystem.Instance.AddNewDialog(dialog3, name);
                }

                if (GameManagers.Instance.charactor == "Reji")
                {
                    DialogSystem.Instance.AddNewDialog(dialog4, name);
                }
                if (isEnd)
                {
                    GameManagers.Instance.EndGame();
                }
            }
            else
            {
                if (GameManagers.Instance.charactor == "Ken")
                {
                    DialogSystem.Instance.AddNewDialog(dialog, name);
                }

                if (GameManagers.Instance.charactor == "Reji")
                {
                    DialogSystem.Instance.AddNewDialog(dialog2, name);
                }
            }
        }
        else
        {
            if (GameManagers.Instance.charactor == "Ken")
            {
                DialogSystem.Instance.AddNewDialog(dialog, name);
            }

            if (GameManagers.Instance.charactor == "Reji")
            {
                DialogSystem.Instance.AddNewDialog(dialog2, name);
            }
        }
        
    }


}
