using System;
using UnityEngine;
using UComponent = UnityEngine.Component;

namespace APP
{
    public abstract class ModelLoadable : MonoBehaviour, IConfigurable, ILoadable, IActivable, IComponent, IMessager
    {
        public static readonly int PARAMS_Config = 0;
        public static readonly int PARAMS_Factory = 1;

        [SerializeField] private bool m_IsDebug = true;
        [SerializeField] private bool m_IsDebugOnLoad = true;
        [SerializeField] private bool m_IsDebugOnRecord = true;
        [SerializeField] private bool m_IsDebugOnConfigure = true;
        [SerializeField] private bool m_IsDebugOnActivate = true;

        [SerializeField] private bool m_IsLoaded;


        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;
        [SerializeField] private bool m_IsActivated;
        [SerializeField] private bool m_IsAnimated;


        public bool IsLoaded => m_IsLoaded;
        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;
        public bool IsActivated => m_IsActivated;
        public bool IsAnimated => m_IsAnimated;

        public GameObject Obj => gameObject;

        public event Action<IMessage> Message;

        // LOAD //
        public virtual void Load() => OnLoadComplete(isDebag: m_IsDebugOnLoad);

        // CACHE //
        public virtual void Record() => OnRecordComplete(isDebag: m_IsDebugOnRecord);
        public virtual void Clear() => OnClearComplete(isDebag: m_IsDebugOnRecord);

        // CONFIGURE //
        public virtual void Configure(params object[] args) => OnConfigureComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Init() => OnInitComplete(isDebag: m_IsDebugOnConfigure);
        public virtual void Dispose() => OnDisposeComplete(isDebag: m_IsDebugOnConfigure);


        // ACTIVATE //
        public virtual void Activate()
        {
            if (VerifyOnActivate())
                return;

            var obj = gameObject;

            try { obj.SetActive(true); OnActivatedComplete(isDebag: m_IsDebugOnActivate); }
            catch (Exception exception) { Send($"Activation failed. Exeption {exception.Message}", LogFormat.Warning); }

        }

        public virtual void Deactivate()
        {
            var obj = gameObject;

            try { obj.SetActive(false); OnActivatedComplete(isDebag: m_IsDebugOnActivate); }
            catch (Exception exception) { Send($"Activation failed. Exeption {exception.Message}", LogFormat.Warning); }

        }


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


        // VERIFY //  
        protected virtual bool VerifyOnLoad()
        {
            if (m_IsLoaded == true)
            {
                Send($"Instance is already loaded.", LogFormat.Warning);
                return true;
            }
            return false;
        }

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
            if (m_IsInitialized == false)
            {
                Send($"Instance is not initialized.", LogFormat.Warning);
                Send($"Activation was aborted!", LogFormat.Warning);

                return true;
            }

            return false;
        }

        // CALLBACK //
        protected virtual void OnLoadComplete(bool isDebag)
        {
            m_IsLoaded = true;
            Send($"Load complete.", isDebag);
        }

        protected virtual void OnRecordComplete(bool isDebag)
        {
            m_IsActivated = true;
            Send("The instance is written to the cache.", isDebag);
        }

        protected virtual void OnClearComplete(bool isDebag)
        {
            m_IsActivated = false;
            Send("The instance was cleared from the cache.", isDebag);
        }

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


        public void OnMessage(IMessage message)
            => Send($"{message.Sender}: {message.Text}", message.LogFormat);


        // COMPONENT //
        public TComponent SetComponent<TComponent>()
        where TComponent : UComponent
            => gameObject.AddComponent<TComponent>();

        public bool GetComponent<TComponent>(out TComponent component)
        where TComponent : UComponent
            => gameObject.TryGetComponent<TComponent>(out component);

        public void SetParent(Transform parent)
            => gameObject.transform.SetParent(parent);



        // UNITY //
        private void Awake()
        {
            Load();
            Configure();
        }

        private void OnEnable()
        {
            Record();
            Init();
            Activate();
        }

        private void Start()
        {

        }

        private void OnDisable()
        {
            Deactivate();
            Clear();
            Dispose();
        }


    }



    public interface ILoadable
    {
        void Load();
    }

    public interface IActivable
    {
        void Activate();
        void Deactivate();
    }

    public interface IComponent
    {
        GameObject Obj { get; }

        TComponent SetComponent<TComponent>()
        where TComponent : UComponent;

        bool GetComponent<TComponent>(out TComponent component)
        where TComponent : UComponent;

        void SetParent(Transform parent);

    }

}

