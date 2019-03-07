using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{

    public static DialogSystem Instance { get; set; }
    public GameObject dialogPanel;

    public List<string> dialogLines = new List<string>();
    public string name;

    Text dialogText, nameText;
    int dialogIndex;

    // Use this for initialization
    void Start()
    {
        dialogText = dialogPanel.transform.Find("Text").GetComponent<Text>();
        nameText = dialogPanel.transform.Find("Name").GetChild(0).GetComponent<Text>();

        dialogPanel.SetActive(false);

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
            ContinueDialog();
        }
    }

    public void AddNewDialog(string[] lines, string name)
    {
        dialogIndex = 0;
        dialogLines = new List<string>();
        foreach (string line in lines)
        {
            dialogLines.Add(line);
        }
        this.name = name;
        //Debug.Log(dialogLines.Count);
        CreateDialog();
    }

    public void CreateDialog()
    {
        dialogText.text = dialogLines[dialogIndex];
        nameText.text = name;
        dialogPanel.SetActive(true);
    }

    public void ContinueDialog()
    {
        if (dialogIndex < dialogLines.Count - 1)
        {
            dialogIndex++;
            dialogText.text = dialogLines[dialogIndex];
        }
        else
        {
            dialogPanel.SetActive(false);
        }
    }
}
