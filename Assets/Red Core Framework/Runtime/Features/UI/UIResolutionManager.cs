using UnityEngine;
using UnityEngine.UI;

namespace RedMinS.UI
{
    /// <summary>
    /// UI 해상도 지원을 담당하는 클래스
    /// </summary>
    public class UIResolutionManager : MonoBehaviour
    {
        [Header("Resolution Settings")]
        [SerializeField] private Vector2 mobileReferenceResolution = new Vector2(1080f, 1920f);
        [SerializeField] private float mobileScaleFactor = 0.9f;
        
        private CanvasScaler canvasScaler;
        
        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                Debug.LogError("[UIResolutionManager] CanvasScaler component not found!");
            }
        }

        /// <summary>
        /// 캔버스 해상도 지원을 설정합니다
        /// </summary>
        public void SetupResolutionSupport(Canvas targetCanvas, GameObject gameSceneObject = null)
        {
            if (targetCanvas == null || canvasScaler == null) return;

            ConfigureMobileResolution(targetCanvas, gameSceneObject);
        }

        private void ConfigureMobileResolution(Canvas canvas, GameObject gameSceneObject)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            float aspectRatio = canvas.pixelRect.width / canvas.pixelRect.height;
            
            canvasScaler.referenceResolution = mobileReferenceResolution;
            canvasScaler.matchWidthOrHeight = 1f;

            // 세로가 긴 해상도인 경우 게임 오브젝트 스케일 조정
            if (aspectRatio <= 0.5f && gameSceneObject != null)
            {
                gameSceneObject.transform.localScale = Vector3.one * mobileScaleFactor;
            }

            Debug.Log($"[UIResolutionManager] Mobile resolution configured. Aspect ratio: {aspectRatio:F3}");
#endif
        }

        /// <summary>
        /// 현재 해상도 정보를 반환합니다
        /// </summary>
        public ResolutionInfo GetCurrentResolutionInfo()
        {
            return new ResolutionInfo
            {
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                aspectRatio = (float)Screen.width / Screen.height,
                dpi = Screen.dpi
            };
        }
    }

    [System.Serializable]
    public struct ResolutionInfo
    {
        public int screenWidth;
        public int screenHeight;
        public float aspectRatio;
        public float dpi;
    }
}