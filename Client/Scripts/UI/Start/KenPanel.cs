using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KenPanel : MonoBehaviour {
    private Text talk;
    private Button next;
    private List<string> sentence = new List<string>();
    private int i = 0;
	// Use this for initialization
	void Start () {
        talk = GameObject.Find("Kentalk").GetComponent<Text>();
        next = GameObject.Find("Kennext").GetComponent<Button>();
        sentence.Add("……都这个时候了，你也稍微有点紧张感吧。");
        sentence.Add("（叹气）嘛，让你这么一打岔，我反而冷静下来了。");
        sentence.Add("总之先调查一下这个屋子吧。");

        talk.text = sentence[0];

        next.onClick.AddListener(OnKenTalkClick);


		
	}
	
    public void OnKenTalkClick()
    {
        if(i+1<sentence.Count)
        {
            talk.text = sentence[i + 1];
            i++;
        }

        else
        {
            //返回游戏
            PanelManage.Instance.KToNull();
        }
    }
}
