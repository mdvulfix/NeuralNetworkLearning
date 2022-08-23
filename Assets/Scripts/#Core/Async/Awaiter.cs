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

        public bool IsReady => m_IsReady;
        public bool IsActive => m_IsActive;
        public bool IsLoaded => m_IsLoaded;

   
        public event Action<Awaiter> Initialized;
        public event Action<Awaiter> Disposed;
        
        
        //public Func<IEnumerator>
        
        
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

        public void Run(Func<bool> funcAsync, int attamps = 3)
        {
            m_IsReady = false;
            Debug.Log($"Awaiter {this.GetHashCode()} start... Ready: {m_IsReady}");
            
            StopCoroutine(nameof(AwaitOperation));
            StartCoroutine(AwaitOperation(funcAsync, attamps));

            m_IsReady = true;
            Debug.Log($"Awaiter {this.GetHashCode()} finish... Ready: {m_IsReady}");
        }

        public void Stop()
        {
            StopCoroutine(nameof(AwaitOperation));
        }

        public IEnumerator AwaitOperation(Func<bool> operationAsync, int attamps, int awaiting = 1)
        {
            while(attamps > 0)
            {
                attamps--;
                Debug.Log("Waiting for finish operation: " + attamps);

                if (operationAsync.Invoke())
                { 
                    Debug.Log("Operation: Success!");
                    yield return null;
                }
                
                yield return new WaitForSeconds(awaiting);
    
            } 
            
            Debug.LogWarning("Operation: Failed!");
            Debug.LogWarning("Operation done by time delay!");
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