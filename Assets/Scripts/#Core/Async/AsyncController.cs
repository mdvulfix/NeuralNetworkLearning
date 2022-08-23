using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP
{
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
                //awaiter.
            
            
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


}