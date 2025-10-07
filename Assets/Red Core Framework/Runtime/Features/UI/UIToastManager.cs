using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS.UI
{
    /// <summary>
    /// UI 토스트 메시지 시스템을 관리하는 클래스
    /// </summary>
    public class UIToastManager : MonoBehaviour
    {
        [Header("Toast Settings")]
        [SerializeField] private GameObject toastPrefab;
        [SerializeField] private Transform toastContainer;
        [SerializeField] private float toastDisplayDuration = 1.5f;
        [SerializeField] private int maxQueueSize = 10;

        private readonly Queue<string> toastMessageQueue = new Queue<string>();
        private readonly HashSet<string> activeMessages = new HashSet<string>();
        private ObjectPool toastPool;
        private Coroutine toastCoroutine;

        /// <summary>
        /// 현재 대기 중인 토스트 메시지 수
        /// </summary>
        public int QueuedMessageCount => toastMessageQueue.Count;

        /// <summary>
        /// 토스트 시스템이 활성화되어 있는지 확인
        /// </summary>
        public bool IsToastActive => toastCoroutine != null;

        private void Awake()
        {
            toastPool = new ObjectPool();
            
            // 컨테이너가 설정되지 않았다면 Canvas를 찾아서 설정
            if (toastContainer == null)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    toastContainer = canvas.transform;
                }
            }
        }

        /// <summary>
        /// 토스트 메시지를 표시 (문자열)
        /// </summary>
        public void ShowToast(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("[UIToastManager] Cannot show empty toast message.");
                return;
            }

            // 중복 메시지 방지
            if (activeMessages.Contains(message))
            {
                Debug.Log($"[UIToastManager] Toast message '{message}' is already active.");
                return;
            }

            // 큐 크기 제한
            if (toastMessageQueue.Count >= maxQueueSize)
            {
                Debug.LogWarning($"[UIToastManager] Toast queue is full. Removing oldest message.");
                string oldestMessage = toastMessageQueue.Dequeue();
                activeMessages.Remove(oldestMessage);
            }

            toastMessageQueue.Enqueue(message);
            activeMessages.Add(message);

            // 첫 번째 메시지라면 코루틴 시작
            if (toastCoroutine == null)
            {
                toastCoroutine = StartCoroutine(ProcessToastQueue());
            }
        }

        /// <summary>
        /// 토스트 메시지를 표시 (문자열 테이블 ID)
        /// </summary>
        public void ShowToast(int stringId)
        {
            if (Core.app?.table?.uiString != null)
            {
                string message = Core.app.table.uiString.GetString(stringId);
                ShowToast(message);
            }
            else
            {
                Debug.LogError("[UIToastManager] Cannot access string table. Core system not initialized?");
            }
        }

        /// <summary>
        /// 즉시 토스트 메시지 표시 (큐를 거치지 않음)
        /// </summary>
        public void ShowImmediateToast(string message)
        {
            if (toastPrefab == null)
            {
                Debug.LogError("[UIToastManager] Toast prefab is not assigned!");
                return;
            }

            StartCoroutine(ShowSingleToast(message));
        }

        /// <summary>
        /// 토스트 큐를 모두 비우기
        /// </summary>
        public void ClearToastQueue()
        {
            toastMessageQueue.Clear();
            activeMessages.Clear();
            
            if (toastCoroutine != null)
            {
                StopCoroutine(toastCoroutine);
                toastCoroutine = null;
            }
            
            Debug.Log("[UIToastManager] Toast queue cleared.");
        }

        private IEnumerator ProcessToastQueue()
        {
            while (toastMessageQueue.Count > 0)
            {
                string message = toastMessageQueue.Peek();
                
                yield return StartCoroutine(ShowSingleToast(message));
                
                // 메시지 처리 완료
                toastMessageQueue.Dequeue();
                activeMessages.Remove(message);
            }

            toastCoroutine = null;
        }

        private IEnumerator ShowSingleToast(string message)
        {
            if (toastPrefab == null || toastContainer == null)
            {
                Debug.LogError("[UIToastManager] Toast prefab or container is not assigned!");
                yield break;
            }

            // 토스트 오브젝트 생성
            GameObject toastObject = toastPool.CreateObject(toastPrefab, toastContainer);
            UIToast toast = toastObject.GetComponent<UIToast>();

            if (toast != null)
            {
                toast.SetToast(message);
                Debug.Log($"[UIToastManager] Showing toast: {message}");
            }
            else
            {
                Debug.LogError("[UIToastManager] UIToast component not found on toast prefab!");
            }

            // 표시 시간 대기
            yield return new WaitForSeconds(toastDisplayDuration);

            // 토스트 오브젝트 제거
            toastPool.RemoveObject(toastObject);
        }

        /// <summary>
        /// 토스트 표시 시간 설정
        /// </summary>
        public void SetToastDuration(float duration)
        {
            toastDisplayDuration = Mathf.Max(0.1f, duration);
        }

        /// <summary>
        /// 최대 큐 크기 설정
        /// </summary>
        public void SetMaxQueueSize(int size)
        {
            maxQueueSize = Mathf.Max(1, size);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            toastDisplayDuration = Mathf.Max(0.1f, toastDisplayDuration);
            maxQueueSize = Mathf.Max(1, maxQueueSize);
        }
#endif
    }
}