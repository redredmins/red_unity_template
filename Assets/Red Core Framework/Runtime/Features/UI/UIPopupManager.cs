using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace RedMinS.UI
{
    /// <summary>
    /// UI 팝업 시스템을 관리하는 클래스
    /// </summary>
    public class UIPopupManager : MonoBehaviour
    {
        [Header("Popup Settings")]
        [SerializeField] private GameObject systemPopupPrefab;
        [SerializeField] private Transform popupContainer;

        public event Action OnAllPopupsClosed;
        public UnityAction NoPopupAction;

        private readonly List<UIPopup> activePopups = new List<UIPopup>();
        private readonly List<string> systemMessageList = new List<string>();
        private ObjectPool popupPool;

        /// <summary>
        /// 현재 팝업이 활성화되어 있는지 확인
        /// </summary>
        public bool IsPopupActive => activePopups.Count > 0;

        /// <summary>
        /// 현재 활성화된 팝업 수
        /// </summary>
        public int ActivePopupCount => activePopups.Count;

        private void Awake()
        {
            popupPool = new ObjectPool();
            
            // 컨테이너가 설정되지 않았다면 Canvas를 찾아서 설정
            if (popupContainer == null)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    popupContainer = canvas.transform;
                }
            }
        }

        /// <summary>
        /// 팝업을 활성화 상태로 등록
        /// </summary>
        public void RegisterPopup(UIPopup popup)
        {
            if (popup != null && !activePopups.Contains(popup))
            {
                activePopups.Add(popup);
                OnPopupOpened(popup);
            }
        }

        /// <summary>
        /// 팝업을 비활성화 상태로 해제
        /// </summary>
        public void UnregisterPopup(UIPopup popup)
        {
            if (popup != null && activePopups.Contains(popup))
            {
                activePopups.Remove(popup);
                OnPopupClosed(popup);

                if (activePopups.Count == 0)
                {
                    NoPopupAction?.Invoke();
                    OnAllPopupsClosed?.Invoke();
                }
            }
        }

        /// <summary>
        /// 가장 위에 있는 팝업의 뒤로가기 액션을 실행
        /// </summary>
        public void HandleBackKey()
        {
            if (activePopups.Count > 0)
            {
                int lastIndex = activePopups.Count - 1;
                activePopups[lastIndex].OnBackKeyAction();
            }
        }

        /// <summary>
        /// 시스템 팝업을 표시 (메시지만)
        /// </summary>
        public void ShowSystemPopup(string message, UnityAction okAction = null)
        {
            ShowSystemPopup(false, message, okAction);
        }

        /// <summary>
        /// 시스템 팝업을 표시 (확인/취소)
        /// </summary>
        public void ShowSystemPopup(bool isSmallError, string message, UnityAction okAction = null, UnityAction cancelAction = null)
        {
            var uiString = Core.app.table.uiString;
            string okText = uiString.GetString(1); // "확인"
            string cancelText = uiString.GetString(2); // "취소"
            
            ShowSystemPopup(isSmallError, message, okText, okAction, cancelText, cancelAction);
        }

        /// <summary>
        /// 시스템 팝업을 표시 (상세 설정)
        /// </summary>
        public void ShowSystemPopup(bool isSmallError, string message, string okText, UnityAction okAction, string cancelText = null, UnityAction cancelAction = null)
        {
            if (systemMessageList.Contains(message))
            {
                Debug.Log($"[UIPopupManager] System popup with message '{message}' is already showing.");
                return;
            }

            systemMessageList.Add(message);

            var systemMessage = new SystemPopupMessage
            {
                message = message,
                okText = okText,
                okAction = okAction,
                cancelText = cancelText,
                cancelAction = cancelAction
            };

            CreateSystemPopup(isSmallError, systemMessage);
        }

        /// <summary>
        /// 강제 종료를 포함한 시스템 팝업 표시
        /// </summary>
        public void ShowSystemPopupAndQuit(string message, UnityAction action = null)
        {
            // 다른 시스템들을 중지
            if (Core.network != null) Core.network.StopAllCoroutines();
            if (Core.database != null) Core.database.StopAllCoroutines();

            ShowSystemPopup(false, message, () =>
            {
                action?.Invoke();
                Application.Quit();
            });
        }

        private void CreateSystemPopup(bool isSmallError, SystemPopupMessage systemMessage)
        {
            if (systemPopupPrefab == null)
            {
                Debug.LogError("[UIPopupManager] System popup prefab is not assigned!");
                return;
            }

            GameObject popupObject = popupPool.CreateObject(systemPopupPrefab, popupContainer);
            UISystemPopup systemPopup = popupObject.GetComponent<UISystemPopup>();
            
            if (systemPopup != null)
            {
                systemPopup.SetSystemPopup(systemMessage, RemoveSystemPopup);
            }
        }

        private void RemoveSystemPopup(string message, GameObject popupObject)
        {
            systemMessageList.Remove(message);
            popupPool.RemoveObject(popupObject);
        }

        private void OnPopupOpened(UIPopup popup)
        {
            Debug.Log($"[UIPopupManager] Popup opened: {popup.gameObject.name}");
            // 팝업 오픈 사운드 재생 등의 추가 로직
        }

        private void OnPopupClosed(UIPopup popup)
        {
            Debug.Log($"[UIPopupManager] Popup closed: {popup.gameObject.name}");
            // 팝업 클로즈 사운드 재생 등의 추가 로직
        }

        /// <summary>
        /// 모든 팝업을 강제로 닫기
        /// </summary>
        public void CloseAllPopups()
        {
            var popupsToClose = new List<UIPopup>(activePopups);
            foreach (var popup in popupsToClose)
            {
                popup.ClosePopup();
            }
        }

        /// <summary>
        /// 특정 타입의 팝업이 열려있는지 확인
        /// </summary>
        public bool IsPopupOpen<T>() where T : UIPopup
        {
            foreach (var popup in activePopups)
            {
                if (popup is T)
                    return true;
            }
            return false;
        }
    }

}