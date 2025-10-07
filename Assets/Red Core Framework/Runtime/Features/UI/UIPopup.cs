using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace RedMinS.UI
{
    public class UIPopup : MonoBehaviour
    {
        protected static UIManager _ui;
        protected static StringTable _uiString = null;


        protected virtual void OnEnable()
        {
            _ui = Core.app.ui;
            if (_uiString == null) _uiString = Core.app.table.uiString;
            //if (_sound == null) _sound = Core.app.sound;
            _ui.OnPopup(this);
        }

        protected virtual void OnDisable()
        {
            _ui.OffPopup(this);
        }

        // (안드로이드) Back키
        public virtual void OnBackKeyAction()
        {
            ClosePopup();
        }

        // 팝업창 닫기
        public virtual void ClosePopup()
        {
            gameObject.SetActive(false);
        }
    }
}