using UnityEngine;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Requests
{
    public class NetworkManager
    {
        [DllImport("__Internal")]
        private static extern string GetUrlParamJS(string paramName);

        private readonly string _url;
        private readonly string _defaultUser;
        private bool _offline;

        public async Awaitable<int> GetSeedAsync()
        {
            if (_offline) return RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
            string json = await Get.SendAsync($"{_url}/api/game/seed");
            return JsonUtility.FromJson<SeedResponse>(json).seed;
        }

        public async Awaitable SendCoinsAsync(uint amount)
        {
            if (_offline) return;
            string user = GetUrlParam("user") ?? _defaultUser;
            await Post.SendAsync($"{_url}/api/game/coins?user={user}", JsonUtility.ToJson(new CoinsRequest(amount)), 5);
        }

        private string GetUrlParam(string param)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return GetUrlParamJS(param);
#else
            return null;
#endif
        }       
    
        public void SetOffline() => _offline = true;

        public NetworkManager(string url, string defaultUser, bool offline = false)
        {
            _url = url;
            _defaultUser = defaultUser;
            _offline = offline;
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
}
