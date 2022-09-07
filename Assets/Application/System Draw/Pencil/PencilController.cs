using System;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace APP.Draw
{
    [Serializable]
    public class PencilController : AController, IController //, IUpdateble
    {

        private Color m_ColorDraw = Color.green;
        private Color m_ColorClear = Color.black;
        private PictureController m_PictureController;
        
        [SerializeField] private IPencil m_Pencil;

        public PencilController() { }
        public PencilController(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            var config = (PencilControllerConfig)args[PARAM_INDEX_Config];

            m_Pencil = config.Pencil;
            
            base.Configure(args);
        }

        public override void Init()
        {

            m_Pencil.Init();
            
            base.Init();
        }


        public override void Dispose()
        {
            m_Pencil.Dispose();

            base.Dispose();
        }

        public void Draw(IPixel pixel) 
            => m_Pencil.Draw(pixel);

        public void Clear(IPixel pixel) 
            => m_Pencil.Clear(pixel);

        
        public static PencilController Get(params object[] args)
            => Get<PencilController>(args);




        /*
        public void Update()
        {
            if (UInput.GetMouseButton(0))
                Pencil.Draw();

            if (UInput.GetMouseButton(1))
                Pencil.Clear();

        }
        */
    }

    public struct PencilControllerConfig
    {
        public PencilControllerConfig(IPencil pencil)
        {
            Pencil = pencil;
        }


        public IPencil Pencil { get; private set; }
    }
}

