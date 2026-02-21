using Requests;
using UnityEngine;
using System.Runtime.InteropServices;

public class NetworkManager
{
    [DllImport("__Internal")]
    private static extern string GetUrlParamJS(string paramName);

    private readonly string _localUrl;
    private readonly string _globalUrl;
    private readonly bool _test;
    private readonly string _defaultUser;

    private string Url
    {
        get
        {
            if (_test) return _localUrl;
#if UNITY_EDITOR
            return _localUrl;
#else
            return _globalUrl;
#endif
        }
    }

    public async Awaitable<int> GetSeedAsync()
    {
        string json = await Get.SendAsync($"{Url}/api/game/seed");
        return JsonUtility.FromJson<SeedResponse>(json).seed;
    }

    public async Awaitable SendCoinsAsync(uint amount)
    {
        string user = GetUrlParam("user") ?? _defaultUser;
        await Post.SendAsync($"{Url}/api/game/coins?user={user}", JsonUtility.ToJson(new CoinsRequest(amount)));
    }

    private string GetUrlParam(string param)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetUrlParamJS(param);
#else
        return null;
#endif
    }       
    
    public NetworkManager(string localUrl, string globalUrl, bool test, string defaultUser)
    {
        _localUrl = localUrl;
        _globalUrl = globalUrl;
        _test = test;
        _defaultUser = defaultUser;
    }
}

[System.Serializable]
public class CoinsRequest
{
    public uint amount;
    
    public CoinsRequest (uint amount) => this.amount = amount;
}


[System.Serializable]
public class SeedResponse
{
    public int seed;
}
