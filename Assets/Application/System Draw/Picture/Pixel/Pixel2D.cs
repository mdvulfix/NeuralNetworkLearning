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

        private Color ColorCurrent => m_Renderer.color;
        
        public static readonly string PREFAB_Label = "Pixel2D";
        
        public Pixel2D() { }
        public Pixel2D(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
        
            var backgroundColor = Color.black;
            var hoverColor = Color.grey;

            var obj = gameObject;

            if (obj.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<SpriteRenderer>();

            if (obj.TryGetComponent<BoxCollider2D>(out m_Collider) == false)
                m_Collider = obj.AddComponent<BoxCollider2D>();
            
            
            
            //var obj = Pixel.gameObject;
            m_Renderer.sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");
            m_Renderer.color = backgroundColor;
            m_Collider.size = Vector2.one;
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            // CONFIGURE BU DEFAULT //
            var position = Vector3.zero;
            var layerMask = 8;
            
            Transform parent = null;
            if (Seacher.Find<IPicture>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var pictureParent))
                    parent = pictureParent != null ? pictureParent : transform.parent;
                
            var pixelConfig = new PixelConfig(this, position, backgroundColor, hoverColor, layerMask, parent);
            base.Configure(pixelConfig);
            Send($"{ this.GetName() } was configured by default!");
        }



        public override void OnSelected(bool selected)
        {
            if (selected == true)
            {
                if (ColorCurrent == ColorSelect)
                    return;

                SetColor(ColorSelect);
                return;
            }

            SetColor(ColorDefault);
        }

        public override void OnHovered(bool hovered)
        {
            if (hovered == true)
            {
                if (ColorCurrent == ColorHover)
                    return;

                SetColor(ColorHover);
                return;
            }
            
            SetColor(ColorDefault);
        }
        
        
        protected override void SetColor(Color color)
        {
            if (m_Renderer.color == color)
                return;

            m_Renderer.color = color;
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

            obj.SetActive(false);

            var instance = obj.AddComponent<Pixel2D>();
            
            //var instance = new Pixel2D();
            
            if(args.Length > 0)
            {
                var config = (PixelConfig)args[PixelModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}