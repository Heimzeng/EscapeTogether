using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManage : MonoBehaviour {

    public static PanelManage Instance { get; set; }
    public GameObject Panel;
    public GameObject KanbanPanel;
    public GameObject ReijiPanel;
    public GameObject KenPanel;
    public GameObject Boom;
    
    private int drawSpeed = 0;

    void Start()
    {
        Boom = GameObject.Find("Boomshift");
        Boom.SetActive(false);
        Panel.SetActive(false);
        KanbanPanel.SetActive(true);
        ReijiPanel.SetActive(false);
        KenPanel.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OnGUI()
    {
        float time = 0;
        if (Boom.active)
        {
            time += Time.deltaTime;

            Texture black = Texture2D.blackTexture;
            GUI.DrawTexture(new Rect(Screen.width / 5, 200, Screen.width / 5 * 4, 30), black);
            if (time >= 0.01f)
            {
                time = 0;
                if (!Input.GetMouseButton(1))
                    drawSpeed++;
                Debug.Log("drawingrect" + drawSpeed);
                Texture probar = Texture2D.whiteTexture;
                if (5 * drawSpeed >= Screen.width / 5 * 3)
                    drawSpeed = 0;

                GUI.DrawTexture(new Rect(124.5f + 5 * drawSpeed, 205, 10, 45), probar);
            }
        }
    }

    public void KToR()
    {
        KanbanPanel.SetActive(false);
        ReijiPanel.SetActive(true);
    }

    public void RToK()
    {
        ReijiPanel.SetActive(false);
        KenPanel.SetActive(true);

    }

    public void KToNull()
    {
        KenPanel.SetActive(false);
        if (GameManagers.Instance.charactor == "Ken")
            GameObject.Find("KenJump").GetComponent<ETCButton>().visible = true;

        GameObject.Find(GameManagers.Instance.charactor + "Joystick").GetComponent<ETCJoystick>().visible = true;
        GameManagers.instance.EnablePlayers();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ContinueShow();
        }
    }


    public void Show()
    {
        Panel.SetActive(true);
    }

    public void ContinueShow()
    {
        Panel.SetActive(false);
    }

}
