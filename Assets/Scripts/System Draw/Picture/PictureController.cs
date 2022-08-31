using System;
using System.Collections;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class PictureController: AConfigurable, IConfigurable
    {
       
        [SerializeField] private int m_Widht = 50;
        [SerializeField] private int m_Height = 50;

        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        
        public Picture Picture { get; private set; }

        public PictureController() { }
        public PictureController(params object[] args)
            => Configure(args);
        
        public override void Configure(params object[] args)
        {
            
            var config = (PictureControllerConfig)args[PARAM_INDEX_Config];

            m_BackgroundColor = config.BackgroundColor;
            m_HoverColor = config.HoverColor;

            
            base.Configure(args);
        }

        public override void Init()
        {
            
            var pictureConfig = new PictureConfig(m_Widht, m_Height, m_BackgroundColor, m_HoverColor);
            
            //TODO: Picture controller init
            //Picture = Picture.Get();
            //HandlerAsync.Execute(() => AwaitSceneObjectLoadingAsync(Picture));
            
            Picture = new Picture(pictureConfig);

            base.Init();
        }

        public override void Dispose()
        {
            Picture.Dispose();

            base.Dispose();
        }



        public void PixelColorize(Color color) =>
            PixelColorize(color, Picture.PixelActive);
        
        public void PixelColorize(Color color, Pixel pixel) =>
            pixel.SetColor(color, ColorMode.Draw);


    
        public IEnumerator AwaitLoadingAsync(IActivatable activatable, float awaiting = 5f)
        {
            while (activatable.IsActive == false && awaiting > 0)
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
