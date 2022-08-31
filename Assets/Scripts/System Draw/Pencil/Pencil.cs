using System;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class Pencil: AConfigurable, IConfigurable
    {        
        private PictureController m_PictureController;

        [SerializeField] private Color m_ColorDraw = Color.green;
        [SerializeField] private Color m_ColorClear = Color.black;
        

        public Pencil() { }
        public Pencil(params object[] args)
            => Configure(args);
        
        public override void Configure(params object[] args)
        {
            var config = (PencilConfig)args[PARAM_INDEX_Config];
           
            m_PictureController = config.PictureController;
            m_ColorDraw = config.ColorDraw;
            m_ColorClear = config.ColorClear;

            base.Configure(args);
        }


        public void Draw() =>
            m_PictureController.PixelColorize(m_ColorDraw);

        public void Clear() =>
            m_PictureController.PixelColorize(m_ColorClear);
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