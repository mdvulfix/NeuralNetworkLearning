using System;
using UnityEngine;


namespace APP.Draw
{
    [Serializable]
    public class PencilModel: AConfigurable
    {
        private IPictureController m_PictureController;

        [SerializeField] private Color m_ColorDraw = Color.green;
        [SerializeField] private Color m_ColorClear = Color.black;

        public static readonly int PARAM_INDEX_Factory = 1;

        public override void Configure(params object[] args)
        {
            var config = (PencilConfig)args[PARAM_INDEX_Config];

            m_PictureController = config.PictureController;
            m_ColorDraw = config.ColorDraw;
            m_ColorClear = config.ColorClear;

            base.Configure(args);
        }

        public void Clear(IPixel pixel) =>
            m_PictureController.Colorize(pixel, m_ColorClear);

        public void Draw(IPixel pixel) =>
            m_PictureController.Colorize(pixel, m_ColorDraw);

        // FACTORY //
        public static TPencil Get<TPencil>(params object[] args)
        where TPencil: IPencil
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAM_INDEX_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new FactoryDefault();
            var instance = factory.Get<TPencil>(args);
            
            return instance;
        }

    }


    public class PencilDefault: PencilModel, IPencil
    {
        public PencilDefault() { }
        public PencilDefault(params object[] args)
            => Configure(args);


        // FACTORY //
        public static PencilDefault Get(params object[] args) 
            => Get<PencilDefault>(args);

    }


    public interface IPencil : IConfigurable
    {
        void Clear(IPixel pixel);
        void Draw(IPixel pixel);
    }

    public struct PencilConfig
    {
        public PencilConfig(IPictureController pictureController, Color colorDraw, Color colorClear)
        {
            PictureController = pictureController;
            ColorDraw = colorDraw;
            ColorClear = colorClear;
        }

        public IPictureController PictureController { get; private set; }
        public Color ColorDraw { get; private set; }
        public Color ColorClear { get; private set; }
        
    }
}