using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace RedMinS.UI
{
    /// <summary>
    /// UI 입력 처리를 담당하는 클래스
    /// </summary>
    public class UIInputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableBackKeyHandling = true;
        [SerializeField] private bool enableMobileOptimization = true;
        [SerializeField] private float mobileDragThresholdMultiplier = 0.5f;

        public event Action OnBackKeyPressed;
        public event Action OnMenuKeyPressed;

        private UIPopupManager popupManager;

        /// <summary>
        /// 뒤로가기 키 처리 활성화 여부
        /// </summary>
        public bool BackKeyEnabled 
        { 
            get => enableBackKeyHandling; 
            set => enableBackKeyHandling = value; 
        }

        private void Awake()
        {
            SetupMobileOptimization();
            FindUIComponents();
        }

        private void FindUIComponents()
        {
            popupManager = FindObjectOfType<UIPopupManager>();
            if (popupManager == null)
            {
                Debug.LogWarning("[UIInputManager] UIPopupManager not found. Back key handling for popups will not work.");
            }
        }

        private void SetupMobileOptimization()
        {
#if (!UNITY_EDITOR) && (UNITY_ANDROID || UNITY_IPHONE)
            if (enableMobileOptimization && EventSystem.current != null)
            {
                // 모바일 환경에서 드래그 임계값 조정
                float dragThreshold = mobileDragThresholdMultiplier * Screen.dpi / 2.54f;
                EventSystem.current.pixelDragThreshold = (int)dragThreshold;
                
                Debug.Log($"[UIInputManager] Mobile drag threshold set to: {dragThreshold:F1} pixels");
            }
#endif
        }

        private void Update()
        {
            HandleInputs();
        }

        private void HandleInputs()
        {
            // Android/iOS 뒤로가기 키 처리
            if (enableBackKeyHandling && Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackKey();
            }

            // 메뉴 키 처리 (Android)
#if UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Menu))
            {
                HandleMenuKey();
            }
#endif

            // 추가 키보드 단축키들
            HandleKeyboardShortcuts();
        }

        private void HandleBackKey()
        {
            OnBackKeyPressed?.Invoke();

            // 팝업이 있다면 팝업 관리자에게 위임
            if (popupManager != null && popupManager.IsPopupActive)
            {
                popupManager.HandleBackKey();
                return;
            }

            // 팝업이 없다면 앱 종료 확인
            HandleAppExit();
        }

        private void HandleMenuKey()
        {
            OnMenuKeyPressed?.Invoke();
            Debug.Log("[UIInputManager] Menu key pressed");
        }

        private void HandleKeyboardShortcuts()
        {
            // ESC 키 추가 처리 (에디터에서 테스트용)
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("[UIInputManager] F1 pressed - Debug shortcut");
            }
#endif
        }

        private void HandleAppExit()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            // 모바일에서 앱 종료 확인 팝업 표시
            if (popupManager != null)
            {
                ShowExitConfirmationPopup();
            }
            else
            {
                // 팝업 매니저가 없다면 직접 종료
                Application.Quit();
            }
#else
            // 에디터나 데스크톱에서는 바로 종료
            Application.Quit();
#endif
        }

        private void ShowExitConfirmationPopup()
        {
            try
            {
                var uiString = Core.app?.table?.uiString;
                if (uiString != null)
                {
                    string message = uiString.GetString(999); // "게임을 종료하시겠습니까?"
                    string yesText = uiString.GetString(1);   // "확인"
                    string noText = uiString.GetString(2);    // "취소"
                    
                    popupManager.ShowSystemPopup(false, message, yesText, Application.Quit, noText, null);
                }
                else
                {
                    // 기본 메시지로 대체
                    popupManager.ShowSystemPopup(false, "Exit the game?", "Yes", Application.Quit, "No", null);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UIInputManager] Error showing exit popup: {e.Message}");
                Application.Quit();
            }
        }

        /// <summary>
        /// 뒤로가기 키 처리를 수동으로 트리거
        /// </summary>
        public void TriggerBackKey()
        {
            HandleBackKey();
        }

        /// <summary>
        /// 모바일 최적화 설정을 업데이트
        /// </summary>
        public void UpdateMobileSettings()
        {
            SetupMobileOptimization();
        }

        /// <summary>
        /// 드래그 임계값 수동 설정
        /// </summary>
        public void SetDragThreshold(float threshold)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.pixelDragThreshold = (int)threshold;
                Debug.Log($"[UIInputManager] Drag threshold set to: {threshold} pixels");
            }
        }

        /// <summary>
        /// 현재 드래그 임계값 반환
        /// </summary>
        public float GetCurrentDragThreshold()
        {
            return EventSystem.current != null ? EventSystem.current.pixelDragThreshold : 0f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            mobileDragThresholdMultiplier = Mathf.Max(0.1f, mobileDragThresholdMultiplier);
        }

        [ContextMenu("Test Back Key")]
        private void TestBackKey()
        {
            if (Application.isPlaying)
                HandleBackKey();
        }
#endif
    }
}