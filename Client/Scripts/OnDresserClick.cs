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
using UnityEngine.EventSystems;
public class OnDresserClick : MonoBehaviour
{
    public float moveX;
    public float moveY;
    public float moveZ;

    bool isOpen = false;

    public void DresserMove()
    {
        Debug.Log("Dress click " + gameObject.name);
        if (isOpen)
        {
            Vector3 position = gameObject.transform.position;
            position[0] -= moveX;
            position[1] -= moveY;
            position[2] -= moveZ;
            Quaternion quaternion = new Quaternion();
            gameObject.transform.SetPositionAndRotation(position, quaternion);
            //gameObject.transform.Translate(position);
            isOpen = false;
        }
        else
        {
            Vector3 position = gameObject.transform.position;
            position[0] += moveX;
            position[1] += moveY;
            position[2] += moveZ;
            Quaternion quaternion = new Quaternion();
            gameObject.transform.SetPositionAndRotation(position, quaternion);
            //gameObject.transform.Translate(position);
            isOpen = true;
            if (gameObject.name == "case_02")
            {
                GameObject.Find("PanelManager").GetComponent<PanelManage>().Boom.SetActive(true);
                GameObject.Find("Furniture").transform.GetChild(1).gameObject.SetActive(true);
            }
                
        }
        
        
        //Destroy(gameObject);
    }

}
