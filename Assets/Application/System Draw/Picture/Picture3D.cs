using UnityEngine;

namespace APP.Draw
{
    public class Picture3D : PictureModel<Pixel3D>, IPicture
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Square";
        
        public Picture3D() { }
        public Picture3D(params object[] args)
            => Configure(args);
  

        public override void Configure(params object[] args)
        {
            if(IsConfigured == true)
                return;
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            
            var width = 5;
            var height = 5;
            var backgroundColor = Color.black;
            var hoverColor = Color.grey;

            var layerMask = 8;

            
            Transform parent = null;
            if(Seacher.Find<IScene>(out var scenes))
                parent = scenes[0].Scene;
   
            var pixelConfig = new PictureConfig(width, height, backgroundColor, hoverColor, layerMask, parent);
            base.Configure(pixelConfig);
            Send($"{ this.GetName() } was configured by default!");
        }



        public override void Init()
        {
            if(IsInitialized == true)
                return;
            
            base.Init();
        }

        public static Picture3D Get(params object[] args)
            => Get<Picture3D>(args);

    }

    public partial class PictureFactory : Factory<IPicture>
    {       
        private Picture3D GetPicture3D(params object[] args)
        {
            var instance = new Picture3D();

            if(args.Length > 0)
            {
                var config = (PictureConfig)args[PictureModel<Pixel3D>.PARAM_INDEX_Config];
                instance.Configure(config);
            }

            return instance;
        }

    }
}