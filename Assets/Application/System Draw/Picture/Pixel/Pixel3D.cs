using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{

    public class Pixel3D : PixelModel, IPixel
    {
        private MeshRenderer m_Renderer;
        private BoxCollider m_Collider;

        private Color ColorCurrent => m_Renderer.sharedMaterial.color;

        public static readonly string PREFAB_Label = "Pixel3D";


        public Pixel3D()
            => Load();

        public Pixel3D(params object[] args)
        {
            Load();
            Configure(args);
        }


        public override void Load(params object[] args)
        {
            if (VerificationOnLoad())
                return;

            var prefabPath = $"{PixelModel.PREFAB_Folder}/{Pixel3D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);

            var obj = (prefab != null) ?
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) :
            new GameObject("Pixel");

            obj.SetActive(false);

            if (obj.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<MeshRenderer>();

            if (obj.TryGetComponent<BoxCollider>(out m_Collider) == false)
                m_Collider = obj.AddComponent<BoxCollider>();

            

            
            //m_Collider.offset = 0;
            base.Load(obj.transform);

        }

        public override void Configure(params object[] args)
        {
            if (VerificationOnConfigure())
                return;

            var backgroundColor = Color.black;
            var hoverColor = Color.grey;


            if (obj.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
                m_Renderer = ga.AddComponent<MeshRenderer>();

            if (obj.TryGetComponent<BoxCollider>(out m_Collider) == false)
                m_Collider = obj.AddComponent<BoxCollider>();
            
            
            
            
            m_Renderer.material.SetColor("_Color", backgroundColor);

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
            if (Seacher.Find<IPicture>(out var pictures))
            {
                parent = pictures[0].Picture != null ?
                pictures[0].Picture :
                Pixel.parent;
            }


            var pixelConfig = new PixelConfig(this, position, backgroundColor, hoverColor, layerMask, parent);
            base.Configure(pixelConfig);
            Send($"{this.GetName()} was configured by default!");
        }


        public void OnSelected(bool selected)
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

        public void OnHovered(bool hovered)
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
            => m_Renderer.material.SetColor("_Color", color);


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


            
            
            
            
            /*
            var prefabPath = $"{PixelModel.PREFAB_Folder}/{Pixel3D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Pixel");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<Pixel3D>();
            */

            var instance = new Pixel3D();

            if (args.Length > 0)
            {
                var config = (PixelConfig)args[PixelModel.CONFIG_PARAM_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}