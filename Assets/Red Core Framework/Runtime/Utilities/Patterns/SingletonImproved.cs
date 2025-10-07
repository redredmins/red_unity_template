using UnityEngine;
using System;

namespace RedMinS
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static T _instance = null;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }

    public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance = null;
        protected static bool _applicationIsQuitting = false;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindAnyObjectByType<T>();
                            if (_instance == null)
                            {
                                GameObject obj = new GameObject($"[Singleton]{typeof(T).Name}");
                                _instance = obj.AddComponent<T>();
                                
                                // DontDestroyOnLoad 적용
                                if (Application.isPlaying)
                                {
                                    DontDestroyOnLoad(obj);
                                }
                            }
                        }
                    }
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(gameObject);
                }
                OnSingletonAwake();
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} found. Destroying.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnSingletonAwake() { }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}