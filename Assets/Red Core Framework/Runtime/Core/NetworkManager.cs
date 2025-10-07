using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace RedMinS
{
    public class NetworkManager : SingletonMonobehaviour<NetworkManager>
    {
        [Header("Network Settings")]
        [SerializeField] private float timeoutDuration = 10f;
        [SerializeField] private int maxRetryCount = 3;

        public event Action OnNetworkConnected;
        public event Action OnNetworkDisconnected;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            // 네트워크 초기화 로직
        }

        public void SendRequest<T>(string url, Action<T> onSuccess, Action<string> onError = null) where T : class
        {
            StartCoroutine(SendRequestCoroutine(url, onSuccess, onError));
        }

        private IEnumerator SendRequestCoroutine<T>(string url, Action<T> onSuccess, Action<string> onError) where T : class
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = (int)timeoutDuration;
                
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
                        onSuccess?.Invoke(result);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Network Error: {request.error}");
                }
            }
        }

        public void CheckNetworkConnection()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                OnNetworkConnected?.Invoke();
            }
            else
            {
                OnNetworkDisconnected?.Invoke();
            }
        }
    }
}