using System;
using UnityEngine;

namespace APP
{
    public abstract class ModelConfigurable : IConfigurable, IMessager
    {
        [SerializeField] private bool m_IsDebug = true;
        [SerializeField] private bool m_IsDebugOnRecord = true;
        [SerializeField] private bool m_IsDebugOnConfigure = true;
        [SerializeField] private bool m_IsDebugOnActivate = true;

        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;


        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;


        public event Action<IMessage> Message;
        
        public static readonly int PARAMS_Config = 0;
        public static readonly int PARAMS_Factory = 1;

        // CONFIGURE //
        public virtual void Configure(params object[] args) => OnConfigureComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Init() => OnInitComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Dispose() => OnDisposeComplete(isDebag: m_IsDebugOnConfigure);

        
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
        

        // MESSAGE //
        public IMessage Send(string text, LogFormat format = LogFormat.None)
            => Send(new Message(this, text, format));

        public IMessage Send(IMessage message)
        {
            Message?.Invoke(message);
            return Messager.Send(m_IsDebug, this, message.Text, message.LogFormat);
        }

        

        protected virtual void OnConfigureComplete(bool isDebag)
        {
            m_IsConfigured = true; 
            ("Configure complete.").Send(isDebag);
        }
        
        protected virtual void OnInitComplete(bool isDebag)
        {
            m_IsInitialized = true; 
            ("Initialize complete.").Send(isDebag);
        }
        
        protected virtual void OnDisposeComplete(bool isDebag)
        {
            m_IsInitialized = false;
            ("Dispose complete.").Send(isDebag); 
        }

        
        public void OnMessage(IMessage message) =>
            Send($"{message.Sender}: {message.Text}", message.LogFormat);
    
    
    
    
    
    }

    public interface IConfig
    {

    }

    public interface IConfigurable : IDisposable
    {
        bool IsConfigured { get; }
        bool IsInitialized { get; }

        //event Action<IConfigurable> Configured;
        //event Action<IConfigurable> Initialized;
        
        void Configure(params object[] args);
        void Init();
    }



}

