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

        private ConfigAsyncController m_Config;

        private static Transform ROOT_AWAITERS;
        private static Transform ROOT_AWAITERS_POOL;

        private static Pool m_AwaiterPool;
        private int m_AwaiterPoolLimit = 5;


        private Awaiter m_AwaiterActive;

        private List<Awaiter> m_AwaiterIsReady;
        private int m_AwaiterIsReadyLimit = 3;

        private Action AwaiterCallback;

        public event Action<FuncAsyncInfo> FuncExecutedAsync;

        public AsyncController() { }
        public AsyncController(params object[] args) =>
            Configure(args);


        public override void Configure(params object[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is ConfigAsyncController)
                    {
                        m_Config = (ConfigAsyncController)arg;
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
            m_AwaiterIsReady = new List<Awaiter>(15);



            base.Configure(m_Config);
        }

        public override void Init()
        {
            PoolUpdate();


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


        public Awaiter GetAwaiter()
        {

            if (m_AwaiterIsReady.Count > 0)
            {
                var awaitersInReadyState = (from Awaiter awaiterIsReady in m_AwaiterIsReady
                                        where awaiterIsReady.IsReady == true
                                        select awaiterIsReady);
            
                if (awaitersInReadyState.Count() > 0)
                    return awaitersInReadyState.First();
            }

            if (PoolPopAwaiter(out var awaiterFromPool) == false)
            {
                PoolUpdate();
                PoolPopAwaiter(out awaiterFromPool);
            }

            return awaiterFromPool;
        }

        public void ExecuteAsync(FuncAsync func)
        {
            var awaiter = GetAwaiter();
            awaiter.Run(func);
            FuncExecutedAsync?.Invoke(new FuncAsyncInfo(awaiter, func));
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

                for (int i = 0; i < number; i++)
                    m_AwaiterPool.Push(Awaiter.Get(ROOT_AWAITERS_POOL));
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

        }

        private void OnAwaiterFuncComplete(Awaiter awaiter)
        {

        }
    }

    public struct ConfigAsyncController : IConfig
    {

    }

    public struct FuncAsyncInfo
    {
        public FuncAsyncInfo(Awaiter awaiter, FuncAsync func)
        {
            FuncAsync = func;
            Awaiter = awaiter;
        }

        public FuncAsync FuncAsync { get; private set; }
        public Awaiter Awaiter { get; private set; }
    }

    public delegate IEnumerator FuncAsync(Action callback);


}