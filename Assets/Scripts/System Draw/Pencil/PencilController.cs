using System;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace APP.Draw
{
    [Serializable]
    public class PencilController : AConfigurable, IConfigurable, IUpdateble
    {

        private Color m_ColorDraw = Color.green;
        private Color m_ColorClear = Color.black;
        private PictureController m_PictureController;
        
        [SerializeField] private Pencil m_Pencil;

        public Pencil Pencil  => m_Pencil;

        public PencilController() { }
        public PencilController(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {
            var config = (PencilControllerConfig)args[PARAM_INDEX_Config];

            m_PictureController = config.PictureController;
            m_ColorDraw = config.ColorDraw;
            m_ColorClear = config.ColorClear;

            base.Configure(args);
        }

        public override void Init()
        {
            var pencilConfig = new PencilConfig(m_PictureController, m_ColorDraw, m_ColorClear);
            m_Pencil = new Pencil(pencilConfig);
            
            base.Init();
        }


        public override void Dispose()
        {
            Pencil.Dispose();

            base.Dispose();
        }


        public void Update()
        {
            if (UInput.GetMouseButton(0))
                Pencil.Draw();

            if (UInput.GetMouseButton(1))
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

