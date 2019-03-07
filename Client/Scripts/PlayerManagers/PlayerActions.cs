using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class PlayerActions : MonoBehaviour {
    public List<string> m_bag;
    public string m_PlayerInput;
    private string m_playerid;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && m_PlayerInput == "Self")
        {
            takeAction();
        }
    }

    void takeAction()
    {
        Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionInfo;
        if (IsPointerOverUIObject().Count == 0)
        {
            
            if (Physics.Raycast(interactionRay, out interactionInfo))
            {
                GameObject interactedObject = interactionInfo.collider.gameObject;
                Debug.Log(interactedObject.name);
                Debug.Log(interactedObject.tag);
                if (interactedObject.tag == "Interactable Object")
                {
                    interactedObject.GetComponent<Interactable>().MoveTointeraction();
                }
                else if (interactedObject.tag == "TipItem")
                {
                    Debug.Log(interactedObject.name);
                    updateBag(interactedObject.name);
                    interactedObject.name = "Fuck the Door";
                }
                else if (interactedObject.tag == "dresser")
                {
                    interactedObject.GetComponent<OnDresserClick>().DresserMove();
                }
                else if (interactedObject.tag == "moveAndTalk")
                {
                    interactedObject.GetComponent<OnDresserClick>().DresserMove();
                    interactedObject.GetComponent<Interactable>().MoveTointeraction();
                }
                else if (interactedObject.tag == "KeyItems")
                {
                    interactedObject.GetComponent<Interactable>().MoveTointeraction();
                    //代码冗余，后期修改，isdone！sb
                    if (GameManagers.Instance.IsItemFound(interactedObject.GetComponent<MObject>().relativeItemIndex))
                    {
                        interactedObject.GetComponent<OnDresserClick>().DresserMove();
                    }
                }
                else if (interactedObject.tag == "UseDeleteAndGet")
                {
                    interactedObject.GetComponent<MObject>().MoveTointeraction();
                    if (GameManagers.Instance.IsItemFound(interactedObject.GetComponent<MObject>().relativeItemIndex))
                    {
                        interactedObject.GetComponent<MItem>().MoveTointeraction();
                    }
                }
                else if (interactedObject.tag == "player")
                {
                    if (Inventory.Instance.seletedIndex >= 0)
                    {
                        int itemid = Inventory.Instance.seletedIndex;
                        string friendid = interactedObject.GetComponent<PlayerActions>().getid();
                        Client.Instance.giveItem(GameManagers.Instance.getRoomNum(), getid(), friendid, itemid);
                        Inventory.Instance.RemoveItem(ImageShowingSystem.Instance.items[itemid]);
                        GameManagers.Instance.ItemnotFound(itemid);
                        Inventory.Instance.seletedIndex = -1;
                    }
                }
            }
            else
            {
                Debug.Log("Not Hit");
            }
        }
        else
        {
            Debug.Log(IsPointerOverUIObject()[0].gameObject.name);
        }

     }

    //判断点击的是否是UI
    private List<RaycastResult> IsPointerOverUIObject() 
    {
        PointerEventData EDCPosition = new PointerEventData(EventSystem.current);
        EDCPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(EDCPosition, results);
        return results;
    }


    public string getid()
    {
        return m_playerid;
    }

    public  void setid(string id)
    {
        m_playerid = id;
    }

    public void updateBag(string tipitem)
    {
        m_bag.Add(tipitem);
    }


}
