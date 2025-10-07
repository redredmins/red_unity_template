using UnityEngine;
using UnityEngine.Events;
using RedMinS.UI;

namespace RedMinS.UI
{
    /// <summary>
    /// 모든 UI 시스템을 통합 관리하는 메인 UI 매니저
    /// </summary>
    public class UIManager : SingletonMonobehaviour<UIManager>
    {
        [Header("UI Components")]
        [SerializeField] private Canvas mainCanvas;
        
        [Header("UI Managers")]
        [SerializeField] private UIPopupManager _popupManager;
        [SerializeField] private UIToastManager _toastManager;
        [SerializeField] private UIEffectManager _effectManager;
        [SerializeField] private UIResolutionManager _resolutionManager;
        [SerializeField] private UIInputManager _inputManager;

        // 프로퍼티로 각 매니저에 접근
        public UIPopupManager popup => GetOrCreateUIManager<UIPopupManager>(ref _popupManager);
        public UIToastManager toast => GetOrCreateUIManager<UIToastManager>(ref _toastManager);
        public UIEffectManager effect => GetOrCreateUIManager<UIEffectManager>(ref _effectManager);
        public UIResolutionManager resolution => GetOrCreateUIManager<UIResolutionManager>(ref _resolutionManager);
        public UIInputManager input => GetOrCreateUIManager<UIInputManager>(ref _inputManager);

        // 편의성을 위한 이벤트 및 액션
        public UnityAction NoPopupAction
        {
            get => popup.NoPopupAction;
            set => popup.NoPopupAction = value;
        }

        /// <summary>
        /// 팝업이 활성화되어 있는지 확인
        /// </summary>
        public bool isPopupOn => popup.IsPopupActive;

        /// <summary>
        /// 메인 캔버스 반환
        /// </summary>
        public Canvas Canvas => mainCanvas;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            InitializeUISystem();
        }

        private void Start()
        {
            SetupResolution();
        }

        private void InitializeUISystem()
        {
            // 메인 캔버스 찾기
            if (mainCanvas == null)
            {
                mainCanvas = FindObjectOfType<Canvas>();
                if (mainCanvas == null)
                {
                    Debug.LogError("[UIManager] No Canvas found in scene!");
                }
            }

            // UI 매니저들 초기화
            InitializeManagers();
            
            Debug.Log("[UIManager] UI system initialized successfully.");
        }

        private void InitializeManagers()
        {
            // 각 매니저들의 인스턴스를 강제로 생성
            _ = popup;
            _ = toast;
            _ = effect;
            _ = resolution;
            _ = input;
        }

        private void SetupResolution()
        {
            if (resolution != null && mainCanvas != null)
            {
                resolution.SetupResolutionSupport(mainCanvas);
            }
        }

        /// <summary>
        /// UI 매니저를 가져오거나 생성합니다
        /// </summary>
        private T GetOrCreateUIManager<T>(ref T manager) where T : MonoBehaviour
        {
            if (manager == null)
            {
                // 먼저 기존 컴포넌트 찾기
                manager = GetComponent<T>();
                
                if (manager == null)
                {
                    // 씬에서 찾기
                    manager = FindObjectOfType<T>();
                    
                    if (manager == null)
                    {
                        // 새로 생성
                        GameObject managerObj = new GameObject($"[UI]{typeof(T).Name}");
                        managerObj.transform.SetParent(this.transform);
                        manager = managerObj.AddComponent<T>();
                        
                        Debug.Log($"[UIManager] Created new UI manager: {typeof(T).Name}");
                    }
                }
            }
            
            return manager;
        }

        // === 편의성 메서드들 (기존 인터페이스 유지) ===

        /// <summary>
        /// 시스템 팝업 표시
        /// </summary>
        public void ShowSystemPopup(bool isSmallError, string message, UnityAction okAction)
        {
            popup.ShowSystemPopup(isSmallError, message, okAction);
        }

        /// <summary>
        /// 시스템 팝업 표시 (확인/취소)
        /// </summary>
        public void ShowSystemPopup(bool isSmallError, string message, string yesText, UnityAction yesAction, string noText, UnityAction noAction)
        {
            popup.ShowSystemPopup(isSmallError, message, yesText, yesAction, noText, noAction);
        }

        /// <summary>
        /// 강제 종료 팝업 표시
        /// </summary>
        public void ShowSystemPopupAndQuit(string message, UnityAction action = null)
        {
            popup.ShowSystemPopupAndQuit(message, action);
        }

        /// <summary>
        /// 토스트 메시지 표시
        /// </summary>
        public void ShowToast(string message)
        {
            toast.ShowToast(message);
        }

        /// <summary>
        /// 토스트 메시지 표시 (문자열 ID)
        /// </summary>
        public void ShowToast(int stringId)
        {
            toast.ShowToast(stringId);
        }

        /// <summary>
        /// 페이드 인 효과
        /// </summary>
        public void FadeIn()
        {
            effect.FadeIn();
        }

        /// <summary>
        /// 페이드 아웃 효과
        /// </summary>
        public void FadeOut(Color screenColor)
        {
            effect.FadeOut(screenColor);
        }

        /// <summary>
        /// 페이드 인 코루틴
        /// </summary>
        public System.Collections.IEnumerator IEFadeIn()
        {
            return effect.FadeInCoroutine();
        }

        /// <summary>
        /// 페이드 아웃 코루틴
        /// </summary>
        public System.Collections.IEnumerator IEFadeOut(Color screenColor)
        {
            return effect.FadeOutCoroutine(screenColor);
        }

        /// <summary>
        /// 해상도 지원 설정
        /// </summary>
        public void SupportResolution(Canvas canvas, GameObject gameSceneObj = null)
        {
            resolution.SetupResolutionSupport(canvas, gameSceneObj);
        }

        /// <summary>
        /// 팝업 등록 (UIPopup에서 호출)
        /// </summary>
        public void OnPopup(UIPopup popupInstance)
        {
            popup.RegisterPopup(popupInstance);
        }

        /// <summary>
        /// 팝업 해제 (UIPopup에서 호출)
        /// </summary>
        public void OffPopup(UIPopup popupInstance)
        {
            popup.UnregisterPopup(popupInstance);
        }

        /// <summary>
        /// 모든 UI 시스템 상태 로그
        /// </summary>
        [ContextMenu("Log UI System Status")]
        public void LogUISystemStatus()
        {
            Debug.Log($"[UIManager] UI System Status:");
            Debug.Log($"  - Main Canvas: {(mainCanvas != null ? "✓" : "✗")}");
            Debug.Log($"  - Popup Manager: {(popup != null ? "✓" : "✗")} (Active Popups: {popup?.ActivePopupCount ?? 0})");
            Debug.Log($"  - Toast Manager: {(toast != null ? "✓" : "✗")} (Queued: {toast?.QueuedMessageCount ?? 0})");
            Debug.Log($"  - Effect Manager: {(effect != null ? "✓" : "✗")} (Is Fading: {effect?.IsFading ?? false})");
            Debug.Log($"  - Resolution Manager: {(resolution != null ? "✓" : "✗")}");
            Debug.Log($"  - Input Manager: {(input != null ? "✓" : "✗")}");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (mainCanvas == null)
            {
                mainCanvas = GetComponentInChildren<Canvas>();
            }
        }
#endif
    }
}