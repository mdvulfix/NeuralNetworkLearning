using System;
using UnityEngine;

namespace APP
{
    public abstract class ModelConfigurable : IConfigurable, IMessager
    {
        public static readonly int CONFIG_PARAM_Config = 0;
        public static readonly int CONFIG_PARAM_Factory = 1;

        [SerializeField] private bool m_IsDebug = true;

        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;


        
        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;

        public event Action<IMessage> Message;


        // CONFIGURE //
        public abstract void Configure(params object[] args);
        public abstract void Init();
        public abstract void Dispose();

        
        // VERIFY //        
        protected virtual bool VerificationOnConfigure()
        {
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




}

