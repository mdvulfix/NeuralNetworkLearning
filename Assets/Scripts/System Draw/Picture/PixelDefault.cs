using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{

    public class PixelDefault : PixelModel, IPixel
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Square";
        
        public PixelDefault() { }
        public PixelDefault(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {
            if(IsConfigured == true)
                return;
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            var position = Vector3.zero;
            var backgroundColor = Color.black;
            var hoverColor = Color.grey;
            var sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");
            var layerMask = 8;
            
            Transform parent = null;
            if(Seacher.Find<IPicture>(out var picture))
            {
                parent = picture[0].SceneObject.transform != null ?
                picture[0].SceneObject.transform:
                SceneObject.transform.parent;
            }
                
   
            var pixelConfig = new PixelConfig(position, sprite, backgroundColor, hoverColor, layerMask, parent);
            base.Configure(pixelConfig);
            Send($"{ this.GetName() } was configured by default!");
        }


        public override void Init()
        {
            if(IsInitialized == true)
                return;
            
            
            base.Init();
        }
    }



    [Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class PixelModel : AConfigurableOnAwake
    {
        private Transform m_Transform;
        private Transform m_Parent;
        private Vector3 m_Position;
        
        private BoxCollider2D m_Collider;
        
        private SpriteRenderer m_Renderer;
        private Sprite m_Sprite;
        private Color m_ColorDefault = Color.black;
        private Color m_ColorHover = Color.grey;

        private int m_LayerMask;
        

        public Sensor Sensor { get; private set; }


        public override void Configure(params object[] args)
        {
            var config = (PixelConfig)args[PARAM_INDEX_Config];

            m_Sprite = config.Sprite;
            m_ColorDefault = config.ColorDefault;
            m_ColorHover = config.ColorHover;

            m_Position = config.Position;
            m_Parent = config.Parent;

            m_LayerMask = config.LayerMask;
    
            base.Configure(args);
        }


        public override void Init()
        {
            if (SceneObject.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = SceneObject.AddComponent<SpriteRenderer>();

            m_Renderer.sprite = m_Sprite;
            m_Renderer.color = m_ColorDefault;

            if (SceneObject.TryGetComponent<BoxCollider2D>(out m_Collider) == false)
                m_Collider = SceneObject.AddComponent<BoxCollider2D>();
            
            m_Collider.size = Vector2.one;
            //m_Collider.offset = 0;

            
            SceneObject.layer = m_LayerMask;

            m_Transform = SceneObject.transform;
            m_Transform.position = m_Position;
            m_Transform.parent = m_Parent;
            m_Transform.name = $"Pixel ({m_Position.x.ToString()}; {m_Position.y.ToString()})";

            
            if(m_Parent != null )
                SceneObject.transform.SetParent(m_Parent);
            
            base.Init();
        }

        public void SetColor() =>
            SetColor(m_ColorDefault);

        public void SetColor(Color color, ColorMode mode = ColorMode.None)
        {
            if (mode == ColorMode.Draw)
                m_ColorDefault = color;

            if (m_Renderer.color == color)
                return;

            m_Renderer.color = color;
        }

        public void SetSensor(Sensor sensor)
        {
            Sensor = sensor;
        }



        public void Update()
        {
            //Sensor.Excite();
        }



        private void OnMouseOver()
        {
            SetColor(m_ColorHover);
        }

        private void OnMouseExit()
        {
            SetColor(m_ColorDefault);
        }


        // FACTORY //
        public static TPixel Get<TPixel>(params object[] args)
        where TPixel: Component, IPixel
        {
            var obj = new GameObject("Pixel");
            obj.SetActive(false);

            //var renderer = obj.AddComponent<MeshRenderer>();           
            var instance = obj.AddComponent<TPixel>();
            
            if(args.Length > 0)
            {
                instance.Configure(args);
                instance.Init();
            }
            
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
        public Sprite Sprite { get; private set; }
        public Color ColorDefault { get; private set; }
        public Color ColorHover { get; private set; }
        public int LayerMask { get; }
        public Transform Parent { get; private set; }

        public PixelConfig(Vector3 position, Sprite sprite, Color colorDefault, Color colorHover, int layerMask,  Transform parent = null)
        {
            Sprite = sprite;
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

    public class PixelFactory : Factory<IPixel>
    {
        public PixelFactory()
        {
            Set<PixelDefault>(Constructor.Get((args) => PixelModel.Get<PixelDefault>(args)));
        }
    }

}