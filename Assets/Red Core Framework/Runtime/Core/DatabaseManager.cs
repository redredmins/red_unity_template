using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RedMinS
{
    public class DatabaseManager : SingletonMonobehaviour<DatabaseManager>
    {
        [Header("Database Settings")]
        [SerializeField] private bool enableLocalStorage = true;
        [SerializeField] private bool enableCloudSync = false;

        public event Action OnDataLoaded;
        public event Action OnDataSaved;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            Debug.Log("[DatabaseManager] Initializing database system...");
            // 데이터베이스 초기화 로직
        }

        public void SaveData<T>(string key, T data) where T : class
        {
            try
            {
                string jsonData = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(key, jsonData);
                PlayerPrefs.Save();
                
                Debug.Log($"[DatabaseManager] Data saved: {key}");
                OnDataSaved?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[DatabaseManager] Save failed: {e.Message}");
            }
        }

        public T LoadData<T>(string key) where T : class
        {
            try
            {
                string jsonData = PlayerPrefs.GetString(key, string.Empty);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    T data = JsonUtility.FromJson<T>(jsonData);
                    Debug.Log($"[DatabaseManager] Data loaded: {key}");
                    OnDataLoaded?.Invoke();
                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DatabaseManager] Load failed: {e.Message}");
            }
            
            return null;
        }

        public bool HasData(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void DeleteData(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"[DatabaseManager] Data deleted: {key}");
        }

        public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("[DatabaseManager] All data cleared");
        }
    }
}