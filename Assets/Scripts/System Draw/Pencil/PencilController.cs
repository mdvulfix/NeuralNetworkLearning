using System;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class PencilController: IUpdateble
    {
        private PencilControllerConfig m_Config;
        
        private Color m_ColorDraw = Color.green;
        private Color m_ColorClear = Color.black;
        private PictureController m_PictureController;
        
        
        public Pencil Pencil {get; private set; }
        

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is PencilControllerConfig)
                    {
                        m_Config = (PencilControllerConfig)args[0];
                        
                        m_PictureController = m_Config.PictureController;
                        m_ColorDraw = m_Config.ColorDraw;
                        m_ColorClear = m_Config.ColorClear;
                        
                    }    
                }
            } 
        }

        public virtual void Init()
        {
            var pencilConfig = new PencilConfig(m_PictureController, m_ColorDraw, m_ColorClear);
            
            Pencil = new Pencil();
            Pencil.Configure(pencilConfig);
            Pencil.Init();


            UpdateController.SetUpdateble(this);
            
        }

        
        public virtual void Dispose()
        {
            Pencil.Dispose();

            UpdateController.RemoveUpdateble(this);
        }

        
        public void Update()
        {
            if(Input.GetMouseButton(0))
                Pencil.Draw();

            if(Input.GetMouseButton(1))
                Pencil.Clear();

        }
    }

    public struct PencilControllerConfig
    {
        public PencilControllerConfig(Color colorClear, Color colorDraw, PictureController pictureController)
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

