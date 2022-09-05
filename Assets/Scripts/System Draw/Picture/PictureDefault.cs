using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SceneObject = UnityEngine.GameObject;

using APP.Brain;

namespace APP.Draw
{

    public class PictureDefault : PictureModel, IPicture
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Square";
        
        public PictureDefault() { }
        public PictureDefault(params object[] args)
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

            
            var width = 5;
            var height = 5;
            var backgroundColor = Color.black;
            var hoverColor = Color.grey;

            var layerMask = 8;

            
            Transform parent = null;
            if(Seacher.Find<IScene>(out var scene))
                parent = scene[0].SceneObject.transform;
   
            var pixelConfig = new PictureConfig(width, height, backgroundColor, hoverColor, layerMask, parent);
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
    public abstract class PictureModel : AConfigurableOnAwake
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Square";

        private Transform m_Parent;

        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;

        private IPixel[,] m_Matrix;
        private List<IPixel> m_Pixels;

        [SerializeField] private Color m_BackgroundColor = Color.black;
        [SerializeField] private Color m_HoverColor = Color.grey;

        private int m_LayerMask;

        private Sprite m_Sprite;

        public IPixel PixelActive => m_Pixels.Where(pixel => pixel.IsActivated == true).First();

        public override void Configure(params object[] args)
        {
            var config = (PictureConfig)args[PARAM_INDEX_Config];

            m_Width = config.Width;
            m_Height = config.Height;
            m_BackgroundColor = config.BackgroundColor;
            m_HoverColor = config.HoverColor;
            
            m_LayerMask = config.LayerMask;

            m_Parent = config.Parent;

            base.Configure(args);
        }

        public override void Init()
        {
            SceneObject.layer = m_LayerMask;
            
            m_Sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");

            m_Matrix = new IPixel[m_Width, m_Height];
            m_Pixels = new List<IPixel>();
            
            
            
            if(m_Parent != null )
                SceneObject.transform.SetParent(m_Parent);

            var pixelParent = SceneObject.transform;
            
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var position = new Vector3(x - m_Width / 2, y - m_Height / 2);
                    var pixelConfig = new PixelConfig(position, m_Sprite, m_BackgroundColor, m_HoverColor, m_LayerMask, pixelParent);
                    var pixel = PixelModel.Get<PixelDefault>(pixelConfig);
                    
                    pixel.Configure(pixelConfig);
                    pixel.Init();

                    m_Pixels.Add(m_Matrix[x, y] = pixel);
                }
            }

            base.Init();
        }

        public override void Dispose()
        {

            foreach (var pixel in m_Pixels)
                pixel.Dispose();


            m_Pixels.Clear();


            base.Dispose();
        }


        public override void Activate()
        {
            base.Activate();

            foreach (var pixel in m_Pixels)
                pixel.Activate();      
        }

        public override void Deactivate()
        {
            foreach (var pixel in m_Pixels)
                pixel.Deactivate();  

            base.Deactivate();    
        }
        
        
        public IEnumerable<ISensible> GetSensibles() =>
            m_Pixels;

        
        public static TPicture Get<TPicture>(params object[] args)
        where TPicture: Component, IPicture
        {
            var obj = new GameObject("Picure");
            obj.SetActive(false);

            //var renderer = obj.AddComponent<MeshRenderer>();           
            var instance = obj.AddComponent<TPicture>();
            
            if(args.Length > 0)
            {
                instance.Configure(args);
                instance.Init();
            }
            
            return instance;
        }
    }

    public struct PictureConfig
    {
        public PictureConfig(int width, int height, Color backgroundColor, Color hoverColor, int layerMask, Transform parent)
        {
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            LayerMask = layerMask;
            Parent = parent;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
        public int LayerMask { get; }
        public Transform Parent { get; internal set; }
    }

    public interface IPicture: IConfigurable, IRecognizable
    {
        SceneObject SceneObject {get; }

        IPixel PixelActive { get; }
    }

    public class PictureFactory : Factory<IPicture>
    {
        private string m_Label = "Picture";
        
        public PictureFactory()
        {
            Set<PictureDefault>(Constructor.Get((args) => PictureModel.Get<PictureDefault>(args)));
        }
    }
}