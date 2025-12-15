using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class VersionChecker : NetworkBehaviour
{
    [SerializeField] string version;
    private NetworkVariable<FixedString32Bytes> netVersion =
        new NetworkVariable<FixedString32Bytes>(
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            netVersion.Value = version;
        else
        {
            netVersion.OnValueChanged += OnVersionChanged;
        }
    }
    private void OnVersionChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        CheckVersion(newValue);
    }
    private void CheckVersion(FixedString32Bytes serverVersion)
    {
        if (serverVersion != version)
        {
            Debug.LogError($"Version mismatch! Client: {version}, Server: {serverVersion}");
            NetworkManager.Singleton.Shutdown();
        }
    }
}
