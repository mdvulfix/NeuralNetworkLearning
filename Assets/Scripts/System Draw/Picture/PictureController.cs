using System;
using System.Collections;
using UnityEngine;


namespace APP.Draw
{
    [Serializable]
    public class PictureController: AConfigurable, IConfigurable
    {
       
        private Transform m_SceneRoot;
        
        [SerializeField] private int m_Widht = 10;
        [SerializeField] private int m_Height = 10;

        [SerializeField] private PictureDefault m_Picture;

        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        
        private int m_LayerMask = 8;
        

        public PictureController() { }
        public PictureController(params object[] args)
        {
            Configure(args);
            Init();
        }
        
        public override void Configure(params object[] args)
        {
            
            var config = (PictureControllerConfig)args[PARAM_INDEX_Config];

            m_BackgroundColor = config.BackgroundColor;
            m_HoverColor = config.HoverColor;

            m_SceneRoot = config.Root;

            
            base.Configure(args);
        }

        public override void Init()
        {
            
            var pictureConfig = new PictureConfig(m_Widht, m_Height, m_BackgroundColor, m_HoverColor, m_LayerMask, m_SceneRoot);
            
            //TODO: Picture controller init
            //Picture = Picture.Get();
            //HandlerAsync.Execute(() => AwaitSceneObjectLoadingAsync(Picture));
            
            m_Picture = PictureModel.Get<PictureDefault>();
            m_Picture.Configure(pictureConfig);
            m_Picture.Init();
            m_Picture.Activate();

            base.Init();
        }

        public override void Dispose()
        {
            
            m_Picture.Deactivate();
            m_Picture.Dispose();

            base.Dispose();
        }



        //public void PixelColorize(Color color) =>
        //    PixelColorize(color, Picture.PixelActive);
        
        public void PixelColorize(IPixel pixel, Color color) =>
            pixel.SetColor(color, ColorMode.Draw);


    
        public IEnumerator AwaitLoadingAsync(IActivable activatable, float awaiting = 5f)
        {
            while (activatable.IsActivated == false && awaiting > 0)
            {
                yield return new WaitForSeconds(1);
                awaiting -= Time.deltaTime;
            }
        }
    }



    public struct PictureControllerConfig
    {
        public PictureControllerConfig(Color backgroundColor, Color hoverColor, Transform root)
        {
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            Root = root;
        }

        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
        public Transform Root { get; internal set; }
    }
}
