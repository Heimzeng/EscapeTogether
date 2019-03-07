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

public class ImageShow : MonoBehaviour, IPointerClickHandler
{
    public int position;

    public void OnPointerClick(PointerEventData eventData)
    {
        // OnClick code goes here ...
        Debug.Log("Image " + position +  " Click");
        //Inventory.Instance.RemoveItemAt(position);
        List<Sprite> spritesToAdd = new List<Sprite>();
        spritesToAdd.Add(Inventory.Instance.GetSpriteeAt(position));
        ImageShowingSystem.Instance.AddNewImageShow(spritesToAdd);
    }
}
