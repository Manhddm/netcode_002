using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        networkManager.OnClientConnectedCallback += OnClientConnect;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;
        Debug.Log(userData.userName);
        response.Approved = true;
        response.CreatePlayerObject = false;

    }

    private void OnNetworkReady()
    {
        Debug.Log("OnNetworkReady");
    }
    private void OnClientConnect(ulong clientId)
    { 
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, mode) => {
            if (sceneName == "Game")
            {
                Vector3 spawnPoint = SpawnPoint.GetRandomSpawnPoint();
                if (networkManager.NetworkConfig.PlayerPrefab != null)
                {
                    GameObject playerInstance = 
                        UnityEngine.Object.Instantiate(networkManager.NetworkConfig.PlayerPrefab, spawnPoint, Quaternion.identity);
                    
                    playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                }
            }
        };
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out var authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }
    
    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out var authId))
        {
            if (authIdToUserData.TryGetValue(authId, out var userData))
            {
                return userData;
            }
        }

        return null;
    }
    public void Dispose()
    {
        if (networkManager)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnClientConnectedCallback -= OnClientConnect;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.OnServerStarted -= OnNetworkReady;
            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
        
    }
}
