using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class Picture
    {
        private PictureConfig m_Config;
        
        private GameObject m_GameObject;
        
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


        public virtual void Configure(params object[] args)
        {
            m_GameObject = new GameObject("Picture");
            m_Sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");


            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is PictureConfig)
                    {
                        m_Config = (PictureConfig)args[0];

                        m_Width = m_Config.Width;
                        m_Height = m_Config.Height;
                        m_BackgroundColor = m_Config.BackgroundColor;
                        m_HoverColor = m_Config.HoverColor;
                    }
                }
            }
            
            m_Matrix = new Pixel[m_Width, m_Height];
            m_Pixels = new List<Pixel>();
        }

        public virtual void Init()
        {
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var position = new Vector3(x - m_Width/2, y - m_Height/2);
                    var pixelConfig = new PixelConfig(position, m_GameObject, m_Sprite, m_BackgroundColor, m_HoverColor);
                    
                    var objPixel = new GameObject("Pixel");
                    var pixel = objPixel.AddComponent<Pixel>();
                    //var pixel = new Pixel();
                    pixel.Configure(pixelConfig);
                    pixel.Init();
                    
                    m_Pixels.Add(m_Matrix[x, y] = pixel);
                }
            }
        }

        public virtual void Dispose()
        {
            foreach (var pixel in m_Pixels)
                pixel.Dispose();


            m_Pixels.Clear();
        }
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