using System;
using System.Collections;
using UnityEngine;

namespace APP
{
    
    public class Awaiter : MonoBehaviour, IConfigurable, ILoadable, IPoolable
    {     
        
        [SerializeField] private bool m_IsReady;
        [SerializeField] private bool m_IsActive;
        [SerializeField] private bool m_IsLoaded;
       
        private Func<IEnumerator> Func;

        public bool IsReady => m_IsReady;
        public bool IsActive => m_IsActive;
        public bool IsLoaded => m_IsLoaded;

    
        public event Action<Awaiter> Initialized;
        public event Action<Awaiter> Disposed;

        public event Action<Awaiter> FuncStarted;
        public event Action<Awaiter> FuncCompleted;
        
        public event Action<Awaiter, bool> StateChanged;


        
        
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
            m_IsLoaded = true;
            return true;
        }
        
        public bool Load(out Awaiter awaiter)
        {
            
            Activate();
            m_IsLoaded = true;
            awaiter = this;
            return true;
        }


        
        public bool Unload()
        {
            m_IsLoaded = false;
            Deactivate();
            return true;
        }
        
        public bool Activate()
        {
            gameObject.SetActive(true);
            m_IsActive = true;
            m_IsReady = true;
            return true;

        }
        
        public bool Deactivate()
        {
            m_IsReady = false;
            m_IsActive = false;
            gameObject.SetActive(false);
            return true;
        }


        public void Run(Func<Action, IEnumerator> func)
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


        public void SetState(bool isReady)
        {
            m_IsReady =  isReady;
            Debug.Log($"Awaiter {this.GetHashCode()} start... Ready: {m_IsReady}");
            StateChanged?.Invoke(this, isReady);
        }

        public void Callback()
        {
            Stop();
        }


        // UNITY //
        private void Awake() =>
            Configure();
        
        private void OnEnable() =>
            Init();

        private void OnDisable() =>
            Dispose();
        


        // FACTORY //
        public static Awaiter Get(Transform parent = null)
        {
            var obj = new GameObject("Awaiter");
            obj.SetActive(false);
            
            if(parent != null)
                obj.transform.SetParent(parent);

            obj.transform.position = Vector3.zero;

            return obj.AddComponent<Awaiter>();
        }
    }


    public delegate IEnumerator FuncAsyncDelegate();
}

