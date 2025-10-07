using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS.Tool
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject spawnObject;
        [SerializeField] float delayTime = 0f;
        [SerializeField] float duringTime = 1f;


        IEnumerator SpawnCoroutine;
        public void SpawnObject()
        {
            if (transform.childCount > 0) return;

            if(SpawnCoroutine != null) StopCoroutine(SpawnCoroutine);
            SpawnCoroutine = IESpawn();
            StartCoroutine(SpawnCoroutine);
        }

        IEnumerator IESpawn()
        {
            if (delayTime > 0)
                yield return new WaitForSeconds(delayTime);

            GameObject obj = Instantiate(spawnObject, Vector3.zero, Quaternion.identity, transform);
            obj.transform.localPosition = Vector3.zero;

            yield return new WaitForSeconds(duringTime);

            Destroy(obj);
        }
    }
}
