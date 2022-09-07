using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Pixel2D : PixelModel, IPixel
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Square";
        
        private SpriteRenderer m_Renderer;
        private BoxCollider2D m_Collider;
        
        public static readonly string PREFAB_Label = "Pixel2D";
        
        public Pixel2D() { }
        public Pixel2D(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if(IsConfigured == true)
                return;
        
            var backgroundColor = Color.black;
            var hoverColor = Color.grey;

            var obj = Pixel.gameObject;
            
            if (obj.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<SpriteRenderer>();

            m_Renderer.sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");
            m_Renderer.color = backgroundColor;


            if (obj.TryGetComponent<BoxCollider2D>(out m_Collider) == false)
                m_Collider = obj.AddComponent<BoxCollider2D>();
            
            m_Collider.size = Vector2.one;
            //m_Collider.offset = 0;
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            
            var position = Vector3.zero;
            var layerMask = 8;
            
            Transform parent = null;
            if(Seacher.Find<IPicture>(out var pictures))
            {
                parent = pictures[0].Picture != null ?
                pictures[0].Picture:
                Pixel.parent;
            }
                
   
            var pixelConfig = new PixelConfig(position, backgroundColor, hoverColor, layerMask, parent);
            base.Configure(pixelConfig);
            Send($"{ this.GetName() } was configured by default!");
        }



        public override void Init()
        {
            if(IsInitialized == true)
                return;
            
            
            base.Init();
        }


        public override void SetColor(Color color, ColorMode mode = ColorMode.None)
        {
            if (m_Renderer.color == color)
                return;

            m_Renderer.color = color;
            base.SetColor(color, mode);
        }
        
        
        
        public void Excite()
        {
            
        }

        // FACTORY //
        public static Pixel2D Get(params object[] args)
            => Get<Pixel2D>(args);

    }


    public partial class PixelFactory : Factory<IPixel>
    {
        private Pixel2D GetPixel2D(params object[] args)
        {
            var prefabPath = $"{PixelModel.PREFAB_Folder}/{Pixel2D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Pixel");

            var instance = obj.AddComponent<Pixel2D>();

            if(args.Length > 0)
            {
                var config = (PixelConfig)args[PixelModel.PARAM_INDEX_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}