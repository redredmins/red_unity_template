using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS.Tool
{
    public class DestroySelf : MonoBehaviour
    {
        [SerializeField] float destoryTime;


        IEnumerator Start()
        {
            if (destoryTime > 0)
                yield return new WaitForSeconds(destoryTime);

            Destroy(gameObject);
        }
    }
}
