using System;
using UnityEngine;

namespace APP
{
    public abstract class ModelConfigurableOnAwake : MonoBehaviour, IConfigurable, ILoadable, IActivable, IMessager
    {
        public static readonly int CONFIG_PARAM_Config = 0;
        public static readonly int CONFIG_PARAM_Factory = 1;

        [SerializeField] private bool m_IsDebug = true;

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
        public virtual void Load() { m_IsLoaded = true; Send("Load complete."); } 
            
        // CONFIGURE //
        public virtual void Configure(params object[] args) { m_IsConfigured = true; Send("Configure complete."); }
        public virtual void Init() { m_IsInitialized = true; Send("Initialize complete."); }
        public virtual void Dispose() { m_IsInitialized = false; Send("Dispose complete."); }


        // ACTIVATE //
        public virtual void Activate()  { m_IsActivated = true; Send("Activate complete."); }
        public virtual void Deactivate() { m_IsActivated = true; Send("Deactivate complete."); }

        
        // VERIFY //
        protected virtual bool VerificationOnLoad()
        {
            if (m_IsLoaded == true)
            {
                Send($"Instance is already loaded.", LogFormat.Warning);
                return true;
            }

            return false;
        }
        
        protected virtual bool VerificationOnConfigure()
        {
            if (m_IsLoaded == false)
            {
                Send($"{this.GetName()} is not loaded.", LogFormat.Warning);
                Send($"Configuration was aborted!", LogFormat.Warning);

                return true;
            }

            if (m_IsConfigured == true)
            {
                Send($"Instance is already configured.", LogFormat.Warning);
                return true;
            }
            return false;
        }

        protected virtual bool VerificationOnInit()
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
        
        protected virtual bool VerificationOnActivate()
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


        // MESSAGE //
        public IMessage Send(string text, LogFormat format = LogFormat.None)
            => Send(new Message(this, text, format));

        public IMessage Send(IMessage message)
        {
            Message?.Invoke(message);
            return Messager.Send(m_IsDebug, this, message.Text, message.LogFormat);
        }

        // CALLBACK //
        public void OnMessage(IMessage message) =>
            Send($"{message.Sender}: {message.Text}", message.LogFormat);

        
        
        // UNITY //
        private void Awake() 
            => Configure();

        private void OnEnable() 
            => Init();

        private void OnDisable()
            => Dispose();

    }

    public interface IConfig
    {

    }

    public interface IConfigurable : IDisposable
    {
        bool IsConfigured { get; }
        bool IsInitialized { get; }
        
        void Configure(params object[] args);
        void Init();
    }

    public interface ILoadable
    {
        bool IsLoaded { get; }
        
        void Load();
    }

    public interface IActivable
    {
        bool IsActivated { get; }
        
        void Activate();
        void Deactivate();
    }

}

