using System;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class Pencil
    {
        private PencilConfig m_Config;
        private PictureController m_PictureController;

        [SerializeField] private Color m_ColorDraw = Color.green;
        [SerializeField] private Color m_ColorClear = Color.black;
        

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is PencilConfig)
                    {
                        m_Config = (PencilConfig)args[0];
                        
                        m_PictureController = m_Config.PictureController;
                        m_ColorDraw = m_Config.ColorDraw;
                        m_ColorClear = m_Config.ColorClear;
                    }
                }
            }
        }


        public void Init()
        {
 
        }

        public void Dispose()
        {

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