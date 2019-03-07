using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMangeMent : MonoBehaviour
{

    public static PanelMangeMent Instance { get; set; }
    public GameObject Panel;

    void Start()
    {
        Panel.SetActive(false);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
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
