using UnityEngine;
using System.Collections.Generic;

namespace RedMinS
{
    public class ObjectPool //: MonoBehaviour
    {
        Dictionary<string, List<GameObject>> _objPools = null;


        public ObjectPool()
        {
            _objPools = new Dictionary<string, List<GameObject>>();
        }

        // 
        void MakePool(GameObject prefab)
        {
            _objPools.Add(prefab.name, new List<GameObject>());
        }

        GameObject MakeObject(GameObject prefab, Transform parent)
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            obj.name = prefab.name;

            return obj;
        }

        // 오브젝트풀에서 꺼내서 사용
        public GameObject CreateObject(GameObject prefab, Transform parent)
        {
            if (prefab == null) return null;

            string objName = prefab.name;
            if (_objPools.ContainsKey(objName) == false)
            {
                MakePool(prefab);
            }

            GameObject obj = null;
            if (_objPools[objName].Count > 0)
            {
                obj = _objPools[objName][0];
                _objPools[objName].Remove(obj);
                obj.transform.SetParent(parent);
                obj.SetActive(true);

                //Debug.Log("CreateObject - re", obj);
            }
            else
            {
                obj = MakeObject(prefab, parent);
                //Debug.Log("CreateObject - new", obj);
            }

            obj.transform.localScale = new Vector3(1f, 1f, 1f);

            return obj;
        }

        // 오브젝트풀에 다시 넣어둠
        public void RemoveObject(GameObject obj)
        {
            //Debug.Log(obj.name);
            obj.SetActive(false);
            //obj.transform.SetParent(this.transform);
            _objPools[obj.name].Add(obj);
            //Debug.Log("RemoveObject", obj);
        }

        // 풀의 오브젝트 모두 제거
        public void ClearPool()
        {
            foreach (var objs in _objPools)
            {
                //AnalyticsManager.DebugLog(this.transform.name + " => 오브젝트풀 : 풀의 모든 오브젝트 제거");
                int numObjs = objs.Value.Count - 1;
                for (int i = numObjs; i >= 0; --i)
                {
                    Object.Destroy(objs.Value[i].gameObject);
                }
            }

            _objPools.Clear();
        }

    }
}