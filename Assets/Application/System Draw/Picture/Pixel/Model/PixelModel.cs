using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PixelModel : AConfigurableOnAwake
    {        
        private int m_LayerMask;
        
        public Color ColorDefault {get; private set; } = Color.black;
        public Color ColorHover {get; private set; } = Color.grey;
        
        public Transform Pixel => gameObject.transform;
        public Vector3 Position { get => Pixel.position; private set => Pixel.position = value; }

        
        public static readonly string PREFAB_Folder = "Prefab";
        public static readonly int PARAM_INDEX_Factory = 1;

        public override void Configure(params object[] args)
        {
            var config = (PixelConfig)args[PARAM_INDEX_Config];
            
            Pixel.name = $"Pixel ({config.Position.x.ToString()}; {config.Position.y.ToString()})";

            if(config.Parent != null )
                Pixel.SetParent(config.Parent);
            
            Position = config.Position;
            
            Pixel.gameObject.layer = config.LayerMask;
            ColorDefault = config.ColorDefault;
            ColorHover = config.ColorHover;


            base.Configure(args);
        }


        public override void Init()
        {

            base.Init();
        }

        public void SetColor() =>
            SetColor(ColorDefault);

        public virtual void SetColor(Color color, ColorMode mode = ColorMode.None)
        {
            if (mode == ColorMode.Draw)
                ColorDefault = color;
        }


        public void Update()
        {
            //Sensor.Excite();
        }


        /*
        private void OnMouseOver()
        {
            SetColor(ColorHover);
        }

        private void OnMouseExit()
        {
            SetColor(ColorDefault);
        }
        */

        // FACTORY //
        public static TPixel Get<TPixel>(params object[] args)
        where TPixel: Component, IPixel
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAM_INDEX_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PixelFactory();
            var instance = factory.Get<TPixel>(args);
            
            return instance;
        }

        /*
        // LOAD & ACTIVATE //
        public virtual void Load()
        {
            m_IsLoaded = true;
            Send(($"{this.GetName()} {this.GetHashCode()} load status: {m_IsLoaded}"));
        }

        public virtual void Unload()
        {
            m_IsLoaded = false;
            Send(($"{this.GetName()} {this.GetHashCode()} load status: {m_IsLoaded}"));
        }
        
        public virtual void Activate()
        {
            //m_Transform.SetParent(ROOT);
            //m_Transform.position = Vector3.zero;

            m_IsActivated = true;
            m_GameObject.SetActive(m_IsActivated);

            Send(($"{this.GetName()} {this.GetHashCode()} active status: {m_IsActivated}"));

        }

        public virtual void Deactivate()
        {
            //transform.SetParent(ROOT_POOL);
            //transform.position = Vector3.zero;

            m_IsActivated = false;
            m_GameObject.SetActive(m_IsActivated);

            Send(($"{this.GetName()} {this.GetHashCode()} active status: {m_IsActivated}"));;
        }
        */
    }
    
    public interface IPixel: IConfigurable, IUpdateble, IActivable, ISelectable, ISensible
    {
        void SetColor(Color color, ColorMode mode = ColorMode.None);
    }



    public struct PixelConfig
    {
        public Vector3 Position { get; private set; }
        public Color ColorDefault { get; private set; }
        public Color ColorHover { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }


        public PixelConfig(Vector3 position, Color colorDefault, Color colorHover, int layerMask, Transform parent = null)
        {
            ColorDefault = colorDefault;
            ColorHover = colorHover;
            LayerMask = layerMask;
            Position = position;
            Parent = parent;

        }
    }

    public enum ColorMode
    {
        None,
        Draw
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