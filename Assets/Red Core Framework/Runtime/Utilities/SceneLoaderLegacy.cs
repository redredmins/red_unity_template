using System.Collections;
using UnityEngine;
using UnityEngine.Events;
//using RedMinS.SceneManagement;

/*
namespace RedMinS
{
    /// <summary>
    /// 레거시 SceneLoader - 기존 코드 호환성을 위한 래퍼 클래스
    /// 새로운 코드에서는 AdvancedSceneManager를 사용하세요
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public const string SCENE_TITLE = "TITLE";
        public const string SCENE_MAIN = "MAIN";

        private string _curScene;
        public string CurSecne => _curScene;

        private bool _isLoading = false;

        void Start()
        {
            _curScene = SCENE_TITLE;
        }

        /// <summary>
        /// 씬 로드 (레거시 호환)
        /// </summary>
        public void Load(string now, string toGo, UnityAction loadSceneAction = null)
        {
            if (_isLoading) return;

            StartCoroutine(IELoadScene(now, toGo, loadSceneAction));
        }

        /// <summary>
        /// 레거시 씬 로딩 코루틴 (호환성 유지)
        /// </summary>
        private IEnumerator IELoadScene(string now, string toLoad, UnityAction reloadSceneAction)
        {
            _isLoading = true;

            // 새로운 시스템 사용
            var sceneManager = AdvancedSceneManager.Instance;
            bool loadCompleted = false;

            // 트랜지션 설정
            var transitionSettings = new SceneTransitionSettings
            {
                useFadeTransition = true,
                fadeColor = Color.black,
                fadeOutDuration = 0.5f,
                fadeInDuration = 0.5f,
                showLoadingScreen = false
            };

            sceneManager.LoadScene(toLoad, transitionSettings, () =>
            {
                _curScene = toLoad;
                reloadSceneAction?.Invoke();
                loadCompleted = true;
            });

            yield return new WaitUntil(() => loadCompleted);
            _isLoading = false;
        }
    }
}
*/