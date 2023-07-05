using System;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{

    public class Pixel3D : PixelModel, IPixel
    {
        private MeshRenderer m_Renderer;
        private BoxCollider m_Collider;

        private Color m_Color;

        public static readonly string PREFAB_Label = "Pixel3D";


        public Pixel3D() { }
        public Pixel3D(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            var backgroundColor = Color.black;
            var hoverColor = Color.grey;

            var obj = gameObject;

            if (obj.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<MeshRenderer>();

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

            // CONFIGURE BY DEFAULT //
            var position = Vector3.zero;
            var layerMask = 8;

            Transform parent = null;
            if (Seacher.Find<IPicture>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var pictureParent))
                    parent = pictureParent != null ? pictureParent : transform.parent;


            var pixelConfig = new PixelConfig(this, position, backgroundColor, hoverColor, layerMask, parent);
            base.Configure(pixelConfig);
            Send($"{this.GetName()} was configured by default!");
        }


        public override void OnSelected(bool selected)
        {
            if (selected == true)
            {
                if (GetColor() == ColorSelect)
                    return;

                m_Color = ColorSelect;
                SetColor(m_Color);
                return;
            }

            m_Color = ColorDefault;
            SetColor(m_Color);
        }

        public override void OnHovered(bool hovered)
        {
            if (hovered == true)
            {
                if (GetColor() == ColorHover)
                    return;

                SetColor(ColorHover);
                return;
            }
            
            SetColor(m_Color);
        }


        protected override void SetColor(Color color)
            => m_Renderer.material.SetColor("_Color", color);

        private Color GetColor()
            => m_Renderer.sharedMaterial.color;
        
        
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
            

            //var instance = new Pixel3D();

            if (args.Length > 0)
            {
                var config = (PixelConfig)args[PixelModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}