using System;
using UnityEngine;

namespace APP
{

    public abstract class AConfigurable<TConfig> : AConfigurable
    where TConfig: struct, IConfig
    {
        public TConfig Config { get; protected set; }

        public virtual void Configure(TConfig config, params object[] args)
        {
            
            
            
            base.Configure(args);
        }
    }

    public abstract class AConfigurable : IMessager
    {
        [SerializeField] private bool m_IsDebug = true;
        
        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;

        public event Action Initialized;
        public event Action Disposed;

        public event Action<IMessage> Message;

        public virtual void Configure(params object[] args)
        {

            m_IsConfigured = true;
            Send("Configuration completed.");
        }



        public virtual void Init() 
        { 
            if (m_IsConfigured == false)
            {
                Send($"{ this.GetName() } is not configured.", LogFormat.Warning);
                Send($"Initialization was aborted!", LogFormat.Warning);
                
                return;
            }
                
            if (m_IsInitialized == true)
            {
                Send($"{this.GetName()} is already initialized.", LogFormat.Warning);
                Send($"Current initialization was aborted!", LogFormat.Warning);
                return;
            }




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


    public interface IConfigurable<TConfig>: IConfigurable
    where TConfig: struct, IConfig
    {
        TConfig? Config {get; }

        void Configure(TConfig config, params object[] args);
    }
    
    public interface IConfigurable: IDisposable
    {
        event Action Initialized;
        event Action Disposed;
        
        void Configure(params object[] args);
        void Init();
    }


    public interface IConfig
    {
        void Setup(IConfigurable configurable);
    }

}

