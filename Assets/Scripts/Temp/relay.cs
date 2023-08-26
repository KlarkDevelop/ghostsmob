using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using TMPro;

public class relay : MonoBehaviour
{
    private UnityTransport transport;
    [SerializeField] private int MaxPlayers = 2;
    [SerializeField] private GameObject buttonH;
    [SerializeField] private GameObject buttonJ;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text code;

    private async void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
        await Authenticate();
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        buttonH.SetActive(false);
        buttonJ.SetActive(false);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        code.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame()
    {
        buttonH.SetActive(false);
        buttonJ.SetActive(false);

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(input.text);

        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
        NetworkManager.Singleton.StartClient();
    }
}
