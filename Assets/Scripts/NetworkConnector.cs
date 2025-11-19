using Unity.Netcode;
using UnityEngine;

public class NetworkConnector : MonoBehaviour
{
    public void Host() => NetworkManager.Singleton.StartHost();
    public void Join() => NetworkManager.Singleton.StartClient();
    public void Server() => NetworkManager.Singleton.StartServer();
}
