using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KanbanPanel : MonoBehaviour {
    private Text talk;
    private Button next;
    private List<string> sentence = new List<string>();
    private int i = 0;
    // Use this for initialization
    void Start()
    {
        talk = GameObject.Find("Kanbantalk").GetComponent<Text>();
        next = GameObject.Find("Kanbannext").GetComponent<Button>();
        sentence.Add("痛痛痛……这里是哪里？");
        sentence.Add("我明明记得自己在学校里走得好好的，忽然眼前一黑，醒过来居然就到了这里。");
        sentence.Add("莫非是绑架？");
        sentence.Add("对了！阿莱雅呢？阿莱雅！");

        talk.text = sentence[0];

        next.onClick.AddListener(OnKanbanTalkClick);



    }

    public void OnKanbanTalkClick()
    {
        if (i+1<sentence.Count)
        {
            talk.text = sentence[i + 1];
            i++;
        }

        else
        {
            PanelManage.Instance.KToR();
        }
    }
   
}
