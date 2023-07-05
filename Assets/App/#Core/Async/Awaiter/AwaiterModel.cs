using System;
using System.Collections;
using UnityEngine;

namespace APP
{
    public class AwaiterModel : ModelCacheable
    {
        private static Transform ROOT;
        private static Transform ROOT_POOL;

        private IAwaiter m_Instance;


        [SerializeField] private bool m_IsDebug = true;
        [SerializeField] private bool m_IsReady;
        [SerializeField] private string m_Label;

        private Func<IEnumerator> Func;


        public bool IsReady => m_IsReady;

        public event Action<IAwaiter> Initialized;
        public event Action<IAwaiter> Disposed;
        public event Action<IAwaiter> FuncStarted;
        public event Action<IAwaiter> FuncCompleted;
        public event Action<IAwaiter, bool> StateChanged;

        public static string PREFAB_Folder;
        public static string PREFAB_Label;

        public override void Configure(params object[] args)
        {
            var config = (AwaiterConfig)args[PARAMS_Config];

            m_Instance = config.Instance;
            transform.name = config.Label;

            if (ROOT == null)
                ROOT = SceneRoot.AWAITER;

            if (ROOT_POOL == null)
                ROOT_POOL = SceneRoot.AWAITER_POOL;

            base.Configure();
        }

        public override void Init()
        {


            base.Init();
        }



        public void Run(Func<Action<bool>, IEnumerator> func)
        {
            var isReady = false;
            SetState(isReady);

            Func = () => func(Callback);

            StopCoroutine(Func());

            try
            {
                FuncStarted?.Invoke(m_Instance);

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
            FuncCompleted?.Invoke(m_Instance);
        }


        private void Callback(bool isReady)
        {
            SetState(isReady);
        }

        private void SetState(bool isReady)
        {
            m_IsReady = isReady;
            Debug.Log($"Awaiter {m_Instance.GetHashCode()} state: {isReady}");
            StateChanged?.Invoke(m_Instance, isReady);
        }



        protected override void OnActivatedComplete(bool isDebag)
        {

            m_IsReady = true;
            base.OnActivatedComplete(m_IsDebug);
        }

        protected override void OnDeactivatedComplete(bool isDebag)
        {

            m_IsReady = false;
            base.OnDeactivatedComplete(m_IsDebug);
        }

        protected override void OnInitComplete(bool isDebag)
        {

            Initialized?.Invoke(m_Instance);

            base.OnInitComplete(m_IsDebug);
        }

        protected override void OnDisposeComplete(bool isDebag)
        {

            Disposed?.Invoke(m_Instance);

            base.OnDisposeComplete(m_IsDebug);
        }



        // FACTORY //
        public static TAwaiter Get<TAwaiter>(params object[] args)
        where TAwaiter : IAwaiter
        {
            IFactory factoryCustom = null;

            if (args.Length > 0)
                try { factoryCustom = (IFactory)args[PARAMS_Factory]; }
                catch { Debug.Log("Custom factory not found! The instance will be created by default."); }


            var factory = (factoryCustom != null) ? factoryCustom : new AwaiterFactory();
            var instance = factory.Get<TAwaiter>(args);

            return instance;
        }

    }

    public struct AwaiterConfig : IConfig
    {
        public AwaiterConfig(IAwaiter instance, string label)
        {
            Instance = instance;
            Label = label;
        }

        public IAwaiter Instance { get; private set; }
        public string Label { get; private set; }

    }

    public interface IAwaiter : IPoolable, IActivable
    {
        bool IsReady { get; }

        event Action<IAwaiter> Initialized;
        event Action<IAwaiter> Disposed;
        event Action<IAwaiter> FuncStarted;
        event Action<IAwaiter> FuncCompleted;
        event Action<IAwaiter, bool> StateChanged;

        void Run(Func<Action<bool>, IEnumerator> func);
        void Stop();
    }





    public partial class AwaiterFactory : Factory<IAwaiter>, IFactory
    {
        public AwaiterFactory()
        {
            Set<AwaiterDefault>(Constructor.Get((args) => GetAwaiterDefault(args)));

        }

    }


}

