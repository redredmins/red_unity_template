using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RedMinS.UI
{
    /// <summary>
    /// UI 효과(페이드, 트랜지션 등)를 관리하는 클래스
    /// </summary>
    public class UIEffectManager : MonoBehaviour
    {
        [Header("Fade Effect")]
        [SerializeField] private Animator fadeAnimator;
        [SerializeField] private Image fadeBoard;
        [SerializeField] private float defaultFadeDuration = 0.5f;

        [Header("Animation Triggers")]
        [SerializeField] private string fadeInTrigger = "FADE_IN";
        [SerializeField] private string fadeOutTrigger = "FADE_OUT";

        private WaitForSeconds halfSecondWait;
        private WaitForSeconds customWait;

        /// <summary>
        /// 현재 페이드 효과가 진행 중인지 확인
        /// </summary>
        public bool IsFading { get; private set; }

        private void Awake()
        {
            halfSecondWait = new WaitForSeconds(defaultFadeDuration);
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (fadeAnimator == null)
            {
                Debug.LogWarning("[UIEffectManager] Fade animator is not assigned.");
            }

            if (fadeBoard == null)
            {
                Debug.LogWarning("[UIEffectManager] Fade board is not assigned.");
            }
        }

        /// <summary>
        /// 페이드 인 효과 (화면을 밝게)
        /// </summary>
        public void FadeIn()
        {
            StartCoroutine(FadeInCoroutine(defaultFadeDuration));
        }

        /// <summary>
        /// 페이드 인 효과 (지정된 시간)
        /// </summary>
        public void FadeIn(float duration)
        {
            StartCoroutine(FadeInCoroutine(duration));
        }

        /// <summary>
        /// 페이드 아웃 효과 (화면을 어둡게, 기본 검은색)
        /// </summary>
        public void FadeOut()
        {
            StartCoroutine(FadeOutCoroutine(Color.black, defaultFadeDuration));
        }

        /// <summary>
        /// 페이드 아웃 효과 (지정된 색상)
        /// </summary>
        public void FadeOut(Color screenColor)
        {
            StartCoroutine(FadeOutCoroutine(screenColor, defaultFadeDuration));
        }

        /// <summary>
        /// 페이드 아웃 효과 (지정된 색상과 시간)
        /// </summary>
        public void FadeOut(Color screenColor, float duration)
        {
            StartCoroutine(FadeOutCoroutine(screenColor, duration));
        }

        /// <summary>
        /// 페이드 인 코루틴
        /// </summary>
        public IEnumerator FadeInCoroutine()
        {
            return FadeInCoroutine(defaultFadeDuration);
        }

        /// <summary>
        /// 페이드 인 코루틴 (지정된 시간)
        /// </summary>
        public IEnumerator FadeInCoroutine(float duration)
        {
            if (fadeAnimator == null)
            {
                Debug.LogWarning("[UIEffectManager] Cannot perform fade in - animator not assigned.");
                yield break;
            }

            IsFading = true;

            fadeAnimator.SetTrigger(fadeInTrigger);
            
            yield return duration <= 0 ? halfSecondWait : new WaitForSeconds(duration);

            IsFading = false;
        }

        /// <summary>
        /// 페이드 아웃 코루틴
        /// </summary>
        public IEnumerator FadeOutCoroutine(Color screenColor)
        {
            return FadeOutCoroutine(screenColor, defaultFadeDuration);
        }

        /// <summary>
        /// 페이드 아웃 코루틴 (지정된 시간)
        /// </summary>
        public IEnumerator FadeOutCoroutine(Color screenColor, float duration)
        {
            if (fadeAnimator == null || fadeBoard == null)
            {
                Debug.LogWarning("[UIEffectManager] Cannot perform fade out - components not assigned.");
                yield break;
            }

            IsFading = true;

            // 페이드 보드 색상 설정 (투명도는 0으로)
            fadeBoard.color = new Color(screenColor.r, screenColor.g, screenColor.b, 0f);
            
            fadeAnimator.SetTrigger(fadeOutTrigger);
            
            yield return duration <= 0 ? halfSecondWait : new WaitForSeconds(duration);

            IsFading = false;
        }

        /// <summary>
        /// 즉시 화면을 특정 색상으로 설정
        /// </summary>
        public void SetScreenColor(Color color)
        {
            if (fadeBoard != null)
            {
                fadeBoard.color = color;
            }
        }

        /// <summary>
        /// 즉시 화면을 투명하게 설정
        /// </summary>
        public void ClearScreen()
        {
            if (fadeBoard != null)
            {
                fadeBoard.color = Color.clear;
            }
        }

        /// <summary>
        /// 페이드 효과 중단
        /// </summary>
        public void StopAllFadeEffects()
        {
            StopAllCoroutines();
            IsFading = false;
        }

        /// <summary>
        /// 기본 페이드 시간 설정
        /// </summary>
        public void SetDefaultFadeDuration(float duration)
        {
            defaultFadeDuration = Mathf.Max(0.1f, duration);
            halfSecondWait = new WaitForSeconds(defaultFadeDuration);
        }

        /// <summary>
        /// 크로스 페이드 효과 (페이드 아웃 → 액션 실행 → 페이드 인)
        /// </summary>
        public IEnumerator CrossFade(System.Action action, Color fadeColor, float fadeDuration = -1f)
        {
            float duration = fadeDuration > 0 ? fadeDuration : defaultFadeDuration;

            // 페이드 아웃
            yield return StartCoroutine(FadeOutCoroutine(fadeColor, duration));

            // 액션 실행
            action?.Invoke();

            // 잠깐 대기
            yield return new WaitForEndOfFrame();

            // 페이드 인
            yield return StartCoroutine(FadeInCoroutine(duration));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            defaultFadeDuration = Mathf.Max(0.1f, defaultFadeDuration);
        }

        [ContextMenu("Test Fade In")]
        private void TestFadeIn()
        {
            if (Application.isPlaying)
                FadeIn();
        }

        [ContextMenu("Test Fade Out")]
        private void TestFadeOut()
        {
            if (Application.isPlaying)
                FadeOut();
        }
#endif
    }
}