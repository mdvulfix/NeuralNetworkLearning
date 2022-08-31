using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Brain;

namespace APP
{
    [Serializable]
    public class Picture : AConfigurable, IConfigurable, IRecognizable
    {
        private readonly string FOLDER_SPRITES = "Sprites";
        private string m_SpriteLabel = "box_white";

        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;

        private Pixel[,] m_Matrix;
        private List<Pixel> m_Pixels;

        [SerializeField] private Color m_BackgroundColor = Color.black;
        [SerializeField] private Color m_HoverColor = Color.grey;

        private Sprite m_Sprite;

        public Pixel PixelActive => m_Pixels.Where(pixel => pixel.IsActive == true).First();

        public Picture() { }
        public Picture(params object[] args)
            => Configure(args);

        
        public override void Configure(params object[] args)
        {
            var config = (PictureConfig)args[PARAM_INDEX_Config];

            m_Width = config.Width;
            m_Height = config.Height;
            m_BackgroundColor = config.BackgroundColor;
            m_HoverColor = config.HoverColor;

            //OnSceneObject.name = "Picture";

            m_Sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");

            m_Matrix = new Pixel[m_Width, m_Height];
            m_Pixels = new List<Pixel>();

            base.Configure(args);
        }

        public override void Init()
        {

            var parent = new GameObject("Picture");
            
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var position = new Vector3(x - m_Width/2, y - m_Height/2);
                    var pixelConfig = new PixelConfig(position, parent, m_Sprite, m_BackgroundColor, m_HoverColor);
                    
                    var objPixel = new GameObject("Pixel");
                    var pixel = objPixel.AddComponent<Pixel>();
                    //var pixel = new Pixel();
                    pixel.Configure(pixelConfig);
                    pixel.Init();
                    
                    m_Pixels.Add(m_Matrix[x, y] = pixel);
                }
            }

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


        public IEnumerable<ISensible> GetSensibles() =>
            m_Pixels;


    }

    public struct PictureConfig
    {
        public PictureConfig(int width, int height, Color backgroundColor, Color hoverColor)
        {
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
    }
}