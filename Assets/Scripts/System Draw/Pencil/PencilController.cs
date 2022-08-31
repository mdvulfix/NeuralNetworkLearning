using System;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class PencilController : AConfigurable, IConfigurable, IUpdateble
    {
        private PencilControllerConfig m_Config;

        private Color m_ColorDraw = Color.green;
        private Color m_ColorClear = Color.black;
        private PictureController m_PictureController;


        public Pencil Pencil { get; private set; }

        public PencilController() { }
        public PencilController(params object[] args)
            => Configure(args);

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
            Pencil = new Pencil(pencilConfig);
             
            UpdateController.SetUpdateble(this);

            base.Init();
        }


        public override void Dispose()
        {
            Pencil.Dispose();
            UpdateController.RemoveUpdateble(this);

            base.Dispose();
        }


        public void Update()
        {
            if (Input.GetMouseButton(0))
                Pencil.Draw();

            if (Input.GetMouseButton(1))
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

