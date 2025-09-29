using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class AuthencationWrapper
{
    public static AuthState AuthSate { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthSate == AuthState.Authenticated)
        {
            return AuthSate;
        }
        AuthSate = AuthState.Authenticating;
        int tries = 0;
        while (AuthSate == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn &&
                AuthenticationService.Instance.IsAuthorized)
            {
                AuthSate = AuthState.Authenticated;
                break;
            }
            tries++;
            await Task.Delay(1000);
        }

        return AuthSate;
    } 
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}
