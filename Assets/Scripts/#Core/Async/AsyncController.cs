using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace APP
{
    public class AsyncController : Controller<AsyncController>, IController, IUpdateble
    {
        private AsyncControllerConfig m_Config;

        private static Transform ROOT_AWAITERS;
        private static Transform ROOT_AWAITERS_POOL;

        private static Pool m_AwaiterPool;
        private int m_AwaiterPoolLimit = 1;


        private List<Awaiter> m_AwaiterIsReady;
        private List<Awaiter> m_AwaiterIsBusy;

        private int m_AwaiterIsReadyLimit = 5;

        public event Action<FuncAsyncInfo> FuncAsyncExecuted;

        public AsyncController() { }
        public AsyncController(params object[] args) =>
            Configure(args);


        public override void Configure(params object[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is AsyncControllerConfig)
                    {
                        m_Config = (AsyncControllerConfig)arg;
                    }
                }
            }


            if (ROOT_AWAITERS == null)
                ROOT_AWAITERS = new GameObject("Awaiters").transform;

            if (ROOT_AWAITERS_POOL == null)
            {
                ROOT_AWAITERS_POOL = new GameObject("Pool").transform;
                ROOT_AWAITERS_POOL.transform.SetParent(ROOT_AWAITERS);
            }

            m_AwaiterPool = Pool.Get(new PoolConfig());
            m_AwaiterIsReady = new List<Awaiter>(m_AwaiterIsReadyLimit);
            m_AwaiterIsBusy = new List<Awaiter>(100);



            base.Configure(m_Config);
        }

        public override void Init()
        {
            PoolUpdate();
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));

            base.Init();
        }

        public override void Dispose()
        {

            base.Dispose();
        }

        public void Update()
        {
            PoolUpdate();
        }


        public bool GetAwaiter(out Awaiter awaiter)
        {
            awaiter = null;

            if (m_AwaiterIsReady.Count > m_AwaiterIsReadyLimit)
                awaiter = m_AwaiterIsReady[0];

            if ((m_AwaiterIsReady.Count <= m_AwaiterIsReadyLimit))
            {
                PoolUpdate();
                PoolPopAwaiter(out awaiter);
            }

            return true;
        }

        public void ExecuteAsync(Func<Action, IEnumerator> func)
        {
            if (GetAwaiter(out var awaiter))
            {
                awaiter.Run(func);
                FuncAsyncExecuted?.Invoke(new FuncAsyncInfo(awaiter, func));
            }
            else
            {
                throw new Exception("The awaiter was not found! Execution faild!");
            }
        }


        private bool PoolPopAwaiter(out Awaiter awaiter)
        {
            awaiter = null;

            if (m_AwaiterPool.Pop(out var instance) == false)
                return false;

            awaiter = (Awaiter)instance;
            awaiter.transform.SetParent(ROOT_AWAITERS);

            awaiter.Initialized += OnAwaiterInitialized;
            awaiter.Disposed += OnAwaiterDisposed;
            awaiter.FuncStarted += OnAwaiterBusy;
            awaiter.FuncCompleted += OnAwaiterFuncComplete;

            awaiter.Load();

            return true;
        }

        private void PoolPushAwaiter(Awaiter awaiter)
        {
            awaiter.transform.SetParent(ROOT_AWAITERS_POOL);

            awaiter.Initialized -= OnAwaiterInitialized;
            awaiter.Disposed -= OnAwaiterDisposed;
            awaiter.FuncStarted -= OnAwaiterBusy;
            awaiter.FuncCompleted -= OnAwaiterFuncComplete;

            awaiter.Unload();

            m_AwaiterPool.Push(awaiter);
        }


        private void PoolUpdate()
        {
            // Check pool limit;
            var awaiterInPoolNumber = m_AwaiterPool.Count;
            if (awaiterInPoolNumber < m_AwaiterPoolLimit)
            {
                var number = m_AwaiterPoolLimit - awaiterInPoolNumber;

                //for (int i = 0; i < number; i++)
                    //m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
            }

            // Check awaiters in ready state limit;
            var awaiterIsReadyNumber = m_AwaiterIsReady.Count;

            // If the limit is less than the current number of awaiters, push unnecessary awaiters in the pool
            if (awaiterIsReadyNumber > m_AwaiterIsReadyLimit)
            {
                var number = awaiterIsReadyNumber - m_AwaiterIsReadyLimit;
                for (int i = 0; i < number; i++)
                    PoolPushAwaiter(m_AwaiterIsReady[i]);
            }
            // else, pop awaiters from the pool in the number of missing up to the limit
            else
            {
                var number = m_AwaiterIsReadyLimit - awaiterIsReadyNumber;
                for (int i = 0; i < number; i++)
                    PoolPopAwaiter(out var awaiter);
            }
        }


        private void OnAwaiterInitialized(Awaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);

        }

        private void OnAwaiterDisposed(Awaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

        }

        private void OnAwaiterBusy(Awaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

            m_AwaiterIsBusy.Add(awaiter);
        }

        private void OnAwaiterFuncComplete(Awaiter awaiter)
        {
            if (m_AwaiterIsBusy.Contains(awaiter))
                m_AwaiterIsBusy.Remove(awaiter);

            m_AwaiterIsReady.Add(awaiter);
        }
    }

    public struct AsyncControllerConfig : IConfig
    {

    }

    public struct FuncAsyncInfo
    {
        public FuncAsyncInfo(Awaiter awaiter, Func<Action, IEnumerator> func)
        {
            FuncAsync = func;
            Awaiter = awaiter;
        }

        public Awaiter Awaiter { get; private set; }
        public Func<Action, IEnumerator> FuncAsync { get; private set; }

    }
}