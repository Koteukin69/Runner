namespace Requests
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class Get
    {
        public static async Awaitable<string> SendAsync(string uri, int timeout = 30)
        {
            using UnityWebRequest www = UnityWebRequest.Get(uri);
            www.timeout = timeout;
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception($"Server does not respond: {www.error}");

            return www.downloadHandler.text;
        }
    }
}
