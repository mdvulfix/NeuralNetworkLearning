using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PictureModel<TPixel> : AConfigurable
    where TPixel: Component, IPixel
    {

        

        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;

        private IPixel[,] m_Matrix;
        private List<IPixel> m_Pixels;

        [SerializeField] private Color m_BackgroundColor = Color.black;
        [SerializeField] private Color m_HoverColor = Color.grey;

        private int m_LayerMask;

        private Sprite m_Sprite;
        private static int PARAM_INDEX_Factory;

        public Transform Parent {get; private set; }
        public Transform Picture {get; private set; }
        public IPixel PixelActive => m_Pixels.Where(pixel => pixel.IsActivated == true).First();

        public override void Configure(params object[] args)
        {
            var config = (PictureConfig)args[PARAM_INDEX_Config];

            m_Width = config.Width;
            m_Height = config.Height;
            m_BackgroundColor = config.BackgroundColor;
            m_HoverColor = config.HoverColor;
            
            m_LayerMask = config.LayerMask;

            Parent = config.Parent;

            base.Configure(args);
        }

        public override void Init()
        {

            Picture = new GameObject("Picture").transform;
             
            if(Parent != null)
                Picture.SetParent(Parent);
            
            m_Matrix = new IPixel[m_Width, m_Height];
            m_Pixels = new List<IPixel>();
                        
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var position = new Vector3(x - m_Width / 2, y - m_Height / 2);
                    
                    var pixelConfig = new PixelConfig(position, m_BackgroundColor, m_HoverColor, m_LayerMask, Picture);
                    var pixel = PixelModel.Get<TPixel>();
                    
                    pixel.Configure(pixelConfig);
                    pixel.Init();

                    m_Pixels.Add(m_Matrix[x, y] = pixel);
                    
                }
            }

            
            foreach (var pixel in m_Pixels)
                pixel.Activate();      
            
            base.Init();
        }

        
        public override void Dispose()
        {

            foreach (var pixel in m_Pixels)
            {
                pixel.Deactivate();
                pixel.Dispose();
            }
                


            m_Pixels.Clear();


            base.Dispose();
        }

        
        public IEnumerable<ISensible> GetSensibles() =>
            m_Pixels;

        
        public static TPicture Get<TPicture>(params object[] args)
        where TPicture: IPicture
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAM_INDEX_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PictureFactory();
            var instance = factory.Get<TPicture>(args);
            
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
        Transform Picture {get; }

        IPixel PixelActive { get; }
    }

    public partial class PictureFactory : Factory<IPicture>
    {
        private string m_Label = "Picture";
        
        public PictureFactory()
        {
            Set<Picture2D>(Constructor.Get((args) => GetPicture2D(args)));
            Set<Picture3D>(Constructor.Get((args) => GetPicture3D(args)));
        }
    }
}