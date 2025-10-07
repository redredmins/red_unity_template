using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RedMinS
{
    public class SceneLoader : MonoBehaviour
    {
        public const string SCENE_TITLE = "TITLE";
        public const string SCENE_MAIN = "MAIN";

        private string _curScene;
        public string CurSecne
        {
            get { return _curScene; }
        }

        bool _isLoading = false;
        Dictionary<string, UnityAction> _reloadSceneAction;


        void Start()
        {
            _curScene = SCENE_TITLE;
            _reloadSceneAction = new Dictionary<string, UnityAction>();

            SceneManager.LoadSceneAsync(SCENE_TITLE, LoadSceneMode.Additive);
        }

        public void Load(string now, string toGo, UnityAction loadSceneAction = null)
        {
            if (_isLoading == false)
            {
                StartCoroutine(IELoadScene(now, toGo, loadSceneAction));
                //analytics.SetScene(toGo);
            }
        }

        WaitForSeconds halfSec = new WaitForSeconds(0.5f);
        IEnumerator IELoadScene(string now, string toLoad, UnityAction reloadSceneAction)
        {
            var ui = Core.app.ui;
            //Debug.Log(now + " -> " + toLoad);

            _isLoading = true;
            if (reloadSceneAction != null)
            {
                _reloadSceneAction.Add(now, reloadSceneAction);
            }

            yield return ui.IEFadeOut(Color.black);
            //ui.ShowLoadingSpinner(true);

            AsyncOperation async = null;
            SceneManager.UnloadSceneAsync(now);
            yield return halfSec;

            async = SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Additive);

            while (async != null && async.isDone == false)
            {
                yield return halfSec;
            }

            _curScene = toLoad;
            //ui.ShowLoadingSpinner(false);
            yield return ui.IEFadeIn();

            if (_reloadSceneAction.ContainsKey(toLoad) == true)
            {
                _reloadSceneAction[toLoad]();
                _reloadSceneAction.Remove(toLoad);
            }

            _isLoading = false;
        }

    }
}