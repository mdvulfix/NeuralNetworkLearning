using System;
using System.Collections;
using UnityEngine;

namespace APP
{


    public class Awaiter : AConfigurableOnScene, IAwaiter 
    {

        private static Transform ROOT;
        private static Transform ROOT_POOL;


        [SerializeField] private bool m_IsReady;
        [SerializeField] private string m_Label;


        private Func<IEnumerator> Func;

        public bool IsReady => m_IsReady;

        public event Action<Awaiter> FuncStarted;
        public event Action<Awaiter> FuncCompleted;

        public event Action<Awaiter, bool> StateChanged;

        public Awaiter() { }
        public Awaiter(AwaiterConfig config, params object[] args)
        {
            Configure(config, args);
            Init();
        }

        public override void Configure(IConfig config, params object[] args)
        {
            var awaiterConfig = (AwaiterConfig)config;
            m_Label = awaiterConfig.Label;
            
            if (ROOT == null)
                ROOT = SceneRoot.AWAITER;

            if (ROOT_POOL == null)
                ROOT_POOL = SceneRoot.AWAITER_POOL;

            base.Configure();
        }


        public override void Init()
        {
            OnSceneGameObject.name = m_Label;
            
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
        public static IAwaiter Get(IFactory<IAwaiter, IConfig> factory, IConfig config, params object[] args)
            => factory.Get(config, args);
    
        // FACTORY //
        public static IAwaiter Get(IConfig config, params object[] args)
        {
            var obj =  new GameObject("Awaiter");
            obj.SetActive(false);
            obj.transform.SetParent(ROOT_POOL);
            
            var awaiter = obj.AddComponent<Awaiter>();
            awaiter.Configure(config, args);
            awaiter.Init();

            return awaiter;
        }
    
    
    
    }

    public struct AwaiterConfig : IConfig
    {
        public AwaiterConfig(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }



    }

    public interface IAwaiter: IPoolable
    {
        bool IsReady { get; }

        event Action<Awaiter> FuncStarted;
        event Action<Awaiter> FuncCompleted;
        event Action<Awaiter, bool> StateChanged;

        void Run(Func<Action<bool>, IEnumerator> func);
        void Stop();
    }

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

