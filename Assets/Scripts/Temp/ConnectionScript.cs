using Unity.Netcode;
using UnityEngine;

public class ConnectionScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) NetworkManager.Singleton.StartClient();
        if (Input.GetKeyDown(KeyCode.H)) NetworkManager.Singleton.StartHost();
    }
}
