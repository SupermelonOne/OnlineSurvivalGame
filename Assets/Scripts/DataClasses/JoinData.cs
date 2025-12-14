using UnityEngine;

public class JoinData
{
    public static JoinData Instance;
    public string IPADDRESS;
    public string PORT;
    public bool isHosting;

    public JoinData(string IP, string PORT, bool Host)
    {
        this.IPADDRESS = IP;
        this.PORT = PORT;
        this.isHosting = Host;
        Instance = this;
    }
}
