using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedMinS.UI;

namespace RedMinS
{
    public class AppManager : SingletonMonobehaviour<AppManager>
    {
        [Header("UI Management")]
        [SerializeField] private UIManager _ui;
        public UIManager ui => GetOrCreateModule<UIManager>(ref _ui);

        [Header("Scene Management")]
        [SerializeField] private SceneLoader _scene;
        public SceneLoader scene => GetOrCreateModule<SceneLoader>(ref _scene);

        [Header("Data Management")]
        [SerializeField] private DesignDataContainer _table;
        public DesignDataContainer table => GetOrCreateModule<DesignDataContainer>(ref _table);

        [Header("Sound Management")]
        [SerializeField] private SoundManager _sound;
        public SoundManager sound => GetOrCreateModule<SoundManager>(ref _sound);

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            InitializeModules();
        }

        private void InitializeModules()
        {
            Debug.Log("[AppManager] Initializing modules...");
            
            // 모듈들의 초기화를 강제로 트리거
            _ = ui;
            _ = scene;
            _ = table;
            _ = sound;
            
            Debug.Log("[AppManager] All modules initialized successfully.");
        }

        /// <summary>
        /// MonoBehaviour 모듈을 가져오거나 생성합니다.
        /// </summary>
        private M GetOrCreateModule<M>(ref M member) where M : MonoBehaviour
        {
            if (member == null)
            {
                // 먼저 씬에서 찾아보기
                member = FindAnyObjectByType<M>();
                
                if (member == null)
                {
                    // 없으면 새로 생성
                    var obj = new GameObject($"[Module]{typeof(M).Name}");
                    obj.transform.SetParent(this.transform);
                    member = obj.AddComponent<M>();
                    
                    Debug.Log($"[AppManager] Created new module: {typeof(M).Name}");
                }
                else
                {
                    Debug.Log($"[AppManager] Found existing module: {typeof(M).Name}");
                }
            }
            return member;
        }

        /// <summary>
        /// 일반 클래스 모듈을 가져오거나 생성합니다.
        /// </summary>
        private M GetOrCreateNewModule<M>(ref M member) where M : new()
        {
            if (member == null)
            {
                member = new M();
                Debug.Log($"[AppManager] Created new data module: {typeof(M).Name}");
            }
            return member;
        }

        /// <summary>
        /// 애플리케이션 일시정지/재개
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("[AppManager] Application paused");
                // 일시정지 시 필요한 작업 수행
            }
            else
            {
                Debug.Log("[AppManager] Application resumed");
                // 재개 시 필요한 작업 수행
            }
        }

        /// <summary>
        /// 애플리케이션 포커스 변경
        /// </summary>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("[AppManager] Application gained focus");
            }
            else
            {
                Debug.Log("[AppManager] Application lost focus");
            }
        }
    }
}