using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class ClientGameManager 
{
    private const string MenuSceneName = "Menu";
    public async Task<bool> InitAsync()
    {
        //Authenticate 
        await UnityServices.InitializeAsync();
        AuthState authState = await AuthencationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }
    public void GoToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
