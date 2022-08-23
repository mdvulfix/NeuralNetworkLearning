using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Console
{

    public class TestAsync : MonoBehaviour
    {

        private List<Awaiter> m_Pool;
        private List<Awaiter> m_Awaiter;

        private Queue<Func<bool>> m_Funcs;

        private void Awake()
        {
            m_Pool = new List<Awaiter>(10);
            m_Funcs = new Queue<Func<bool>>(50);

            for (int i = 0; i < 10; i++)
                m_Pool.Add(Awaiter.Get());

            m_Funcs.Enqueue(OperationAsync_1);
            m_Funcs.Enqueue(OperationAsync_2);
            m_Funcs.Enqueue(OperationAsync_3);
            m_Funcs.Enqueue(OperationAsync_4);
            m_Funcs.Enqueue(OperationAsync_5);
        }

        private void Start()
        {

            var awaitersInReadyStateNumber = 5;
            var awaitersAwaliable = m_Pool.Count - awaitersInReadyStateNumber;

            if (awaitersAwaliable < 0)
            {
                for (int i = 0; i < -1 * awaitersAwaliable; i++)
                    m_Pool.Add(Awaiter.Get());
            }

            for (int i = 0; i < awaitersInReadyStateNumber; i++)
            {
                var awaiter = m_Pool[i];
                awaiter.Load();

            }

            ExecuteAsync();

        }

        private void OnAwaiterInitialized(Awaiter awaiter)
        {
            m_Pool.Remove(awaiter);
            m_Awaiter.Add(awaiter);
        }

        private void OnAwaiterDisposed(Awaiter awaiter)
        {
            m_Awaiter.Remove(awaiter);
            m_Pool.Add(awaiter);
        }


        private void OnEnable()
        {

            foreach (var awaiter in m_Pool)
            {
                awaiter.Initialized += OnAwaiterInitialized;

            }

        }

        private void OnDisable()
        {
            foreach (var awaiter in m_Pool)
            {
                awaiter.Initialized -= OnAwaiterInitialized;

            }
        }

        private bool OperationAsync_1()
        {
            Debug.Log("Waiting operation 1...");
            return false;
        }

        private bool OperationAsync_2()
        {
            Debug.Log("Waiting operation 2...");
            return false;
        }

        private bool OperationAsync_3()
        {
            Debug.Log("Waiting operation 3...");
            return false;
        }

        private bool OperationAsync_4()
        {
            Debug.Log("Waiting operation 4...");
            return false;
        }

        private bool OperationAsync_5()
        {
            Debug.Log("Waiting operation 5...");
            return false;
        }

        private void ExecuteAsync()
        {
            var awaitersInReadyState = from awaiter in m_Awaiter
                                       where awaiter.IsReady == true
                                       select awaiter;

            var awaitersInReadyStateNumber = awaitersInReadyState.Count();
            var funcsNumber = m_Funcs.Count;
            var awaitersAwaliable = awaitersInReadyStateNumber - funcsNumber;

            if (awaitersAwaliable < 0)
            {
                for (int i = 0; i < -1 * awaitersAwaliable; i++)
                {
                    var awaiter = m_Pool[i];
                    awaiter.Load();
                }

                awaitersInReadyState = from awaiter in m_Awaiter
                                       where awaiter.IsReady == true
                                       select awaiter;
            }

            var awaiters = awaitersInReadyState.ToArray();

            for (int i = 0; i < funcsNumber; i++)
                awaiters[i].Run(m_Funcs.Dequeue(), 2);

        }

    }

}
