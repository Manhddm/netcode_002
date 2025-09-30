using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class UGSInit : MonoBehaviour
{
    private static bool _initialized;

    private async void Awake()
    {
        await EnsureInitializedAndSignedIn();
    }

    public static async Task EnsureInitializedAndSignedIn()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync(); // yêu cầu đã Link Project (Project Settings > Services)
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"UGS ready. PlayerID={AuthenticationService.Instance.PlayerId}");
        }

        _initialized = true;
    }
}