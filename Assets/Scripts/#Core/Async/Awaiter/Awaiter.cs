using System;
using System.Collections;
using UnityEngine;

namespace APP
{
    public interface IAwaiter
    {
        bool IsReady { get; }

        event Action<Awaiter> FuncStarted;
        event Action<Awaiter> FuncCompleted;
        event Action<Awaiter, bool> StateChanged;

        void Run(Func<Action<bool>, IEnumerator> func);
        void Stop();
    }

    public class Awaiter : AConfigurable, IAwaiter, IPoolable 
    {

        private static Transform ROOT;
        private static Transform ROOT_POOL;


        [SerializeField] private bool m_IsReady;


        private Func<IEnumerator> Func;

        public bool IsReady => m_IsReady;

        public event Action<Awaiter> FuncStarted;
        public event Action<Awaiter> FuncCompleted;

        public event Action<Awaiter, bool> StateChanged;

        public Awaiter() { }
        public Awaiter(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {
            if (ROOT == null)
                ROOT = SceneRoot.AWAITER;

            if (ROOT_POOL == null)
                ROOT_POOL = SceneRoot.AWAITER_POOL;

            base.Configure();
        }


        public void Run(Func<Action<bool>, IEnumerator> func)
        {
            var isReady = false;
            SetState(isReady);

            Func = () => func(Callback);

            StopCoroutine(Func());

            try
            {
                FuncStarted?.Invoke(this);

                StartCoroutine(Func());
                Debug.Log("Async operation started...");

            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
                Stop();
            }
        }

        public void Stop()
        {
            StopCoroutine(Func());
            Func = null;

            var isReady = true;
            SetState(isReady);

            Debug.Log("Async operation finished...");
            FuncCompleted?.Invoke(this);
        }


        private void Callback(bool isReady)
        {
            SetState(isReady);
        }

        private void SetState(bool isReady)
        {
            m_IsReady = isReady;
            Debug.Log($"Awaiter {this.GetHashCode()} state: {isReady}");
            StateChanged?.Invoke(this, isReady);
        }


        
        
        // FACTORY //
        public static TAwaiter Get<TAwaiter>(IFactory<TAwaiter> factory, params object[] arg)
        where TAwaiter: class, IAwaiter, new()
            => factory.Get(arg, "Awaiter", ROOT_POOL, ROOT);
    }

    public struct AwaiterConfig : IConfig
    {
        public void Setup(IConfigurable configurable) { }

    }


}

namespace APP
{
    public partial class SceneRoot
    {
        public static Transform AWAITER;
        public static Transform AWAITER_POOL;

        public SceneRoot()
        {
            if (AWAITER == null)
                AWAITER = new GameObject("Awaiters").transform;

            if (AWAITER_POOL == null)
            {
                AWAITER_POOL = new GameObject("Pool").transform;
                AWAITER_POOL.transform.SetParent(AWAITER);
            }
        }
    }
}

