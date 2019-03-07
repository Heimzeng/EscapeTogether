using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonClick : MonoBehaviour {

    public Button yourButton;
    public InputField host;
    public InputField port;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        InputField hostInput = host.GetComponent<InputField>();
        InputField portInput = port.GetComponent<InputField>();
        string hostName = hostInput.text;
        int portNum;
        int.TryParse(portInput.text, out portNum);
        Debug.Log("ip:" + hostName + ";port:" + portNum);
        Server.Instance.host = hostName;
        Server.Instance.port = portNum;
        Server.Instance.SocketServie();
    }
}
