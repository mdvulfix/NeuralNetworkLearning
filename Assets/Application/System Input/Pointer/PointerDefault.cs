using System;
using UnityEngine;

namespace APP.Input
{

    [Serializable]
    public class PointerDefault : PointerModel, IPointer
    {
        private SpriteRenderer m_Renderer;
    
        public static readonly string PREFAB_Label = "Pointer";

        public PointerDefault()
            => Load();
        
        public PointerDefault(params object[] args)
        {
            Load();
            Configure(args);
        }  


        public override void Load(params object[] args)
        {
            if(VerificationOnLoad())
                return;

            var prefabPath = $"{PointerModel.PREFAB_Folder}/{PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Pointer");
            
            obj.SetActive(false);
            
            if (obj.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<SpriteRenderer>();
            
            base.Load(obj.transform);
        }

        public override void Configure(params object[] args)
        {
            if(VerificationOnConfigure())
                return;
        
            Pointer.name = "Pointer";
            Pointer.localPosition = Vector3.back;
            Pointer.localScale = Vector3.one * 3;

            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }
            
            var position = Vector3.zero;
            var defaultColor = m_Renderer.color;
            var layerMask = 5;
            
            Transform parent = null;
            if(Seacher.Find<IScene>(out var scenes))
            {
                parent = scenes[0].Scene != null ?
                scenes[0].Scene :
                Pointer.parent;
            }
                
   
            var config = new PointerConfig(this, defaultColor, layerMask, parent);
            base.Configure(config);
            
            Send($"{ this.GetName() } was configured by default!");
        }
    

        public override void SetColor(Color color)
        {
            m_Renderer.color = color;
        }


    }

    public class PointerView : ILoadable
    {
        

        
        public void Load(params object[] args)
        {

        }
    }

    
    public abstract class PointerModel
    {
        private static IFactory m_Factory = new FactoryDefault();

        private IPointer m_Instance;
        private PointerConfig m_Config;

        [SerializeField] private bool m_IsLoaded;
        [SerializeField] private bool m_IsConfigured;
        [SerializeField] private bool m_IsInitialized;
        [SerializeField] private bool m_IsActivated;
        [SerializeField] private bool m_IsDebug = true;

        private int m_LayerMask;
        
        public bool IsLoaded => m_IsLoaded;
        public bool IsConfigured => m_IsConfigured;
        public bool IsInitialized => m_IsInitialized;
        public bool IsActivated => m_IsActivated;
        
        
        public Color ColorDefault { get; private set; }
        public Transform Pointer { get; private set; }
        public Vector3 Position { get => Pointer.position; private set => Pointer.position = value; }
        
        
        public static readonly int LOAD_PARAM_Transform = 0;
        
        public static readonly int CONFIG_PARAM_Config = 0;
        public static readonly int CONFIG_PARAM_Factory = 1;
        
        public static readonly string PREFAB_Folder = "Prefab";

        
        public event Action Initialized;
        public event Action Disposed;

        public event Action<IMessage> Message;


        // LOAD //
        public virtual void Load(params object[] args)
        {
            if(VerificationOnLoad())
                return;
            
            Pointer = args.Length > 0 ?
            (Transform)args[LOAD_PARAM_Transform] :
            new GameObject("Pointer").transform;

            m_IsLoaded = true;
            Send("Load completed.");
        }
        
        public virtual void Unload()
        {
            Deactivate();
            
            m_IsLoaded = false;
            Send("Unload completed.");
        }
        
                
        public virtual void Configure(params object[] args)
        {
            if(VerificationOnConfigure())
                return;
                       
            m_Config = args.Length > 0 ?
            (PointerConfig)args[CONFIG_PARAM_Config] :
            default(PointerConfig);
            
            m_Instance = m_Config.Instance;
            ColorDefault = m_Config.ColorDefault;

            if(m_Config.Parent != null )
                Pointer.SetParent(m_Config.Parent);
                        
            m_IsConfigured = true;
            Send("Configuration completed.");

        }

        public virtual void Init()
        {
            if(VerificationOnInit())
                return;
            
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


        // ACTIVATE //
        public virtual void Activate()
        {
            if(VerificationOnActivate())
                return;
            
            try 
            { 
                m_IsActivated = true;
                Pointer.gameObject.SetActive(m_IsActivated); 
                Send("Activation completed.");
            }
            catch (Exception exception) 
            { 
                m_IsActivated = false;
                Send($"Activation failed. Exeption { exception.Message }", LogFormat.Warning);
            }
 
            //m_Transform.SetParent(ROOT);
            //m_Transform.position = Vector3.zero;
        }

        public virtual void Deactivate()
        {
            try 
            { 
                m_IsActivated = false;
                Pointer.gameObject.SetActive(m_IsActivated); 
                Send("Deactivation completed.");
            }
            catch (Exception exception) 
            { 
                m_IsActivated = true;
                Send($"Deactivation failed. Exeption { exception.Message }", LogFormat.Warning);
            }
            
            //transform.SetParent(ROOT_POOL);
            //transform.position = Vector3.zero;
        }


        public abstract void SetColor(Color color);
        
        public void SetPosition(Vector3 position)
            => Position = position;


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

        public void OnMessage(IMessage message) =>
            Send($"{message.Sender}: {message.Text}", message.LogFormat);
        
        // FACTORY //
        public static TPointer Get<TPointer>(params object[] args)
        where TPointer : IConfigurable
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[CONFIG_PARAM_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new FactoryDefault();
            var instance = factory.Get<TPointer>(args);
            
            return instance;
        }
    }

    public interface IPointer: IConfigurable, ILoadable, IActivable, IMessager
    {
        Transform Pointer { get; }
        Vector3 Position { get; }

        void SetPosition(Vector3 position);
        void SetColor(Color color);
    }


    public class PointerConfig
    {
        public PointerConfig(IPointer instance, Color colorDefault, int layerMask, Transform parent = null)
        {
            Instance = instance;
            ColorDefault = colorDefault;
            LayerMask = layerMask;
            Parent = parent;
        }

        public IPointer Instance { get; private set; }
        public Color ColorDefault { get; }
        public int LayerMask { get; }
        public Transform Parent { get; private set; }
    }






}