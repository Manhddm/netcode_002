using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
    public async void StartClient()
    {
        string joinCode = joinCodeField.text;
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
    }
}
