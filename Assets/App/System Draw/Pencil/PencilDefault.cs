using System;
using UnityEngine;


namespace APP.Draw
{
    public class PencilDefault: PencilModel, IPencil
    {
        public PencilDefault() { }
        public PencilDefault(params object[] args)
            => Configure(args);

        public override void Clear(IPixel pixel) { }
        public override void Draw(IPixel pixel) { }
    }
    

    [Serializable]
    public abstract class PencilModel: ModelConfigurable
    {
        private PencilConfig m_Config;
        
        private IPictureController m_PictureController;

        public Color ColorDraw { get; private set; }
        public Color ColorClear { get; private set; }

        public IPencil Pencil {get; private set; }


        public static readonly string PREFAB_Folder = "Prefab";
        
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (PencilConfig)args[PARAMS_Config] :
            default(PencilConfig);

            Pencil = m_Config.Instance;
            ColorDraw = m_Config.ColorDraw;
            ColorClear = m_Config.ColorClear;

            base.Configure(args);
        }


        public abstract void Clear(IPixel pixel);
        public abstract void Draw(IPixel pixel);



        // FACTORY //
        public static PencilDefault Get(params object[] args) 
            => Get<PencilDefault>(args);

        public static TPencil Get<TPencil>(params object[] args)
        where TPencil: IPencil
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new FactoryDefault();
            var instance = factory.Get<TPencil>(args);
            
            return instance;
        }

    }





    public interface IPencil : IConfigurable
    {
        void Clear(IPixel pixel);
        void Draw(IPixel pixel);
    }

    public struct PencilConfig
    {
        public PencilConfig(IPencil instance, Color colorDraw, Color colorClear)
        {
            Instance = instance;
            ColorDraw = colorDraw;
            ColorClear = colorClear;

        }

        public IPencil Instance { get; private set; }
        public Color ColorDraw { get; private set; }
        public Color ColorClear { get; private set; }

    }
}