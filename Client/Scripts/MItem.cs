using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Tutorial;
using Google.Protobuf;

public class MItem : Interactable {
    public string itemName;
    public string[] description;
    public int id;
    public bool oneTime;

    public Item item;
    private Inventory inventory;

    Thread listeningThread;
    Thread newThread;

    void Start()
    {
        oneTime = true;
        inventory = FindObjectOfType<Inventory>();
    }

    public override void Interact()
    {
        inventory.AddItem(item);
        Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), id);
        GameManagers.Instance.ItemFound(id);
        Destroy(gameObject);
        if(id == 2 || id == 4)
        {
            /*if (GameManagers.Instance.IsItemFound(2) && GameManagers.Instance.IsItemFound(4))
            {
                inventory.RemoveItem(ImageShowingSystem.Instance.items[2]);
                inventory.RemoveItem(ImageShowingSystem.Instance.items[4]);
                Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), 2);
                Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), 4);
                string[] des = new string[2];
                des[0] = "使用锤子砸碎玻璃球";
                des[1] = "获得纸条一张";
                inventory.AddItem(ImageShowingSystem.Instance.items[5]);
                GameManagers.Instance.ItemFound(5);
                int l = description.Length;
                string[] toShow = new string[l + 2];
                for (int i = 0; i < l + 2; i++)
                {
                    if (i < l)
                    {
                        toShow[i] = description[i];
                    }
                    else
                    {
                        toShow[i] = des[i - l];
                    }
                }
                if (GameManagers.Instance.IsItemFound(3) && GameManagers.Instance.IsItemFound(5))
                {
                    inventory.RemoveItem(ImageShowingSystem.Instance.items[3]);
                    inventory.RemoveItem(ImageShowingSystem.Instance.items[5]);
                    Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), 3);
                    Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), 5);
                    string[] des2 = new string[2];
                    des2[0] = "两张纸条拼在一起";
                    des2[1] = "获得完整的纸条";
                    inventory.AddItem(ImageShowingSystem.Instance.items[6]);
                    GameManagers.Instance.ItemFound(6);
                    int l2 = toShow.Length;
                    string[] toShow2 = new string[l2 + 2];
                    for (int i = 0; i < l2 + 2; i++)
                    {
                        if (i < l2)
                        {
                            toShow2[i] = toShow[i];
                        }
                        else
                        {
                            toShow2[i] = des2[i - l2];
                        }
                    }
                    DialogSystem.Instance.AddNewDialog(toShow2, itemName);
                }
                else
                {
                    DialogSystem.Instance.AddNewDialog(toShow, itemName);
                }

            }*/
        }
        else if(id == 3 || id == 5)
        {
            /*if (GameManagers.Instance.IsItemFound(3) && GameManagers.Instance.IsItemFound(5))
            {
                inventory.RemoveItem(ImageShowingSystem.Instance.items[3]);
                inventory.RemoveItem(ImageShowingSystem.Instance.items[5]);
                Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), 3);
                Client.Instance.UpdateItem(GameManagers.instance.getRoomNum(), GameManagers.instance.getPlayerid(), 5);

                string[] des = new string[2];
                des[0] = "两张纸条拼在一起";
                des[1] = "获得完整的纸条";
                inventory.AddItem(ImageShowingSystem.Instance.items[6]);
                GameManagers.Instance.ItemFound(6);
                int l = description.Length;
                string[] toShow = new string[l + 2];
                for(int i = 0; i < l + 2; i++)
                {
                    if (i < l)
                    {
                        toShow[i] = description[i];
                    }
                    else
                    {
                        toShow[i] = des[i - l];
                    }
                }
                DialogSystem.Instance.AddNewDialog(toShow, itemName);

            }*/
        }
        else
        {
            DialogSystem.Instance.AddNewDialog(description, itemName);
        }
        
        //oneTime = false;
        //bool registres = Client.Instance.regist("heim3", "123456");

        //Debug.Log(registres);
        //string[] friends = Client.Instance.GetFriends("heim3");
        //bool isExist = Client.Instance.findFriend("heim2");
        //Debug.Log("has this name?"+isExist);
        //isExist = Client.Instance.findFriend("heim4");
        //Debug.Log("has this name?" + isExist);

        //bool addres = Client.Instance.addFriend("heim", "heim2");
        //bool addres2 = Client.Instance.addFriend("heim", "heim4");
        //Debug.Log("add su?" + addres);
        //Debug.Log("add su?" + addres2);

        //string maxLevel = Client.Instance.getMaxLevel("heim");
        //Debug.Log("maxlevel:" + maxLevel);

        //Debug.Log("heim's state is :" + Client.Instance.getFrendState("heim"));
        //Debug.Log("heim3's state is :" + Client.Instance.getFrendState("heim3"));
        /*
        int roomNum = Client.Instance.newRoom();
        int playerNum = Client.Instance.addPlayer(roomNum, "heim3");

        Debug.Log(roomNum);
        Position position = new Position();
        position.X = 1;
        position.Y = 1;
        position.Z = 1;
        position.W = 1;
        List<Player> players = Client.Instance.updatePlayerPosition(roomNum, playerNum, position, 1);

        Debug.Log(players[playerNum].Position);
        */
        //Debug.Log(Client.Instance.setRoomLevel(roomNum, 1));
        //Debug.Log(Client.Instance.setRoomLevel(roomNum, 2));


        //bool isRemoved = Client.Instance.deletePlayer(roomNum, playerNum);
        //Debug.Log(isRemoved);

        //Client.Instance.inviteFriend(0, "heim3", "heim3");
        //Client.Instance.inviteFriend(0, "heim3", "123");

        //Client.Instance.deletePlayer(roomNum, playerNum);

        //Destroy(gameObject);
    }
}
