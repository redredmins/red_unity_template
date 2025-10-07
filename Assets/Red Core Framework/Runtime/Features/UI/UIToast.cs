using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;

namespace RedMinS.UI
{
    public class UIToast : MonoBehaviour
    {
        [SerializeField] protected RectTransform moveRect;
        [SerializeField] protected TextMeshProUGUI txtMessage = null;


        public void SetToast(string message)
        {
            txtMessage.text = message;

            float yPos = moveRect.localPosition.y;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            this.transform.localPosition = new Vector3(0f, 0f, 0f);
            //moveRect.localScale = new Vector3(1f, 1f, 1f);
            moveRect.localPosition = new Vector3(0f, (yPos - 200f), 0f);
            moveRect.DOLocalMoveY(yPos, 0.3f);

            //if (ShowToastCoroutine != null) StopCoroutine(ShowToastCoroutine);
            //ShowToastCoroutine = IEShowToast(message);
            //StartCoroutine(ShowToastCoroutine);
        }

        IEnumerator ShowToastCoroutine;
        IEnumerator IEShowToast(string message)
        {
            yield return null;
        }

    }
}
