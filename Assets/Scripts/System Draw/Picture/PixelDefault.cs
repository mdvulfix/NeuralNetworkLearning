using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{

    public class PixelDefault : PixelModel, IPixel
    {
        public PixelDefault() { }
        public PixelDefault(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            Send($"{this.GetName()} is not configured because of config not found!", LogFormat.Warning);
            Send($"Configuration was aborted!", LogFormat.Warning);
        }


    }



    [Serializable]
    public abstract class PixelModel : AConfigurable, IConfigurable, ISensible, IUpdateble, ILoadable
    {
        
        private static IFactory m_Factory = new FactoryDefault();
        
        private GameObject m_GameObject;
        private Transform m_Transform;
        private BoxCollider2D m_Collider;
        private SpriteRenderer m_Renderer;

        [SerializeField] private bool m_IsLoaded;
        [SerializeField] private bool m_IsActivated;

        private Color m_ColorDefault = Color.black;
        private Color m_ColorHover = Color.grey;


        public Sensor Sensor { get; private set; }


        public bool IsActivated => m_IsActivated;
        public bool IsLoaded => m_IsLoaded;


        public override void Configure(params object[] args)
        {
            var config = (PixelConfig)args[PARAM_INDEX_Config];

            //m_GameObject = OnSceneObject;
            m_GameObject = new GameObject("Pixel");
            m_Transform = m_GameObject.transform;

            if (m_GameObject.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = m_GameObject.AddComponent<SpriteRenderer>();

            if (m_GameObject.TryGetComponent<BoxCollider2D>(out m_Collider) == false)
            {
                m_Collider = m_GameObject.AddComponent<BoxCollider2D>();
                m_Collider.size = Vector2.one;
                //m_Collider.offset = 0;
            }


            m_Renderer.sprite = config.Sprite;

            m_ColorDefault = config.ColorDefault;
            m_Renderer.color = m_ColorDefault;

            m_ColorHover = config.ColorHover;

            m_Transform.position = config.Position;
            m_Transform.parent = config.Parent.transform;
            m_Transform.name = $"Pixel ({m_Transform.position.x.ToString()}; {m_Transform.position.y.ToString()})";

            base.Configure(args);

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
            if (IsActivated == false)
                Activate();



            SetColor(m_ColorHover);
        }

        private void OnMouseExit()
        {
            if (IsActivated == false)
                Activate();

            SetColor(m_ColorDefault);
        }



        public static TPixel Get<TPixel>(params object[] args)
        where TPixel : IConfigurable
            => Get<TPixel>(null, args);
        
        
        public static TPixel Get<TPixel>(IFactory factory, params object[] args)
        where TPixel : IConfigurable
        {
            var pixelFactory = (factory == null) ? m_Factory : factory;
            var pixel = pixelFactory.Get<TPixel>();
            
            
            //var obj = new GameObject("Pixel");
            //obj.SetActive(false);
            //obj.transform.SetParent(ROOT_POOL);

            //var pixel = obj.AddComponent<TPixel>();
            return pixel;
        }

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

        






        /*
        private void Awake()
        {
            
            var folder = "Sprites";
            var label = "box_white";
            
            var sprite = Resources.Load<Sprite>($"{folder}/{label}");
            var pixelConfig = new PixelConfig(Vector3.zero, gameObject, sprite, Color.black, Color.gray);

            Configure(pixelConfig);
        }
        */


    }



    public interface IPixel: IConfigurable, ILoadable, IUpdateble
    {
        void SetColor(Color color, ColorMode mode = ColorMode.None);
        void SetSensor(Sensor sensor);

    }



    public struct PixelConfig
    {
        public Vector3 Position { get; private set; }
        public GameObject Parent { get; private set; }
        public Sprite Sprite { get; private set; }
        public Color ColorDefault { get; private set; }
        public Color ColorHover { get; private set; }

        public PixelConfig(Vector3 position, GameObject parent, Sprite sprite, Color colorDefault, Color colorHover)
        {
            Parent = parent;
            Sprite = sprite;
            ColorDefault = colorDefault;
            ColorHover = colorHover;
            Position = position;
        }
    }

    public enum ColorMode
    {
        None,
        Draw
    }
}