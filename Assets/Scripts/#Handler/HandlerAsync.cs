using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP
{
    public static class HandlerAsync
    {

        private static Awaiter m_Awaiter = Awaiter.Get();
        private static Func<IEnumerator> m_Func;
        
        public static void Start(Func<IEnumerator> func)
        {
            
            m_Func = func;
            using (var awaiter = Awaiter.Get())
            {
                

                m_Awaiter.Disposed += OnAwaiterDisposed;
                
                m_Awaiter.StopCoroutine(nameof(m_Func));
                m_Awaiter.StartCoroutine(nameof(m_Func));
            
            }
            
        }

        public static void Stop(Func<IEnumerator> func)
        {
            m_Awaiter.StopCoroutine(nameof(func));
            
        }
        
        public static void StopAll()
        {
            m_Awaiter.StopAllCoroutines();
            
        }
    
        private static void OnAwaiterInitialized()
        {

        }

        private static void OnAwaiterDisposed()
        {
            //m_Awaiter.Disposed -= OnAwaiterDisposed;
            //
            //var obj = awaiter.GameObject;
            //GameObject.Destroy(obj);
        }
    
    }


    public class AsyncController : Controller
    {
        

        private List<Awaiter> m_Awaiters;

        private int m_AwaiterNumberLimit = 5;
        [SerializeField] private int m_AwaiterNumber;
        
        
        
        public override void Configure(params object[] args)
        {
            base.Configure();

            m_Awaiters = new List<Awaiter>(10);
        
        
        }
        
        public override void Init()
        {
            base.Init();

            for (int i = 0; i < m_AwaiterNumberLimit; i++)
                m_Awaiters.Add(Awaiter.Get());
                
                
            foreach (var awaiter in m_Awaiters)
            {
                awaiter.Initialized -= OnAwaiterInitialized;
                awaiter.Disposed -= OnAwaiterDisposed;
                
            }
                
        }

        public override void Dispose()
        {
            foreach (var awaiter in m_Awaiters)
            {
                awaiter.Initialized -= OnAwaiterInitialized;
                awaiter.Disposed -= OnAwaiterDisposed;
            }

            base.Dispose();
        }

        public void AsyncExecute()
        {
            var awaitersAwaliable = from Awaiter a in  m_Awaiters where a.IsReady == true select a;
            if(awaitersAwaliable.Count() > 0)
            {
                var awaiter = awaitersAwaliable.First();
                awaiter.
            
            
            }
            else
            {
                var awaiter = Awaiter.Get();
                
                awaiter.Initialized -= OnAwaiterInitialized;
                awaiter.Disposed -= OnAwaiterDisposed;
                
                m_Awaiters.Add(awaiter);

            }
            



        }


        private void OnAwaiterInitialized(Awaiter awaiter)
        {
            
            
            
            m_Awaiters.Add(awaiter);
        }

        private void OnAwaiterDisposed(Awaiter awaiter)
        {
            if(m_Awaiters.Contains(awaiter))
                m_Awaiters.Remove(awaiter);
        }
    
    
    
    }



    public class Awaiter : MonoBehaviour, IConfigurable, ILoadable, IPoolable
    {     
        
        public bool IsReady {get; private set; }
        public bool IsLoaded {get; private set; }
        public bool IsActive {get; private set; }

   
        public event Action<Awaiter> Initialized;
        public event Action<Awaiter> Disposed;
        
        
        public Func<IEnumerator>
        
        
        public virtual void Configure(params object[] args)
        {

        }

        public virtual void Init()
        {
            Initialized?.Invoke(this);
        }
        
        public virtual void Dispose()
        {
            Disposed?.Invoke(this);
        }


        public bool Load()
        {
            
            
            Activate();
            IsLoaded = true;
            return true;
        }
        
        public bool Unload()
        {
            IsLoaded = false;
            Deactivate();
            return true;
        }
        
        public bool Activate()
        {
            gameObject.SetActive(true);
            IsActive = true;
            return true;

        }
        
        public bool Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
            return true;
        }

        public void StartAwait()
        {
            StopCoroutine(nameof(AwaitFunc));
            
            var awaiting = 5;
            StartCoroutine(AwaitFunc((awaiting) => TestFunc(awaiting), awaiting));
        }

        public void StopAwait()
        {
            StopCoroutine(nameof(AwaitFunc));
        }

        public IEnumerator AwaitFunc(Func<float, bool> operationAsyncFunc, float awaiting)
        {
            if(operationAsyncFunc.Invoke(awaiting) == false)
                yield return new WaitForSeconds(1);
            
        }


        public bool TestFunc(float awaiting)
        {
            awaiting -= Time.deltaTime;
            
            do
            {
                Debug.Log("Waiting for finish operation: " + awaiting);

            } while(awaiting <= 0);

            return true;
        }

        
        // UNITY //
        private void Awake() =>
            Configure();
        
        private void OnEnable() =>
            Init();

        private void OnDisable() =>
            Dispose();

        
        
        // FACTORY //
        public static Awaiter Get()
        {
            var obj = new GameObject("Awaiter");
            obj.transform.position = Vector3.zero;
            obj.SetActive(false);
            
            return obj.AddComponent<Awaiter>();
        }
    }


}