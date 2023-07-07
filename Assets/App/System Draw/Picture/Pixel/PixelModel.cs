using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PixelModel: ModelCacheable
    {        
        private PixelConfig m_Config;

        private int m_LayerMask;
        
        public IPixel Instance {get; private set;}
        
        public Vector3 Position { get => transform.position; private set => transform.position = value; }


        public Color ColorDefault {get; private set; } = Color.black;
        public Color ColorSelect {get; private set; } = Color.green;
        public Color ColorHover {get; private set; } = Color.grey;
                
        
        
        public static readonly string PREFAB_Folder = "Prefab";

        

        // CONFIGURE //
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (PixelConfig)args[PARAMS_Config] :
            default(PixelConfig);
        
            Instance = m_Config.Instance;
            Position = m_Config.Position;
            
            transform.name = $"Pixel ({Position.x.ToString()}; {Position.y.ToString()})";
            gameObject.layer = m_Config.LayerMask;

            if(m_Config.Parent != null )
                transform.SetParent(m_Config.Parent);
                        
            ColorDefault = m_Config.ColorDefault;
            ColorHover = m_Config.ColorHover;

            base.Configure(args);
        }

        
        public abstract void OnSelected(bool selected);
        public abstract void OnHovered(bool hovered);
        
        protected abstract void SetColor(Color color);







        // FACTORY //
        public static TPixel Get<TPixel>(params object[] args)
        where TPixel: IPixel
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PixelFactory();
            var instance = factory.Get<TPixel>(args);
            
            return instance;
        }
    }
    
    
    
    
    
    
    public interface IPixel: IConfigurable, ICacheable, IActivable, ISelectable, ISensible, IMessager
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