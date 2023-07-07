using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Pool;

namespace APP
{
    public class AsyncController : ModelController, IAsyncController
    {


        [SerializeField] private static Transform m_AsyncHolder;

        [SerializeField] private IFactory m_AwaiterFactory;
        private Func<IAwaiter> GetAwaiterFunc;

        private static List<IAwaiter> m_AwaiterIsReady;
        private int m_AwaiterIsReadyLimit = 2;

        private static List<IAsyncInfo> m_FuncExecuteQueue;
        private IAwaiter m_FuncQueueAwaiter;



        private IPoolController m_PoolController;




        public event Action<IAsyncInfo> FuncAsyncExecuted;

        public AsyncController() { }
        public AsyncController(params object[] args)
            => Configure(args);



        public override void Configure(params object[] args)
        {
            var config = (AsyncControllerConfig)args[PARAMS_Config];

            if (m_AsyncHolder == null)
                m_AsyncHolder = config.AsyncHolder;

            base.Configure(args);
        }

        public override void Init()
        {

            if (m_AsyncHolder == null)
                m_AsyncHolder = new GameObject("Async").transform;

            if (m_AwaiterIsReady == null)
                m_AwaiterIsReady = new List<IAwaiter>(m_AwaiterIsReadyLimit);

            if (m_FuncExecuteQueue == null)
                m_FuncExecuteQueue = new List<IAsyncInfo>(100);


            // SET AWAITER //
            if (m_AwaiterFactory == null)
                m_AwaiterFactory = new AwaiterFactory();

            GetAwaiterFunc = () => m_AwaiterFactory.Get<AwaiterDefault>();

            m_FuncQueueAwaiter = GetAwaiterFunc();
            var awaiterConfig = new AwaiterConfig(m_FuncQueueAwaiter, "FuncQueueAwaiter", m_AsyncHolder);

            m_FuncQueueAwaiter.Configure(awaiterConfig);
            m_FuncQueueAwaiter.Init();

            // SET POOL //
            var poolControllerConfig = new PoolControllerConfig(m_AsyncHolder);
            m_PoolController = PoolController.Get();
            m_PoolController.Configure(poolControllerConfig);
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
        }


        public void ExecuteAsync(Func<Action<bool>, IEnumerator> func)
        {
            if (GetAwaiter(out var awaiter))
            {
                if (awaiter != null && awaiter.IsReady == true)
                {
                    awaiter.FuncRun(func);
                    FuncAsyncExecuted?.Invoke(new FuncAsyncInfo(awaiter, func));
                    return;
                }

                m_FuncExecuteQueue.Add(new FuncAsyncInfo(awaiter, func));
            }
        }


        private bool GetAwaiter(out IAwaiter awaiter)
        {
            awaiter = null;

            if ((m_AwaiterIsReady.Count < m_AwaiterIsReadyLimit))
                AwaiterLimitUpdate();


            awaiter = m_AwaiterIsReady.First();
            return true;
        }



        private void AwaiterLimitUpdate()
        {
            // Check awaiters in ready state limit;
            var awaiterIsReadyNumber = m_AwaiterIsReady.Count;

            // If the limit is less than the current number of awaiters, push unnecessary awaiters in the pool
            if (awaiterIsReadyNumber > m_AwaiterIsReadyLimit)
            {
                var number = awaiterIsReadyNumber - m_AwaiterIsReadyLimit;
                for (int i = 0; i < number; i++)
                {
                    PushAwaiter(m_AwaiterIsReady.First());
                }

            }
            // else, pop awaiters from the pool in the number of missing up to the limit
            else
            {

                var number = m_AwaiterIsReadyLimit - awaiterIsReadyNumber;
                for (int i = 0; i < number; i++)
                    PopAwaiter(out var awaiter);
            }
        }

        private void PushAwaiter(IAwaiter awaiter)
        {
            awaiter.Deactivate();
            awaiter.Dispose();

            awaiter.Initialized -= OnAwaiterInitialized;
            awaiter.Disposed -= OnAwaiterDisposed;
            awaiter.FuncStarted -= OnAwaiterBusy;
            awaiter.FuncCompleted -= OnAwaiterFuncComplete;

            m_PoolController.Push(awaiter);

        }

        private bool PopAwaiter(out IAwaiter awaiter)
        {
            if (!m_PoolController.Pop(out awaiter))
            {
                awaiter = GetAwaiterFunc();
                awaiter.Configure(new AwaiterConfig(awaiter, "Awaiter " + awaiter.GetHashCode(), m_AsyncHolder));
            }

            awaiter.Initialized += OnAwaiterInitialized;
            awaiter.Disposed += OnAwaiterDisposed;
            awaiter.FuncStarted += OnAwaiterBusy;
            awaiter.FuncCompleted += OnAwaiterFuncComplete;

            awaiter.Init();
            awaiter.Activate();

            return true;

        }



        private void FuncQueueUpdate()
        {
            if (m_FuncQueueAwaiter.IsReady)
                m_FuncQueueAwaiter.FuncRun(FuncQueueExecuteAsync);
        }

        private IEnumerator FuncQueueExecuteAsync(Action<bool> callback)
        {
            var funcsReadyToBeExecuted = (from IAsyncInfo funcInfo in m_FuncExecuteQueue
                                          where funcInfo.Awaiter.IsReady == true
                                          select funcInfo).ToArray();


            if (funcsReadyToBeExecuted.Length > 0)
            {
                foreach (var info in funcsReadyToBeExecuted)
                {
                    if (m_FuncExecuteQueue.Contains(info))
                        m_FuncExecuteQueue.Remove(info);

                    info.Awaiter.FuncRun(info.FuncAsync);
                    FuncAsyncExecuted?.Invoke(info);
                }
            }

            yield return null;

            callback.Invoke(true);
        }


        private void OnAwaiterInitialized(IAwaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);
        }

        private void OnAwaiterDisposed(IAwaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);
        }

        private void OnAwaiterBusy(IAwaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

            AwaiterLimitUpdate();
        }

        private void OnAwaiterFuncComplete(IAwaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);
            AwaiterLimitUpdate();
        }


        public static AsyncController Get(params object[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    var config = (AsyncControllerConfig)args[PARAMS_Config];
                    var instance = new AsyncController();
                    instance.Configure(config);
                    return instance;
                }

                catch { Debug.Log("Custom factory not found! The instance will be created by default."); }
            }

            return new AsyncController();
        }

    }

    public interface IAsyncController : IController, IConfigurable, IUpdateble
    {
        void ExecuteAsync(Func<Action<bool>, IEnumerator> func);
    }



    public struct AsyncControllerConfig : IConfig
    {
        public AsyncControllerConfig(Transform asyncHolder)
        {
            AsyncHolder = asyncHolder;
        }

        public Transform AsyncHolder { get; private set; }
    }

    public struct FuncAsyncInfo : IAsyncInfo
    {
        public IAwaiter Awaiter { get; private set; }
        public Func<Action<bool>, IEnumerator> FuncAsync { get; private set; }

        public FuncAsyncInfo(IAwaiter awaiter, Func<Action<bool>, IEnumerator> func)
        {
            FuncAsync = func;
            Awaiter = awaiter;
        }
    }

    public interface IAsyncInfo
    {
        IAwaiter Awaiter { get; }
        Func<Action<bool>, IEnumerator> FuncAsync { get; }

    }

}