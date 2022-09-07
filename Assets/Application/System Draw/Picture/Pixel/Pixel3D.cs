using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{

    public class Pixel3D : PixelModel, IPixel
    {
        private MeshRenderer m_Renderer;
        private BoxCollider m_Collider;
        
        
        public static readonly string PREFAB_Label = "Pixel3D";

        public Pixel3D() { }
        public Pixel3D(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if(IsConfigured == true)
                return;
        
            var backgroundColor = Color.black;
            var hoverColor = Color.grey;

            var obj = Pixel.gameObject;
            
            if (obj.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<MeshRenderer>();

            m_Renderer.sharedMaterial.color = backgroundColor;

            if (obj.TryGetComponent<BoxCollider>(out m_Collider) == false)
                m_Collider = obj.AddComponent<BoxCollider>();
            
            //m_Collider.size = Vector2.one;
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
                pictures[0].Picture :
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
            if (m_Renderer.sharedMaterial.color == color)
                return;

            m_Renderer.sharedMaterial.color = color;
            base.SetColor(color, mode);
        }
        
        
        
        public void Excite()
        {
            
        }

        // FACTORY //
        public static Pixel3D Get(params object[] args)
            => Get<Pixel3D>(args);
    }


    

    public partial class PixelFactory : Factory<IPixel>
    {
        private Pixel3D GetPixel3D(params object[] args)
        {
            var prefabPath = $"{PixelModel.PREFAB_Folder}/{Pixel3D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Pixel");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<Pixel3D>();

            if(args.Length > 0)
            {
                var config = (PixelConfig)args[PixelModel.PARAM_INDEX_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}