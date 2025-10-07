using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

namespace RedMinS.UI
{
    [System.Serializable]
    public class SystemPopupMessage
    {
        public string message = string.Empty;
        public string okText = string.Empty;
        public string cancelText = string.Empty;
        public UnityAction okAction = null;
        public UnityAction cancelAction = null;
    }

    public class UISystemPopup : UIPopup
    {
        [SerializeField] TextMeshProUGUI txtContent = null;

        [Header("- 1 button set")]
        [SerializeField] GameObject oneBtnSet = null;
        [SerializeField] Button btnOne = null;
        [SerializeField] TextMeshProUGUI labBtnOne = null;

        [Header("- 2 button set")]
        [SerializeField] GameObject twoBtnSet = null;
        [SerializeField] Button btnTwoYes = null;
        [SerializeField] Button btnTwoNo = null;
        [SerializeField] TextMeshProUGUI labBtnTwoYes = null;
        [SerializeField] TextMeshProUGUI labBtnTwoNo = null;

        UnityAction BackKeyAction = null;


        protected override void OnEnable()
        {
            base.OnEnable();

            // btnOne.onClick.AddListener(() => { PlayBtnSound(); });
            // btnTwoYes.onClick.AddListener(() => { PlayBtnSound(); });
            // btnTwoNo.onClick.AddListener(() => { PlayBtnSound(); });
        }

        protected override void OnDisable()
        {
            btnOne.onClick.RemoveAllListeners();
            btnTwoYes.onClick.RemoveAllListeners();
            btnTwoNo.onClick.RemoveAllListeners();

            oneBtnSet.SetActive(false);
            twoBtnSet.SetActive(false);

            //_systemMessage = null;
            //_checkList = null;

            base.OnDisable();
        }

        public override void OnBackKeyAction()
        {
            //base.OnBackKeyAction();

            if (BackKeyAction != null)
            {
                BackKeyAction();
            }
            else
                base.OnBackKeyAction();
        }

        //SystemPopupMessage _systemMessage;
        //List<string> _checkList;
        public void SetSystemPopup(SystemPopupMessage systemMessage, UnityAction<string, GameObject> removeSelf)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1f, 1f, 1f);

            txtContent.text = systemMessage.message;
            UnityAction removeSelfAction = () => { removeSelf(systemMessage.message, this.gameObject); };

            if (systemMessage.okText != string.Empty)
            {
                if (systemMessage.cancelText != string.Empty) // --- yes, no 버튼 다 있으면
                {
                    oneBtnSet.SetActive(false);
                    twoBtnSet.SetActive(true);
                    labBtnTwoYes.text = systemMessage.okText;
                    labBtnTwoNo.text = systemMessage.cancelText;
                    if (systemMessage.okAction != null)
                    {
                        btnTwoYes.onClick.AddListener(systemMessage.okAction);
                    }
                    btnTwoYes.onClick.AddListener(removeSelfAction);
                    if (systemMessage.cancelAction != null)
                    {
                        btnTwoNo.onClick.AddListener(systemMessage.cancelAction);
                        BackKeyAction = systemMessage.cancelAction;
                    }
                    btnTwoNo.onClick.AddListener(removeSelfAction);
                    BackKeyAction += removeSelfAction;
                }
                else        // --- yes 버튼 있으면
                {
                    oneBtnSet.SetActive(true);
                    twoBtnSet.SetActive(false);
                    labBtnOne.text = systemMessage.okText;
                    if (systemMessage.okAction != null)
                    {
                        btnOne.onClick.AddListener(systemMessage.okAction);
                        BackKeyAction = systemMessage.okAction;
                    }
                    btnOne.onClick.AddListener(removeSelfAction);
                    BackKeyAction += removeSelfAction;
                }
            }
            else            // --- 메시지 확인만 가능
            {
                oneBtnSet.SetActive(true);
                twoBtnSet.SetActive(false);
                labBtnOne.text = Core.app.table.uiString.GetString(3);
                btnOne.onClick.AddListener(removeSelfAction);
                BackKeyAction = removeSelfAction;
            }
        }
    }
}