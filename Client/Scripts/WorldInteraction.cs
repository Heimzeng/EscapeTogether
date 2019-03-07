using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldInteraction : MonoBehaviour
{
    [HideInInspector]
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("touch area is UI");
            }
            else
            {
                GetInteration();
            }
            
        }
    }

    void GetInteration()
    {
        Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionInfo;
        if (Physics.Raycast(interactionRay, out interactionInfo))
        {
            Debug.Log(interactionInfo.collider.name);
            GameObject interactedObject = interactionInfo.collider.gameObject;
            if (interactedObject.tag == "Interactable Object")
            {
                interactedObject.GetComponent<Interactable>().MoveTointeraction();
                //Debug.Log("Click an Object");
                //Debug.Log(interactedObject.name);
            }
            else if (interactedObject.tag == "ItemImage")
            {
                Debug.Log("ItemImage");
            }
            else if(interactedObject.tag == "1")
            {
                interactedObject.GetComponent<OnDresserClick>().DresserMove();
                Debug.Log("dresser click");
            }
            else if(interactedObject.tag == "player")
            {
                if(Inventory.Instance.seletedIndex >= 0)
                {
                    Client.Instance.giveItem(GameManagers.Instance.getRoomNum(), GameManagers.Instance.getPlayerid(), GameManagers.Instance.getPlayerid(), Inventory.Instance.seletedIndex);
                }
            }
            else
            {
                Debug.Log("Tag not match");

            }
        }
        else
        {
            Debug.Log("Not Hit");
        }
    }
}
