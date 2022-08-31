using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Pool;

namespace APP
{
    public class AsyncController : AController, IController, IUpdateble
    {

        private static List<Awaiter> m_AwaiterIsReady;
        private int m_AwaiterIsReadyLimit = 5;

        private static List<FuncAsyncInfo> m_FuncExecuteQueue;
        private IAwaiter m_FuncQueueAwaiter;

        //private PoolController<Awaiter> m_PoolController;

        public event Action<FuncAsyncInfo> FuncAsyncExecuted;

        public AsyncController() { }
        public AsyncController(params object[] args)
        {
            Configure(args);
            Init();
        }


        public override void Configure(params object[] args)
        {
            var config = (AsyncControllerConfig)args[PARAM_INDEX_Config];
            
            if (m_AwaiterIsReady == null)
                m_AwaiterIsReady = new List<Awaiter>(m_AwaiterIsReadyLimit);

            if (m_FuncExecuteQueue == null)
                m_FuncExecuteQueue = new List<FuncAsyncInfo>(100);

            base.Configure(args);
        }

        public override void Init()
        {
            var awaiterConfig = new AwaiterConfig("FuncQueueAwaiter");
            //m_FuncQueueAwaiter = Awaiter.Get(awaiterConfig);

            

            //var poolableFactory = new FactoryAwaiter();
            //var poolControllerConfig = new PoolControllerConfig(poolableFactory);
            
            //m_PoolController = PoolController<Awaiter>.Get(poolControllerFactory, poolControllerConfig);
            
            
            
            
            m_FuncQueueAwaiter.Init();
            //m_PoolController.Init();



            base.Init();
        }

        public override void Dispose()
        {
            //m_PoolController.Dispose();
            m_FuncQueueAwaiter.Dispose();

            base.Dispose();
        }

        public void Update()
        {
            //m_PoolController.Update();
            
            LimitUpdate();
            FuncQueueUpdate();
        }


        public void ExecuteAsync(Func<Action<bool>, IEnumerator> func)
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

        private bool GetAwaiter(out Awaiter awaiter)
        {
            awaiter = null;
            
            //if ((m_AwaiterIsReady.Count < m_AwaiterIsReadyLimit))
            //    LimitUpdate();

            //awaiter = m_AwaiterIsReady[0];
            return true;
        }
        
        
        private bool PopAwaiter(out Awaiter awaiter)
        {
            awaiter = null;
            /*
            try
            {
                if (m_PoolController.Pop(out awaiter))
                {
                    awaiter.Initialized += OnAwaiterInitialized;
                    awaiter.Disposed += OnAwaiterDisposed;
                    awaiter.FuncStarted += OnAwaiterBusy;
                    awaiter.FuncCompleted += OnAwaiterFuncComplete;

                    awaiter.Init();
                    return true;
                }
            }
            catch (Exception exception) { ($"Pop awaiter is failed! Exception: {exception.Message}").Send(LogFormat.Warning); }
    

            ($"Pop awaiter not found!").Send(LogFormat.Warning);
            */
            return false;
            
        }

        private void PushAwaiter(Awaiter awaiter)
        {
            /*
            awaiter.Initialized -= OnAwaiterInitialized;
            awaiter.Disposed -= OnAwaiterDisposed;
            awaiter.FuncStarted -= OnAwaiterBusy;
            awaiter.FuncCompleted -= OnAwaiterFuncComplete;
            awaiter.Dispose();

            m_PoolController.Push(awaiter);
            */
        }
        

        private void LimitUpdate()
        {
            // Check awaiters in ready state limit;
            var awaiterIsReadyNumber = m_AwaiterIsReady.Count;

            // If the limit is less than the current number of awaiters, push unnecessary awaiters in the pool
            if (awaiterIsReadyNumber > m_AwaiterIsReadyLimit)
            {
                var number = awaiterIsReadyNumber - m_AwaiterIsReadyLimit;
                for (int i = 0; i < number; i++)
                    PushAwaiter(m_AwaiterIsReady[i]);
            }
            // else, pop awaiters from the pool in the number of missing up to the limit
            else
            {
                var number = m_AwaiterIsReadyLimit - awaiterIsReadyNumber;
                for (int i = 0; i < number; i++)
                    PopAwaiter(out var awaiter);
            }
        }


        private void FuncQueueUpdate()
        {
            if (m_FuncQueueAwaiter.IsReady)
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
                    if (m_FuncExecuteQueue.Contains(info))
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
        }

        private void OnAwaiterFuncComplete(Awaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);
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