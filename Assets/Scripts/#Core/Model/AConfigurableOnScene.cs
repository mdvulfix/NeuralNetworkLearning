using System;
using System.Collections;
using UnityEngine;


namespace APP
{

    public abstract class AConfigurableOnScene : MonoBehaviour, IMessager
    {

        private static Transform ROOT;
        private static Transform ROOT_POOL;

        
        [SerializeField] private bool m_IsDebug = true;
        
        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;
        
        
        [SerializeField] private bool m_IsActive;


        public IConfig Config { get; protected set; }
        
        public GameObject OnSceneGameObject => gameObject;
        public Transform OnSceneTransform => gameObject.transform;

        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;
        public bool IsActive => m_IsActive;


        public event Action Initialized;
        public event Action Disposed;

        public event Action<IMessage> Message;


        // CONFIGURE //
        public virtual void Configure<TConfig>(TConfig config, params object[] args)
        where TConfig: struct, IConfig
        {
            Config = config;
            Configure(args);
        }

        public virtual void Configure(IConfig config, params object[] args)
        {
            Config = config;
            Configure(args);
        }
       
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

        // LOAD & ACTIVATE //
        public virtual void Activate()
        {
            transform.SetParent(ROOT);
            transform.position = Vector3.zero;

            m_IsActive = true;
            gameObject.SetActive(m_IsActive);

            ($"{this.GetName()} {this.GetHashCode()} active: {m_IsActive}").Send();

        }

        public IEnumerator ActivateAsync(Action<bool> callback)
        {
            Activate();
            callback.Invoke(m_IsActive);
            yield return null;
        }

        public virtual void Deactivate()
        {
            transform.SetParent(ROOT_POOL);
            transform.position = Vector3.zero;

            m_IsActive = false;
            gameObject.SetActive(m_IsActive);

            ($"{this.GetName()} {this.GetHashCode()} active: {m_IsActive}").Send();
        }

        public IEnumerator DeactivateAsync(Action<bool> callback)
        {
            Deactivate();
            callback.Invoke(m_IsActive);
            yield return null;
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


    

}

