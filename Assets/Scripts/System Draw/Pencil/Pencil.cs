using System;
using UnityEngine;


namespace APP.Draw
{
    [Serializable]
    public class Pencil: AConfigurable, IConfigurable
    {        
        private PictureController m_PictureController;

        [SerializeField] private Color m_ColorDraw = Color.green;
        [SerializeField] private Color m_ColorClear = Color.black;
        

        public Pencil() { }
        public Pencil(params object[] args)
        {
            Configure(args);
            Init();
        }
         
        public override void Configure(params object[] args)
        {
            var config = (PencilConfig)args[PARAM_INDEX_Config];
           
            m_PictureController = config.PictureController;
            m_ColorDraw = config.ColorDraw;
            m_ColorClear = config.ColorClear;

            base.Configure(args);
        }


        public void Draw(IPixel pixel) =>
            m_PictureController.PixelColorize(pixel, m_ColorDraw);

        public void Clear(IPixel pixel) =>
            m_PictureController.PixelColorize(pixel, m_ColorClear);
    }


    public struct PencilConfig
    {
        public PencilConfig(PictureController pictureController, Color colorDraw, Color colorClear)
        {
            PictureController = pictureController;
            ColorDraw = colorDraw;
            ColorClear = colorClear;
        }

        public PictureController PictureController { get; private set; }
        public Color ColorDraw { get; private set; }
        public Color ColorClear { get; private set; }
        
    }
}