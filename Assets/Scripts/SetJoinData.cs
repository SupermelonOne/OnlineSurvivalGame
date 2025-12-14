using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetJoinData : MonoBehaviour
{
    string ipText;
    string portText;
    public void Host()
    {
        new JoinData(ipText, portText, true);
        SceneManager.LoadScene("GameScene");
        
    }
    public void Join()
    {
        new JoinData(ipText, portText, false);

        SceneManager.LoadScene("GameScene");
        
    }
    public void ChangeIp(string ip)
    {
        ipText = ip;
    }
    public void ChangePort(string port)
    {
        portText = port;
    }
}
