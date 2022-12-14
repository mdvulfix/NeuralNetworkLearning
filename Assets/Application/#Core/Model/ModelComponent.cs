using System;
using UnityEngine;

namespace APP
{
    public abstract class ModelComponent : MonoBehaviour, IConfigurable, IActivable, IMessager
    {
        public static readonly int PARAMS_Config = 0;
        public static readonly int PARAMS_Factory = 1;

        [SerializeField] private bool m_IsDebug = true;
        [SerializeField] private bool m_IsDebugOnRecord = true;
        [SerializeField] private bool m_IsDebugOnConfigure = true;
        [SerializeField] private bool m_IsDebugOnActivate = true;
        
        
        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;
        [SerializeField] private bool m_IsActivated;

        private static ICache m_Cache;
                
        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;
        public bool IsActivated => m_IsActivated;
               
        public event Action<IMessage> Message;
            
        
        // CONFIGURE //
        public virtual void Configure(params object[] args) => OnConfigureComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Init() => OnInitComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Dispose() => OnDisposeComplete(isDebag: m_IsDebugOnConfigure);


        // ACTIVATE //
        public virtual void Activate()
        {
            if(VerifyOnActivate())
                return;
            
            var obj = gameObject;
            
            try {obj.SetActive(true); OnActivatedComplete(isDebag: m_IsDebugOnActivate); }
            catch (Exception exception) { Send($"Activation failed. Exeption { exception.Message }", LogFormat.Warning); }
 
        }

        public virtual void Deactivate()
        {
            var obj = gameObject;
            
            try {obj.SetActive(false); OnActivatedComplete(isDebag: m_IsDebugOnActivate); }
            catch (Exception exception) { Send($"Activation failed. Exeption { exception.Message }", LogFormat.Warning); }

        }
        
                
        // VERIFY //        
        protected virtual bool VerifyOnConfigure()
        {
            if (m_IsConfigured == true)
            {
                Send($"Instance is already configured.", LogFormat.Warning);
                return true;
            }
            return false;
        }

        protected virtual bool VerifyOnInit()
        {
            if (m_IsConfigured == false)
            {
                Send($"Instance is not configured.", LogFormat.Warning);
                Send($"Initialization was aborted!", LogFormat.Warning);

                return true;
            }

            if (m_IsInitialized == true)
            {
                Send($"Instance is already initialized.", LogFormat.Warning);
                return true;
            }

            return false;
        }
        
        protected virtual bool VerifyOnActivate()
        {
            if (m_IsInitialized== false)
            {
                Send($"Instance is not initialized.", LogFormat.Warning);
                Send($"Activation was aborted!", LogFormat.Warning);

                return true;
            }

            if (m_IsActivated == true)
            {
                Send($"Instance is already activated.", LogFormat.Warning);
                return true;
            }

            return false;
        }


        public Transform GetTransform()
            => transform;


        // MESSAGE //
        public IMessage Send(string text, bool isDebag, LogFormat format = LogFormat.None)
            => Send(new Message(this, text, format), isDebag);

        public IMessage Send(string text, LogFormat format = LogFormat.None)
            => Send(new Message(this, text, format));
             
        public IMessage Send(IMessage message, bool isDebag)
        {
            Message?.Invoke(message);
            return Messager.Send(isDebag, this, message.Text, message.LogFormat);
        }

        public IMessage Send(IMessage message)
        {
            Message?.Invoke(message);
            return Messager.Send(m_IsDebug, this, message.Text, message.LogFormat);
        }



        // CALLBACK //
        public void OnMessage(IMessage message) =>
            Send($"{message.Sender}: {message.Text}", message.LogFormat);


        protected virtual void OnConfigureComplete(bool isDebag)
        {
            m_IsConfigured = true; 
            Send("Configure complete.", isDebag);
        }
        
        protected virtual void OnInitComplete(bool isDebag)
        {
            m_IsInitialized = true; 
            Send("Initialize complete.", isDebag);
        }
        
        protected virtual void OnDisposeComplete(bool isDebag)
        {
            m_IsInitialized = false;
            Send("Dispose complete.", isDebag); 
        }
        
        protected virtual void OnActivatedComplete(bool isDebag)
        {
            m_IsActivated = true;
            Send("Activated complete.", isDebag); 
        }
        
        protected virtual void OnDeactivatedComplete(bool isDebag)
        {
            m_IsActivated = false;
            Send("Deactivated complete.", isDebag); 
        }

    }
}

