using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReijiPanel : MonoBehaviour {
    private Text talk;
    private Button next;
    private List<string> sentence = new List<string>();
    private int i = 0;
    // Use this for initialization
    void Start()
    {
        talk = GameObject.Find("Reijitalk").GetComponent<Text>();
        next = GameObject.Find("Reijinext").GetComponent<Button>();
        sentence = new List<string>();
        sentence.Add("（迷迷糊糊）诶嘿嘿……已经吃不下啦……");
        sentence.Add("诶？肯？原来如此，该起床了吗？");
        sentence.Add("（大声）诶——！！！这是哪儿啊！！");
        sentence.Add("难道是、邪恶的暗部组织（Black Organization）终于对我这个超能力者（psychic）出手了吗？你会保护我的对吧？肯？");
        sentence.Add("顺带一提，我的超能力是超可爱！");

        talk.text = sentence[0];

        next.onClick.AddListener(OnReijiTalkClick);



    }

    public void OnReijiTalkClick()
    {
        Debug.Log("talk");
        if (i+1<sentence.Count)
        {
            talk.text = sentence[i + 1];
            i++;
        }

        else
        {
            //调用KenPanel，关闭ReijiPanel
            PanelManage.Instance.RToK();
        }
    }
}
