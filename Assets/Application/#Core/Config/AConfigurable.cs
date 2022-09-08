using System;
using UnityEngine;

namespace APP
{
    public abstract class AConfigurable : IMessager
    {
        public static readonly int PARAM_INDEX_Config = 0;

        private IConfig m_Config;

        [SerializeField] private bool m_IsDebug = true;

        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;

        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;


        public event Action Initialized;
        public event Action Disposed;

        public event Action<IMessage> Message;


        // CONFIGURE //
        public virtual void Configure(params object[] args)
        {
            if (args.Length > 0)
                foreach (var arg in args)
                    if (arg is IConfig)
                        m_Config = (IConfig)arg;

            
            
            m_IsConfigured = true;
            Send("Configuration completed.");
        }

        public virtual void Init()
        {
            m_IsInitialized = true;
            Initialized?.Invoke();

            Send("Initialization completed!");
        }

        public virtual void Dispose()
        {
            m_IsInitialized = false;
            Disposed?.Invoke();
            Send("Dispose completed!");
        }


        // VERIFY //
        protected virtual bool ConfigureVerification()
        {
            if (m_IsConfigured == true)
            {
                Send($"Instance is already configured.", LogFormat.Warning);
                Send($"Current configuration was aborted!", LogFormat.Warning);
                return true;
            }

            return false;
        }

        protected virtual bool InitVerification()
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
                Send($"Current initialization was aborted!", LogFormat.Warning);
                return true;
            }

            return false;
        }


        public IConfig GetConfig()
            => m_Config;

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
    }

    public interface IConfigurable : IDisposable
    {
        event Action Initialized;
        event Action Disposed;

        void Configure(params object[] args);
        void Init();
    }


    public interface IConfig
    {

    }

}

