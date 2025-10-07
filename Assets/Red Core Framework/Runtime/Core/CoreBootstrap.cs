using UnityEngine;
using System.Collections;

namespace RedMinS
{
    /// <summary>
    /// 게임 시작 시 Core 시스템을 초기화하는 부트스트랩 클래스
    /// </summary>
    public class CoreBootstrap : MonoBehaviour
    {
        [Header("Bootstrap Settings")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private float initializationDelay = 0.1f;

        private void Awake()
        {
            if (autoInitialize)
            {
                StartCoroutine(InitializeCoreSystemsCoroutine());
            }
        }

        private IEnumerator InitializeCoreSystemsCoroutine()
        {
            if (showDebugLogs)
                Debug.Log("[CoreBootstrap] Starting core system initialization...");

            // 약간의 지연을 두어 다른 시스템들이 Awake를 완료할 시간을 줍니다
            yield return new WaitForSeconds(initializationDelay);

            // Core 시스템 초기화
            Core.Initialize();

            // 초기화 완료 대기
            yield return new WaitUntil(() => Core.IsInitialized);

            if (showDebugLogs)
            {
                Debug.Log("[CoreBootstrap] Core system initialization completed!");
                Core.LogSystemStatus();
            }

            // 초기화 완료 후 추가 작업
            OnInitializationComplete();
        }

        /// <summary>
        /// 수동으로 Core 시스템을 초기화합니다
        /// </summary>
        [ContextMenu("Initialize Core Systems")]
        public void ManualInitialize()
        {
            StartCoroutine(InitializeCoreSystemsCoroutine());
        }

        /// <summary>
        /// 초기화 완료 후 호출되는 메서드
        /// </summary>
        protected virtual void OnInitializationComplete()
        {
            // 게임별 추가 초기화 로직을 여기에 구현
            Debug.Log("[CoreBootstrap] Ready to start game!");
        }

        private void Start()
        {
            // 씬이 로드된 후 한 번 더 시스템 상태 확인
            if (showDebugLogs && Core.IsInitialized)
            {
                Core.LogSystemStatus();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // 에디터에서 설정 변경 시 검증
            initializationDelay = Mathf.Max(0f, initializationDelay);
        }
#endif
    }
}