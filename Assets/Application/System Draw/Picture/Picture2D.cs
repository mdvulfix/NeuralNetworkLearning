using UnityEngine;

namespace APP.Draw
{
    public class Picture2D : PictureModel, IPicture
    {
        private Transform m_Parent;

        private int m_Width;
        private int m_Height;

        private int m_LayerMask;

        private Color m_PixelColorDefault;
        private Color m_PixelColorHover;

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

            
            // CONFIGURE BU DEFAULT //
            m_Width = 5;
            m_Height = 5;
            m_PixelColorDefault = Color.black;
            m_PixelColorHover = Color.grey;
            m_LayerMask = 8;

            
            if(Seacher.Find<IScene>(out var scenes))
                m_Parent = scenes[0].Scene;
   
            var pixelConfig = new PictureConfig(m_Width, m_Height, m_PixelColorDefault, m_PixelColorHover, m_LayerMask, m_Parent);
            base.Configure(pixelConfig);
            Send($"{ this.GetName() } was configured by default!");
        }


        public override IPixel GetPixel(Vector3 position)
        {
            var pixel = Pixel2D.Get();
            var pixelParent = GetTransform();
            var pixelConfig = new PixelConfig(pixel, position, m_PixelColorDefault, m_PixelColorHover, m_LayerMask, pixelParent);
            pixel.Configure(pixelConfig);

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