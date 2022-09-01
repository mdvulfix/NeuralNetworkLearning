using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public class Picture : AConfigurable, IConfigurable//, IRecognizable
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

        private Sprite m_Sprite;

        public IPixel PixelActive => m_Pixels.Where(pixel => pixel.IsActivated == true).First();

        public Picture() { }
        public Picture(params object[] args)
        {
            Configure(args);
            Init();
        }


        public override void Configure(params object[] args)
        {
            var config = (PictureConfig)args[PARAM_INDEX_Config];

            m_Width = config.Width;
            m_Height = config.Height;
            m_BackgroundColor = config.BackgroundColor;
            m_HoverColor = config.HoverColor;

            m_Parent = config.Parent;

            //OnSceneObject.name = "Picture";

            m_Sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");

            m_Matrix = new IPixel[m_Width, m_Height];
            m_Pixels = new List<IPixel>();

            base.Configure(args);
        }

        public override void Init()
        {
            var parent = new GameObject("Picture");
            parent.transform.SetParent(m_Parent);
            
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var position = new Vector3(x - m_Width/2, y - m_Height/2);
                    var pixelConfig = new PixelConfig(position, parent, m_Sprite, m_BackgroundColor, m_HoverColor);
                    
                    //var objPixel = new GameObject("Pixel");
                    //var pixel = objPixel.AddComponent<Pixel>();
                    var pixel = PixelModel.Get<PixelDefault>(pixelConfig);
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
            /*
            foreach (var pixel in m_Pixels)
                pixel.Dispose();


            m_Pixels.Clear();
            */

            base.Dispose();
        }


        //public IEnumerable<ISensible> GetSensibles() =>
        //    m_Pixels;


    }

    public struct PictureConfig
    {
        public PictureConfig(int width, int height, Color backgroundColor, Color hoverColor, Transform parent)
        {
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            Parent = parent;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
        public Transform Parent { get; internal set; }
    }
}