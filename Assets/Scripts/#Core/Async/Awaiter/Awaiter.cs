using System;
using System.Collections;
using UnityEngine;

namespace APP
{

    public class Awaiter : MonoBehaviour, IConfigurable, IPoolable
    {
        private AwaiterConfig m_Config;
        
        private static Transform ROOT_AWAITERS;
        private static Transform ROOT_AWAITERS_POOL;
        
        [SerializeField] private bool m_IsReady;
        [SerializeField] private bool m_IsActive;


        private Func<IEnumerator> Func;
        

        public bool IsReady => m_IsReady;
        public bool IsActive => m_IsActive;

        public Transform PoolParent => ROOT_AWAITERS_POOL;

        public IConfig Config => m_Config;

        public event Action<Awaiter> Initialized;
        public event Action<Awaiter> Disposed;

        public event Action<Awaiter> FuncStarted;
        public event Action<Awaiter> FuncCompleted;

        public event Action<Awaiter, bool> StateChanged;


        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is AwaiterConfig)
                    {
                        m_Config = (AwaiterConfig)arg;
                        

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
            
            Func = null;
        }

        public virtual void Init()
        {
            Activate((stateChangeCallback) =>
                { SetState(stateChangeCallback); });

            Initialized?.Invoke(this);
        }

        public virtual void Dispose()
        {
            Deactivate((stateChangeCallback) =>
                { SetState(stateChangeCallback); });

            Disposed?.Invoke(this);
        }


        public void Activate(Action<bool> stateChangeCallback)
        {
            var isReady = true;
            var isActive = true;

            SetActive(isActive);
            stateChangeCallback.Invoke(isReady);
        }
        
        public IEnumerator ActivateAsync(Action<bool> stateChangeCallback)
        {
            var isReady = true;
            var isActive = true;

            SetActive(isActive);
            stateChangeCallback.Invoke(isReady);
            yield return null;
        }


        public void Deactivate(Action<bool> stateChangeCallback)
        {
            var isReady = false;
            var isActive = false;

            SetActive(isActive);
            stateChangeCallback.Invoke(isReady);
        }
        
        public IEnumerator DeactivateAsync(Action<bool> stateChangeCallback)
        {
            var isReady = false;
            var isActive = false;

            SetActive(isActive);
            stateChangeCallback.Invoke(isReady);
            yield return null;
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


        public void Callback(bool isReady)
        {
            SetState(isReady);
        }

        private void SetState(bool isReady)
        {
            m_IsReady = isReady;
            Debug.Log($"Awaiter {this.GetHashCode()} state: {isReady}");
            StateChanged?.Invoke(this, isReady);
        }


        private void SetActive(bool isActive)
        {
            var root = isActive == true ? ROOT_AWAITERS : ROOT_AWAITERS_POOL;
            
            transform.SetParent(root);
            transform.position = Vector3.zero;
            
            gameObject.SetActive(isActive);
            m_IsActive = isActive;

            Debug.Log($"Awaiter {this.GetHashCode()} activation: {isActive}");
        }



        // FACTORY //
        public static Awaiter Get(string name = "Awaiter")
        {
            var obj = new GameObject(name);
            
            obj.SetActive(false);
            obj.transform.SetParent(ROOT_AWAITERS_POOL);
            obj.transform.position = Vector3.zero;
            
            var awaiter = obj.AddComponent<Awaiter>();
            obj.name += awaiter.GetHashCode();

            return awaiter;
        }

    }

    public struct AwaiterConfig: IConfig
    {


    }


}



