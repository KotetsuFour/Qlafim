using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public static class StaticNetworking
{
    public static bool waiting = false;
    public async static void initialize()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed In As {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async static void createRelay(TextMeshProUGUI display)
    {
        try
        {
            Allocation all = await RelayService.Instance.CreateAllocationAsync(1);
            display.text = await RelayService.Instance.GetJoinCodeAsync(all.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                all.RelayServer.IpV4,
                (ushort)all.RelayServer.Port,
                all.AllocationIdBytes,
                all.Key,
                all.ConnectionData
            );
            NetworkManager.Singleton.StartHost();

            waiting = true;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public static void cancelConnection()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public async static void joinRelay(string joinCode)
    {
        try
        {
            JoinAllocation all = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                all.RelayServer.IpV4,
                (ushort)all.RelayServer.Port,
                all.AllocationIdBytes,
                all.Key,
                all.ConnectionData,
                all.HostConnectionData
            );
            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public static void switchScenesWhenReady()
    {
        if (waiting && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Boardgame",
                UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
