using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP
{
    public class AsyncController : Controller<AsyncController>, IController, IUpdateble
    {
        private AsyncControllerConfig m_Config;

        private static List<Awaiter> m_AwaiterIsReady;
        private int m_AwaiterIsReadyLimit = 5;

        private static List<FuncAsyncInfo> m_FuncExecuteQueue;
        private Awaiter m_FuncQueueAwaiter;

        private PoolController<Awaiter> m_PoolController;


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


            if (m_AwaiterIsReady == null)
                m_AwaiterIsReady = new List<Awaiter>(m_AwaiterIsReadyLimit);

            if (m_FuncExecuteQueue == null)
                m_FuncExecuteQueue = new List<FuncAsyncInfo>(100);



            m_FuncQueueAwaiter = Awaiter.Get("FuncQueueAwaiter");

            m_PoolController = new PoolController<Awaiter>();

            base.Configure(m_Config);
        }

        public override void Init()
        {
            m_FuncQueueAwaiter.Init();

            m_PoolController.Configure(new PoolControllerConfig());
            m_PoolController.Init();



            base.Init();
        }

        public override void Dispose()
        {
            m_PoolController.Dispose();
            
            m_FuncQueueAwaiter.Dispose();          
            
            base.Dispose();
        }

        public void Update()
        {
            FuncQueueUpdate();

            m_PoolController.Update();
        }


        public void ExecuteAsync(Func<Action<bool>, IEnumerator> func)
        {
            try
            {
                if (GetAwaiter(out var awaiter))
                {
                    if (awaiter.IsReady == true)
                    {
                        awaiter.Run(func);
                        FuncAsyncExecuted?.Invoke(new FuncAsyncInfo(awaiter, func));
                        return;
                    }

                    m_FuncExecuteQueue.Add(new FuncAsyncInfo(awaiter, func));
                }

            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private bool GetAwaiter(out Awaiter awaiter)
        {
            awaiter = null;

            if ((m_AwaiterIsReady.Count <= m_AwaiterIsReadyLimit))
                if(PoolPopAwaiter(out awaiter) == true)
                    return true;


            if (m_AwaiterIsReady.Count > m_AwaiterIsReadyLimit)
            {
                awaiter = m_AwaiterIsReady[0];
                return true;
            }

            Debug.LogWarning("Awaiter not found...");
            return false;
        }

        private bool PopAwaiter(out Awaiter awaiter)
        {
            
            m_PoolController
            
            awaiter = null;

            if (m_AwaiterPool.Pop(out var instance) == false)
                return false;

            awaiter = (Awaiter)instance;

            awaiter.Initialized += OnAwaiterInitialized;
            awaiter.Disposed += OnAwaiterDisposed;
            awaiter.FuncStarted += OnAwaiterBusy;
            awaiter.FuncCompleted += OnAwaiterFuncComplete;

            awaiter.Init();
            return true;
        }

        private void PoolPushAwaiter(Awaiter awaiter)
        {
            awaiter.Initialized -= OnAwaiterInitialized;
            awaiter.Disposed -= OnAwaiterDisposed;
            awaiter.FuncStarted -= OnAwaiterBusy;
            awaiter.FuncCompleted -= OnAwaiterFuncComplete;

            awaiter.Dispose();

            m_AwaiterPool.Push(awaiter);
        }






        private void PoolUpdate()
        {
            PoolCheckLimit();

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

        
        private void FuncQueueUpdate()
        {
            if(m_FuncQueueAwaiter.IsReady)
                m_FuncQueueAwaiter.Run(FuncQueueExecuteAsync);
        }
        
        
        private IEnumerator FuncQueueExecuteAsync(Action<bool> callback)
        {
            var funcsReadyToBeExecuted = (from FuncAsyncInfo funcInfo in m_FuncExecuteQueue
                                          where funcInfo.Awaiter.IsReady == true
                                          select funcInfo).ToArray();


            if (funcsReadyToBeExecuted.Length > 0)
            {
                foreach (var info in funcsReadyToBeExecuted)
                {
                    if(m_FuncExecuteQueue.Contains(info))
                        m_FuncExecuteQueue.Remove(info);
                    
                    info.Awaiter.Run(info.FuncAsync);
                    FuncAsyncExecuted?.Invoke(info);
                }
            }

            yield return null;
            
            callback.Invoke(true);
        }


        private void OnAwaiterInitialized(Awaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);

            PoolUpdate();
        }

        private void OnAwaiterDisposed(Awaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

            PoolUpdate();
        }

        private void OnAwaiterBusy(Awaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

            m_AwaiterIsBusy.Add(awaiter);
            PoolUpdate();
        }

        private void OnAwaiterFuncComplete(Awaiter awaiter)
        {
            if (m_AwaiterIsBusy.Contains(awaiter))
                m_AwaiterIsBusy.Remove(awaiter);

            m_AwaiterIsReady.Add(awaiter);
            PoolUpdate();
        }
    }

    public struct AsyncControllerConfig : IConfig
    {

    }

    public struct FuncAsyncInfo
    {
        public Awaiter Awaiter { get; private set; }
        public Func<Action<bool>, IEnumerator> FuncAsync { get; private set; }

        public FuncAsyncInfo(Awaiter awaiter, Func<Action<bool>, IEnumerator> func)
        {
            FuncAsync = func;
            Awaiter = awaiter;
        }
    }
}