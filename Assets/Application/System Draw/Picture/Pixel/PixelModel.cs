using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PixelModel: ModelConfigurableOnAwake
    {        
        private IPixel m_Instance;
        private PixelConfig m_Config;

        private int m_LayerMask;
        

        public Color ColorDefault {get; private set; } = Color.black;
        public Color ColorSelect {get; private set; } = Color.green;
        public Color ColorHover {get; private set; } = Color.grey;
                
        
        public Transform Pixel { get; private set; }
        public Vector3 Position { get => Pixel.position; private set => Pixel.position = value; }


        public static readonly int LOAD_PARAM_Transform = 0;
        public static readonly string PREFAB_Folder = "Prefab";

        
        public event Action Initialized;
        public event Action Disposed;

        /*
        // LOAD //
        public override void Load()
        {
            if(VerificationOnLoad())
                return;
            
            Pixel = (Transform)args[LOAD_PARAM_Transform];
            
            m_IsLoaded = true;
            Send("Load completed.");
        }
        */

        // CONFIGURE //
        public override void Configure(params object[] args)
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

            base.Configure();
        }

        public override void Init()
        {
            if(VerificationOnInit())
                return;
            
            base.Init();
        }

        public override void Dispose()
        {
            


            base.Dispose();
        }

        // ACTIVATE //
        public override void Activate()
        {
            if(VerificationOnActivate())
                return;
            
            try { Pixel.gameObject.SetActive(true); base.Activate(); }
            catch (Exception exception) { Send($"Activation failed. Exeption { exception.Message }", LogFormat.Warning); }
 
            //m_Transform.SetParent(ROOT);
            //m_Transform.position = Vector3.zero;
        }

        public override void Deactivate()
        {
            try { Pixel.gameObject.SetActive(false); base.Deactivate(); }
            catch (Exception exception) { Send($"Deactivation failed. Exeption { exception.Message }", LogFormat.Warning); }

            //transform.SetParent(ROOT_POOL);
            //transform.position = Vector3.zero;
        }


        protected abstract void SetColor(Color color);


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