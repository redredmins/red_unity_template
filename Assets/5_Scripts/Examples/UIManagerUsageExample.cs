// ========================================
// UIManager 리팩토링 완료 - 사용법 가이드
// ========================================

/*
 * UIManager가 다음과 같이 모듈화되었습니다:
 * 
 * 1. UIManager (메인 통합 관리자)
 * 2. UIPopupManager (팝업 시스템)
 * 3. UIToastManager (토스트 메시지)
 * 4. UIEffectManager (페이드 효과)
 * 5. UIResolutionManager (해상도 지원)
 * 6. UIInputManager (입력 처리)
 */

using UnityEngine;
using RedMinS;
using RedMinS.UI;

namespace RedMinS.Examples
{
    public class UIManagerUsageExample : MonoBehaviour
    {
        void Start()
        {
            ShowUsageExamples();
        }

        private void ShowUsageExamples()
        {
            var ui = Core.app.ui;

            // === 기존 방식 (그대로 사용 가능) ===
            ui.ShowToast("Hello World!");
            ui.ShowSystemPopup(false, "System message", () => Debug.Log("OK clicked"));

            // === 새로운 모듈별 접근 방식 ===
            
            // 토스트 관련
            ui.toast.ShowToast("New modular toast!");
            ui.toast.ShowToast(1001); // String table ID
            ui.toast.SetToastDuration(2.0f);
            
            // 팝업 관련
            ui.popup.ShowSystemPopup("Popup message", () => Debug.Log("Confirmed"));
            ui.popup.CloseAllPopups();
            bool hasPopup = ui.popup.IsPopupOpen<UISystemPopup>();
            
            // 페이드 효과
            ui.effect.FadeIn();
            ui.effect.FadeOut(Color.black);
            StartCoroutine(ui.effect.FadeInCoroutine());
            
            // 해상도 정보
            var resInfo = ui.resolution.GetCurrentResolutionInfo();
            Debug.Log($"Resolution: {resInfo.screenWidth}x{resInfo.screenHeight}");
            
            // 입력 관리
            ui.input.BackKeyEnabled = true;
            ui.input.OnBackKeyPressed += () => Debug.Log("Back key pressed!");
            
            // 시스템 상태 확인
            ui.LogUISystemStatus();
        }

        // === 고급 사용 예제 ===
        
        void ShowAdvancedExamples()
        {
            var ui = Core.app.ui;
            
            // 크로스 페이드 효과
            StartCoroutine(ui.effect.CrossFade(() => {
                // 씬 전환 로직
                Debug.Log("Scene transition logic here");
            }, Color.black, 1.0f));
            
            // 팝업 이벤트 처리
            ui.popup.OnAllPopupsClosed += () => Debug.Log("All popups closed");
            
            // 토스트 큐 관리
            ui.toast.ShowToast("Message 1");
            ui.toast.ShowToast("Message 2");
            ui.toast.ShowToast("Message 3");
            Debug.Log($"Queued messages: {ui.toast.QueuedMessageCount}");
            
            // 입력 이벤트 처리
            ui.input.OnBackKeyPressed += HandleBackKey;
            ui.input.OnMenuKeyPressed += HandleMenuKey;
        }

        void HandleBackKey()
        {
            Debug.Log("Custom back key handler");
        }

        void HandleMenuKey()
        {
            Debug.Log("Custom menu key handler");
        }

        // === 팝업 시스템 예제 ===
        
        void ShowPopupExamples()
        {
            var popup = Core.app.ui.popup;
            
            // 간단한 확인 팝업
            popup.ShowSystemPopup("Are you sure?", () => Debug.Log("Confirmed"));
            
            // 예/아니오 팝업
            popup.ShowSystemPopup(false, "Delete file?", "Yes", 
                () => Debug.Log("File deleted"),
                "No", 
                () => Debug.Log("Cancelled"));
                
            // 강제 종료 팝업
            popup.ShowSystemPopupAndQuit("Critical error occurred!");
            
            // 팝업 상태 확인
            if (popup.IsPopupActive)
            {
                Debug.Log($"Active popups: {popup.ActivePopupCount}");
            }
        }

        // === 토스트 시스템 예제 ===
        
        void ShowToastExamples()
        {
            var toast = Core.app.ui.toast;
            
            // 다양한 토스트 표시
            toast.ShowToast("Simple message");
            toast.ShowToast(1001); // From string table
            toast.ShowImmediateToast("Urgent message!"); // Skip queue
            
            // 토스트 설정
            toast.SetToastDuration(2.5f);
            toast.SetMaxQueueSize(5);
            
            // 토스트 상태 확인
            if (toast.IsToastActive)
            {
                Debug.Log($"Queued toasts: {toast.QueuedMessageCount}");
            }
        }

        // === 효과 시스템 예제 ===
        
        void ShowEffectExamples()
        {
            var effect = Core.app.ui.effect;
            
            // 페이드 효과들
            effect.FadeIn();
            effect.FadeOut();
            effect.FadeOut(Color.red, 1.5f);
            
            // 즉시 화면 설정
            effect.SetScreenColor(Color.black);
            effect.ClearScreen();
            
            // 코루틴 사용
            StartCoroutine(FadeSequence());
        }

        System.Collections.IEnumerator FadeSequence()
        {
            var effect = Core.app.ui.effect;
            
            yield return effect.FadeOutCoroutine(Color.black);
            yield return new WaitForSeconds(1f);
            yield return effect.FadeInCoroutine();
        }

        // === 해상도 시스템 예제 ===
        
        void ShowResolutionExamples()
        {
            var resolution = Core.app.ui.resolution;
            
            // 현재 해상도 정보 가져오기
            var info = resolution.GetCurrentResolutionInfo();
            Debug.Log($"Screen: {info.screenWidth}x{info.screenHeight}, " +
                     $"Aspect: {info.aspectRatio:F2}, DPI: {info.dpi}");
            
            // 캔버스 해상도 지원 설정
            Canvas myCanvas = FindObjectOfType<Canvas>();
            resolution.SetupResolutionSupport(myCanvas);
        }
    }
}