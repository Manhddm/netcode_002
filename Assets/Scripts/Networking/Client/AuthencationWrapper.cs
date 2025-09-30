using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthencationWrapper
{
    public static AuthState AuthSate { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        if (AuthSate == AuthState.Authenticated)
        {
            return AuthSate;
        }

        if (AuthSate == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating, waiting for result...");
            await Authenticating();
            return AuthSate;
        }
        await SignInAnonymouslyAsync(maxRetries);

        return AuthSate;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthSate == AuthState.Authenticating || AuthSate == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthSate;
    }
    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {

        AuthSate = AuthState.Authenticating;
        int tries = 0;
        while (AuthSate == AuthState.Authenticating && tries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn &&
                    AuthenticationService.Instance.IsAuthorized)
                {
                    AuthSate = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthSate = AuthState.Error;
            }
            catch (RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                AuthSate = AuthState.Error;
            }
            tries++;
            await Task.Delay(1000);
        }

        if (AuthSate != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully within the {maxRetries} limit.");
            AuthSate = AuthState.Timeout;
        }
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
