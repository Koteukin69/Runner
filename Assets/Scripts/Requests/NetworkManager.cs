using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Requests
{
    public class NetworkManager
    {
        [DllImport("__Internal")]
        private static extern string GetUrlParamJS(string paramName);

        private readonly string _url;
        private readonly string _defaultUser;
        private bool _offline;
        private static int _seed;

        public async Awaitable<int> GetSeedAsync()
        {
            if (_offline) return RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
            
            string user = GetUrlParam("user") ?? _defaultUser;
            string json = await Get.SendAsync($"{_url}/api/game/seed?user={user}");
            _seed = JsonUtility.FromJson<SeedResponse>(json).seed;
            return _seed;
        }

        public async Awaitable SendCoinsAsync(uint amount)
        {
            if (_offline || amount == 0) return;
            string user = GetUrlParam("user") ?? _defaultUser;
            await Post.SendAsync($"{_url}/api/game/coins?user={user}", JsonUtility.ToJson(new CoinsRequest(amount, _seed)), 5);
        }

        private string GetUrlParam(string param)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return GetUrlParamJS(param);
#else
            return null;
#endif
        }       
    
        public void SetOffline(bool offline = true) => _offline = offline;

        public NetworkManager(string url, string defaultUser, bool offline = false)
        {
            _url = url;
            _defaultUser = defaultUser;
            _offline = offline;
        }
    }
    
    [Serializable]
    public class CoinsRequest
    {
        public uint amount;
        public int seed;
        public string args;

        public CoinsRequest(uint amount, int seed)
        {
            args = HashCoins(amount, seed);
            this.amount = amount;
            this.seed = seed;
        }
        
        public static string HashCoins(uint coins, int seed)
        {
            byte[] key = Encoding.UTF8.GetBytes(GameSecrets.HashSecret);
            byte[] data = Encoding.UTF8.GetBytes($"{seed}{coins}");
            using var hmac = new HMACSHA256(key);
            byte[] hash = hmac.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }


    [Serializable]
    public class SeedResponse
    {
        public int seed;
    }
}
