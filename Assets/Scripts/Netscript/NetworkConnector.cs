using System.Runtime.InteropServices;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkConnector : MonoBehaviour
{
    public void Host() => NetworkManager.Singleton.StartHost();
    public void Join() => NetworkManager.Singleton.StartClient();
    public void Server() => NetworkManager.Singleton.StartServer();

    private void Start()
    {
        if (JoinData.Instance != null)
        {

            ushort PORT;

            string portString = JoinData.Instance.PORT;
            portString = System.Text.RegularExpressions.Regex.Replace(portString, @"\D", "");
            Debug.Log(portString);
            Debug.Log(JoinData.Instance.IPADDRESS);
            if (!ushort.TryParse(portString, out PORT))
            {
                Debug.LogError("Invalid port number");
                WindowsMessageBox.Show(
                    "Port must be a number between 0 and 65535",
                    "Connection Error"
                );
                foreach (char c in JoinData.Instance.PORT)
                {
                    Debug.Log($"'{c}' = {(int)c}");
                }
            }
            string IPADRESS = JoinData.Instance.IPADDRESS;
            if (JoinData.Instance.isHosting)
            {
                UnityTransport transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetConnectionData("0.0.0.0", PORT);
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                UnityTransport transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetConnectionData(IPADRESS, PORT);
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}

public static class WindowsMessageBox
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int MessageBox(
        System.IntPtr hWnd,
        string text,
        string caption,
        uint type);

    public static void Show(string message, string title = "Error")
    {
        MessageBox(System.IntPtr.Zero, message, title, 0);
    }
}