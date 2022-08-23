using System;
using System.Collections;

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
                

                //m_Awaiter.Disposed += OnAwaiterDisposed;
                
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


}