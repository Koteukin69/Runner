namespace Requests
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    public class Post
    {
        private Action<string> _callback;
        
        private static IEnumerator PostCoroutine(string uri, string json, Action<string> callback, int timeout)
        {
            using UnityWebRequest www = UnityWebRequest.Post(uri, json, "application/json");
            www.timeout = timeout;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("Server does not respond: " + www.error);
            else callback?.Invoke(www.downloadHandler.text);
        }

        public void AddResponseCallback(Action<string> callback) => _callback += callback;

        public Post(string uri, string json, MonoBehaviour instance, int timeout = 30) =>
            instance.StartCoroutine(PostCoroutine(uri, json, _callback, timeout));
    }
}