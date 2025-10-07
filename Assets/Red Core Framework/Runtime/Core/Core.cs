using UnityEngine;
using System;

namespace RedMinS
{
    public static class Core
    {
        /// <summary>
        /// 애플리케이션 매니저 인스턴스
        /// </summary>
        public static AppManager app => AppManager.Instance;
        
        /// <summary>
        /// 네트워크 매니저 인스턴스
        /// </summary>
        public static NetworkManager network => NetworkManager.Instance;

        /// <summary>
        /// 데이터베이스 매니저 인스턴스
        /// </summary>
        public static DatabaseManager database => DatabaseManager.Instance;

        /// <summary>
        /// 모든 Core 시스템이 초기화되었는지 확인
        /// </summary>
        public static bool IsInitialized => 
            AppManager.HasInstance && 
            NetworkManager.HasInstance && 
            DatabaseManager.HasInstance;

        /// <summary>
        /// Core 시스템 강제 초기화
        /// </summary>
        public static void Initialize()
        {
            // 매니저들의 인스턴스 생성을 강제로 트리거
            _ = app;
            _ = network;
            _ = database;
            
            Debug.Log("[Core] All systems initialized.");
        }

        /// <summary>
        /// 애플리케이션 종료 시 정리 작업
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            // 도메인 리로드 시 정적 필드 초기화
        }

        /// <summary>
        /// 디버그용 시스템 상태 출력
        /// </summary>
        public static void LogSystemStatus()
        {
            Debug.Log($"[Core] System Status:");
            Debug.Log($"  - App: {(AppManager.HasInstance ? "✓" : "✗")}");
            Debug.Log($"  - Network: {(NetworkManager.HasInstance ? "✓" : "✗")}");
            Debug.Log($"  - Database: {(DatabaseManager.HasInstance ? "✓" : "✗")}");
        }
    }
}