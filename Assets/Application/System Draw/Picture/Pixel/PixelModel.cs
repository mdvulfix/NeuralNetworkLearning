using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PixelModel
    {        
        private IPixel m_Instance;
        private PixelConfig m_Config;

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
        
        public Color ColorDefault {get; private set; } = Color.black;
        public Color ColorSelect {get; private set; } = Color.green;
        public Color ColorHover {get; private set; } = Color.grey;
                
        
        public Transform Pixel { get; private set; }
        public Vector3 Position { get => Pixel.position; private set => Pixel.position = value; }


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
            
            Pixel = (Transform)args[LOAD_PARAM_Transform];
            
            m_IsLoaded = true;
            Send("Load completed.");
        }
        
        public virtual void Unload()
        {
            Deactivate();
            
            m_IsLoaded = false;
            Send("Unload completed.");
        }
        
        // CONFIGURE //
        public virtual void Configure(params object[] args)
        {
            if(VerificationOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (PixelConfig)args[CONFIG_PARAM_Config] :
            default(PixelConfig);
            
            m_Instance = m_Config.Instance;

            Position = m_Config.Position;
            Pixel.name = $"Pixel ({Position.x.ToString()}; {Position.y.ToString()})";
            Pixel.gameObject.layer = m_Config.LayerMask;

            if(m_Config.Parent != null )
                Pixel.SetParent(m_Config.Parent);
                        
            
            ColorDefault = m_Config.ColorDefault;
            ColorHover = m_Config.ColorHover;

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
                Pixel.gameObject.SetActive(m_IsActivated); 
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
                Pixel.gameObject.SetActive(m_IsActivated); 
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


        protected abstract void SetColor(Color color);


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
        public static TPixel Get<TPixel>(params object[] args)
        where TPixel: IPixel
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[CONFIG_PARAM_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PixelFactory();
            var instance = factory.Get<TPixel>(args);
            
            return instance;
        }
    }
    
    public interface IPixel: IConfigurable, ILoadable, IActivable, ISelectable, ISensible, IMessager
    {
        
    }



    public struct PixelConfig
    {
        public IPixel Instance { get; private set; }
        public Vector3 Position { get; private set; }
        public Color ColorDefault { get; private set; }
        public Color ColorHover { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }


        public PixelConfig(IPixel instance, Vector3 position, Color colorDefault, Color colorHover, int layerMask, Transform parent = null)
        {
            ColorDefault = colorDefault;
            ColorHover = colorHover;
            LayerMask = layerMask;
            Instance = instance;
            Position = position;
            Parent = parent;

        }
    }

    public enum ColorMode
    {
        None,
        Hover,
        Select
    }

    public partial class PixelFactory : Factory<IPixel>
    {
        public PixelFactory()
        {
            Set<Pixel2D>(Constructor.Get((args) => GetPixel2D(args)));
            Set<Pixel3D>(Constructor.Get((args) => GetPixel3D(args)));
        }
    }
}

namespace APP
{
    public interface ILoadable
    {
        void Load(params object[] args);
    }
}