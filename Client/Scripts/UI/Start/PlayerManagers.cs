using System;
using UnityEngine;
using System.Collections.Generic;
[Serializable]
public class PlayerManagers {
    public Transform m_SpawnPoint;   //The spawnpoint of player
   [HideInInspector] public string m_PlayerID;  //The ID of the player
   [HideInInspector] public GameObject m_Instance;
   [HideInInspector] public int m_PlayerNum;   //The number of the player
   [HideInInspector] public string m_Holding;   //The tip that the player chosen
   [HideInInspector] public int lifes = 3;   //The health value of the player
   [HideInInspector] public string m_Inputname; 


    private PlayerMovements m_Movement;
    private PlayerActions m_Action;
    private PlayerRotateView m_RotateView;
    private PlayerRotateView m_Camera;
    private List<string> m_Bag;

    
    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<PlayerMovements>();
        m_Action = m_Instance.GetComponent<PlayerActions>();
        m_RotateView = m_Instance.GetComponent<PlayerRotateView>();
        m_Camera = m_Instance.GetComponentInChildren<CameraControl>().gameObject.GetComponent<PlayerRotateView>();

        m_Movement.m_PlayerInput = m_Inputname;
        m_Action.m_PlayerInput = m_Inputname;
        m_Action.setid(m_PlayerID);
        m_RotateView.m_PlayerInput = m_Inputname;
        m_Camera.m_PlayerInput = m_Inputname;
    }
    
    public void unableAction()
    {
        m_Instance.GetComponent<PlayerActions>().enabled = false;
    }

    //update the bag of player
    public void updateBag(string gettip)
    {
        m_Bag.Add(gettip);
    }
    
    //give the tip to other player
    public void GiveTip2PlaywithID(string m_Holding)
    {
        m_Bag.Remove(m_Holding);
    }

    public void KilltheCamera()
    {
        m_Instance.GetComponentInChildren<Camera>().enabled = false;
    }
	
	public PlayerMovements getMovement()
    {
        return m_Movement;
    }

    public PlayerRotateView getRoate()
    {
        return m_RotateView;
    }

    public void PlayerDisable()
    {
        m_Movement.enabled = false;
        m_RotateView.enabled = false;
        m_Action.enabled = false;
        m_Camera.m_PlayerInput = "others";
    }

    public void PlayerEnable()
    {
        m_Movement.enabled = true;
        m_RotateView.enabled = true;
        m_Action.enabled = true;
        m_Camera.m_PlayerInput = "Self";
    }
}
