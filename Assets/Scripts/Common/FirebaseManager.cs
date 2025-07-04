using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Messaging;
using Firebase.RemoteConfig;
using UnityEngine;

//using Firebase;

public sealed class FirebaseManager
{
    //// 定数

    private readonly Dictionary<string, string> _debugRemoteConfigs = new Dictionary<string, string>();

    public static FirebaseManager Instance { get; } = new FirebaseManager();

    public FirebaseApp App　
    {
        get
        {
            return FirebaseApp.DefaultInstance;
        }
    }

    public FirebaseAuth Auth
    {
        get
        {
            return FirebaseAuth.DefaultInstance;
        }
    }

    public async UniTask Initialize(Dictionary<string, object> defaultRemoteConfigs)
    {
        await FirebaseApp.CheckAndFixDependenciesAsync();
        await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaultRemoteConfigs);
        await FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
    }

    public void AddRemoteConfigValueForDebug(string key, string value)
    {
        this._debugRemoteConfigs[key] = value;
    }

    public string GetRemoteConfigValue(string key)
    {
        // if (Const.isDebug)
        // {
        //     if (this._debugRemoteConfigs.ContainsKey(key))
        //     {
        //         return this._debugRemoteConfigs[key];
        //     }
        // }

        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
    }

    public void InitMessaging()
    {
        FirebaseMessaging.TokenReceived += this.OnTokenReceived;
        FirebaseMessaging.MessageReceived += this.OnMessageReceived;
        FirebaseMessaging.SubscribeAsync("all");
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }

    public async Task<FirebaseUser> LoginAsAnonymous()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            return user;
        }

        AuthResult authResult = await auth.SignInAnonymouslyAsync();

        return authResult.User;
    }

    public async UniTask<FirebaseUser> LoginAsUser(string email, string password)
    {
        try
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);

            return authResult.User;
        }
        catch (Exception e)
        {
            string message = e.Message;
            if (message.Contains("more") && message.Contains("errors"))
            {
                message = $"メールアドレスとパスワードが正しいか確認してください\n({message})";
            }

            throw new Exception(message);
        }

        return null;
    }

    public async UniTask<FirebaseUser> LinkUserInfo(string email, string password)
    {
        try
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            Credential credential =
                EmailAuthProvider.GetCredential(email, password);
            AuthResult authResult = await auth.CurrentUser.LinkWithCredentialAsync(credential);
            return authResult.User;
        }
        catch (Exception e)
        {
            string message = e.Message;
            if (message.Contains("already") || message.Contains("sensitive"))
            {
                message = $"すでに登録されています\n({message})";
            }
            else if (message.Contains("email") && message.Contains("format"))
            {
                message = $"メールアドレスの形式が正しくありません\n({message})";
            }
            else if (message.Contains("email") && message.Contains("provide"))
            {
                message = $"メールアドレスを入力してください\n({message})";
            }

            throw new Exception(message);
        }

        return null;
    }
}
