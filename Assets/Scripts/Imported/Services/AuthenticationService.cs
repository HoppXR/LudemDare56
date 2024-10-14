using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
#endif

public static class Authentication
{
    public static string PlayerId { get; private set; }

    public static async Task Login()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new();

#if UNITY_EDITOR
            // Remove this if you don't have ParrelSync installed. 
            // It's used to differentiate the clients, otherwise lobby will count them as the same
            if (ClonesManager.IsClone())
            {
                options.SetProfile(ClonesManager.GetArgument());
            }
            else
            {
                options.SetProfile("Primary");
            }
#endif
            if (Debug.isDebugBuild)
            {
                /*
                    Since debug builds may be on the same host IP, we need to differentiate them.
                    Generating GUIDs as profiles will ensure every client is unique.
                    Profiles are limited to 30 character length, need to cut some entropy off,
                    otherwise this will throw.
                 */
                options.SetProfile(Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 30));
            }

            await UnityServices.InitializeAsync(options);
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
        }

        // TODO: Remove
        Debug.Log($"Using PlayerId {PlayerId}.");
    }
}