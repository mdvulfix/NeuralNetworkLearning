using System;
using UnityEngine;

namespace APP
{
    public abstract class ModelLoadable : MonoBehaviour, ILoadable, IConfigurable, IActivable, IMessager
    {
        public static readonly int PARAMS_Config = 0;
        public static readonly int PARAMS_Factory = 1;

        [SerializeField] private bool m_IsDebug = true;
        [SerializeField] private bool m_IsDebugOnLoad = true;
        [SerializeField] private bool m_IsDebugOnConfigure = true;
        [SerializeField] private bool m_IsDebugOnActivate = true;

        [SerializeField] private bool m_IsLoaded;
        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;
        [SerializeField] private bool m_IsActivated;


        public bool IsLoaded => m_IsLoaded;
        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;
        public bool IsActivated => m_IsActivated;

        public event Action<IMessage> Message;

        // LOAD //
        public virtual void Load() => OnLoadComplete(isDebag: m_IsDebugOnLoad); 
            

        // CONFIGURE //
        public virtual void Configure(params object[] args) => OnConfigureComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Init() => OnInitComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Dispose() => OnDisposeComplete(isDebag: m_IsDebugOnConfigure);


        // ACTIVATE //
        public virtual void Activate() => OnActivatedComplete(isDebag: m_IsDebugOnActivate);
        public virtual void Deactivate() => OnDeactivatedComplete(isDebag: m_IsDebugOnActivate);


        // MESSAGE //
        public IMessage Send(string text, LogFormat format = LogFormat.None)
            => Send(new Message(this, text, format));

        public IMessage Send(IMessage message)
        {
            Message?.Invoke(message);
            return Messager.Send(m_IsDebug, this, message.Text, message.LogFormat);
        }


        public Transform GetTransform()
            => transform;


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

        // CALLBACK //
        protected virtual void OnLoadComplete(bool isDebag)
        {
            m_IsLoaded = true; 
            ($"Load complete.").Send(this, isDebag);
        }

        protected virtual void OnConfigureComplete(bool isDebag)
        {
            m_IsConfigured = true; 
            ("Configure complete.").Send(this, isDebag);
        }
        
        protected virtual void OnInitComplete(bool isDebag)
        {
            m_IsInitialized = true; 
            ("Initialize complete.").Send(this, isDebag);
        }
        
        protected virtual void OnDisposeComplete(bool isDebag)
        {
            m_IsInitialized = false;
            ("Dispose complete.").Send(this, isDebag); 
        }
        
        protected virtual void OnActivatedComplete(bool isDebag)
        {
            m_IsActivated = true;
            ("Activated complete.").Send(this, isDebag); 
        }
        
        protected virtual void OnDeactivatedComplete(bool isDebag)
        {
            m_IsActivated = false;
            ("Deactivated complete.").Send(this, isDebag); 
        }
        
        
        public void OnMessage(IMessage message) =>
            Send($"{message.Sender}: {message.Text}", message.LogFormat);

        
    
        // UNITY //
        private void Awake() 
            => Load();

        private void OnEnable() 
            => Init();

        private void OnDisable()
            => Dispose();
    }



    public interface ILoadable
    {       
        bool IsLoaded { get; }
        
        void Load();

        Transform GetTransform();
    }

    public interface IActivable
    {        
        bool IsActivated { get; }
        
        void Activate();
        void Deactivate();

        Transform GetTransform();
    }

}

