using System;
using System.Collections;
using UnityEngine;


namespace APP.Draw
{
    [Serializable]
    public class PictureController: AController, IPictureController
    {
       
        [SerializeField] private IPicture m_Picture;

        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        
        public IPixel PixelSelected { get; private set; }
        public IPixel PixelHovered { get; private set; }
        
        
        public PictureController() { }
        public PictureController(params object[] args)
            => Configure(args);
        
        public override void Configure(params object[] args)
        {
            
            var config = (PictureControllerConfig)args[PARAM_INDEX_Config];
            m_Picture = config.Picture;
            
            base.Configure(args);
        }

        public override void Init()
        {
            m_Picture.Init();
            base.Init();
        }

        public override void Dispose()
        {
            m_Picture.Dispose();
            base.Dispose();
        }

        //public void PixelColorize(Color color) =>
        //    PixelColorize(color, Picture.PixelActive);
        
        //public void Colorize(IPixel pixel, Color color, ColorMode mode = ColorMode.None) =>
        //    pixel.SetColor(color);


        public IEnumerator AwaitLoadingAsync(IActivable activatable, float awaiting = 5f)
        {
            while (activatable.IsActivated == false && awaiting > 0)
            {
                yield return new WaitForSeconds(1);
                awaiting -= Time.deltaTime;
            }
        }

        public static PictureController Get(params object[] args)
            => Get<PictureController>(args);


    }

    public interface IPictureController : IController
    {
        IPixel PixelSelected { get; }
        IPixel PixelHovered { get; }
        
        //void Colorize(IPixel pixel, Color color, ColorMode mode = ColorMode.None);
    }

    public struct PictureControllerConfig
    {
        public PictureControllerConfig(IPicture picture)
        {
            Picture = picture;
        }

        public IPicture Picture { get; private set; }

        
    }
}
