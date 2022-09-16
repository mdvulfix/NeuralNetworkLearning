using UnityEngine;

namespace APP.Draw
{
    public class Picture2D : PictureModel, IPicture
    {
        public static readonly string PREFAB_Label = "Picture2D";

        public Picture2D() { }
        public Picture2D(params object[] args)
            => Configure(args);
  

        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            
            // CONFIGURE BY DEFAULT //
            var width = 5;
            var height = 5;
            var colorDefault = Color.black;
            var colorHover = Color.grey;
            var layerMask = 8;

            Transform parent = null;
            if(Seacher.Find<IScene>(out var scenes))
                parent = scenes[0].Scene;
   
            var pictureConfig = new PictureConfig(this, width, height, colorDefault, colorHover, layerMask, parent);
            base.Configure(pictureConfig);
            Send($"{ this.GetName() } was configured by default!");
        }


        protected override IPixel GetPixel(Vector3 position)
        {
            var pixel = Pixel2D.Get();
            GetComponent<Transform>(out var pixelParent);
            var pixelConfig = new PixelConfig(pixel, position, ColorBackground, ColorHover, LayerMask, pixelParent);
            pixel.Configure(pixelConfig);
            pixel.Init();

            return pixel;
        }

        public static Picture2D Get(params object[] args)
            => Get<Picture2D>(args);
    }

    public partial class PictureFactory : Factory<IPicture>
    {
        private Picture2D GetPicture2D(params object[] args)
        {
            
            var prefabPath = $"{PictureModel.PREFAB_Folder}/{Picture2D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Picture");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<Picture2D>();

            //var instance = new Picture2D();

            if(args.Length > 0)
            {
                var config = (PictureConfig)args[PictureModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }

    }
}