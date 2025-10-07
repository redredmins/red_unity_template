using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedMinS
{
    public struct Order
    {
        public UnityAction order { private set; get; }
        public float during { private set; get; }

        public Order(float during, UnityAction order)
        {
            this.during = during;
            this.order = order;
        }
    }

    public struct Order<T>
    {
        public UnityAction<T> order { private set; get; }
        public float during { private set; get; }

        public Order(float during, UnityAction<T> order)
        {
            this.during = during;
            this.order = order;
        }
    }

    public class CoroutineOperator
    {
        public WaitForSeconds halfSec = new WaitForSeconds(0.5f);
        public WaitForSeconds oneSec = new WaitForSeconds(1f);

        MonoBehaviour _main;
        Dictionary<string, IEnumerator> _coroutines;
        

        public CoroutineOperator(MonoBehaviour main)
        {
            _main = main;
            _coroutines = new Dictionary<string, IEnumerator>();
        }

        public void StartMyCoroutine(string key, IEnumerator coroutine)
        {
            //if((_coroutines ??= new Dictionary<string, IEnumerator>()).ContainsKey(key) ==  true)
            if (_coroutines.ContainsKey(key) == true)
            {
                if (_coroutines[key] != null) _main.StopCoroutine(_coroutines[key]);
                _coroutines[key] = coroutine;
            }
            else
            {
                _coroutines.Add(key, coroutine);
            }

            _main.StartCoroutine(_coroutines[key]);
        }

        public void StopMyCoroutin(string key)
        {
            if (_coroutines.ContainsKey(key) == true)
            {
                if (_coroutines[key] != null)
                {
                    _main.StopCoroutine(_coroutines[key]);
                    _coroutines.Remove(key);
                }
            }
        }

        public void StopMyAllCoroutin()
        {
            _main.StopAllCoroutines();
            _coroutines.Clear();
        }

        public void WaitAndAction(string key, TimeSpan wait, UnityAction action)
        {
            StartMyCoroutine(key, IEWaitAndAction(wait, action));
        }

        public void ExecuteInOrder(string key, params Order[] orders)
        {
            StartMyCoroutine(key, IEExecuteInOrder(orders));
        }

        IEnumerator IEWaitAndAction(TimeSpan wait, UnityAction action)
        {
            double c = wait.TotalSeconds;
            while (c > 0)
            {
                yield return oneSec;
                --c;
            }
            action();
        }

        IEnumerator IEExecuteInOrder(Order[] orders)
        {
            for (int i = 0; i < orders.Length; ++i)
            {
                orders[i].order();

                if (orders[i].during > 0f)
                    yield return new WaitForSeconds(orders[i].during);
            }
        }
    }

}