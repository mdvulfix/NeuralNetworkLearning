using System;
using System.Collections;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class PictureController
    {
        [SerializeField] private int m_Widht = 50;
        [SerializeField] private int m_Height = 50;

        private PictureControllerConfig m_Config;
        
        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        
        public Picture Picture { get; private set; }

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is PictureControllerConfig)
                    {
                        m_Config = (PictureControllerConfig)arg;

                        m_BackgroundColor = m_Config.BackgroundColor;
                        m_HoverColor = m_Config.HoverColor;
                    }
                }
            }
        }

        public virtual void Init()
        {
            
            var pictureConfig = new PictureConfig(m_Widht, m_Height, m_BackgroundColor, m_HoverColor);
            
            Picture = Picture.Get();
            HandlerAsync.Start(() => AwaitSceneObjectLoadingAsync(Picture));
            
            
            Picture.Configure(pictureConfig);
            Picture.Init();
        }

        public virtual void Dispose()
        {
            Picture.Dispose();
        }



        public void PixelColorize(Color color) =>
            PixelColorize(color, Picture.PixelActive);
        
        public void PixelColorize(Color color, Pixel pixel) =>
            pixel.SetColor(color, ColorMode.Draw);


    
        public IEnumerator AwaitSceneObjectLoadingAsync(ILoadable loadable, float awaiting = 5f)
        {
            while (loadable.IsLoaded == false && awaiting > 0)
            {
                yield return new WaitForSeconds(1);
                awaiting -= Time.deltaTime;
            }
        }
    
    
    }

    public struct PictureControllerConfig
    {
        public PictureControllerConfig(Color backgroundColor, Color hoverColor)
        {
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
        }

        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
    }
}
