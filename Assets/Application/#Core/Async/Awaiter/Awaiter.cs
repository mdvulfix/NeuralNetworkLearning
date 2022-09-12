using System;
using System.Collections;
using UnityEngine;

namespace APP
{
    public class AwaiterModel: ModelCacheable
    {
        private static Transform ROOT;
        private static Transform ROOT_POOL;

        private IAwaiter m_Instance;

        [SerializeField] private bool m_IsReady;
        [SerializeField] private string m_Label;

        private Func<IEnumerator> Func;


        public bool IsReady => m_IsReady;

        public event Action<IAwaiter> FuncStarted;
        public event Action<IAwaiter> FuncCompleted;
        public event Action<IAwaiter, bool> StateChanged;



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

        // FACTORY //
        public static TAwaiter Get<TAwaiter>(params object[] args)
        where TAwaiter: Component, IAwaiter
        {
            var obj = new GameObject("Awaiter");
            obj.SetActive(false);
            obj.transform.SetParent(ROOT_POOL);

            var awaiter = obj.AddComponent<TAwaiter>();
            awaiter.Configure(args);
            awaiter.Init();

            return awaiter;
        }        
    }



    public class AwaiterDefault : AwaiterModel, IAwaiter
    {
        public AwaiterDefault() { }
        public AwaiterDefault(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if(IsConfigured == true)
                return;
                    
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            var label = $"Awaiter {this.GetHashCode()}";

            var pixelConfig = new AwaiterConfig(this, label);
            base.Configure(pixelConfig);
            Send($"{ this.GetName() } was configured by default!");
        }



        public override void Init()
        {
            if(IsInitialized == true)
                return;
            
            
            base.Init();
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

    public interface IAwaiter: IPoolable, IActivable
    {
        bool IsReady { get; }

        event Action<IAwaiter> FuncStarted;
        event Action<IAwaiter> FuncCompleted;
        event Action<IAwaiter, bool> StateChanged;

        void Run(Func<Action<bool>, IEnumerator> func);
        void Stop();
    }

    
    /*
    public class FactoryAwaiter : IFactory<IAwaiter>
    {
        //public FactoryPool() { }
   
        public IAwaiter Get(params object[] args)
        {
            var instance =  new Awaiter();
            instance.Configure(args);
            instance.Init();

            return instance;
        }
    
    }
    */

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

