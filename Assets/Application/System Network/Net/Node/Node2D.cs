using System;
using UnityEngine;

namespace APP.Network
{
    [Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Node2D : NodeModel, INode
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Circle";
        
        private SpriteRenderer m_Renderer;
        private CircleCollider2D m_Collider;

        private Color ColorCurrent => m_Renderer.color;
        
        public static readonly string PREFAB_Label = "Node2D";
        
        public Node2D() { }
        public Node2D(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
        
            var backgroundColor = Color.grey;
            var activeColor = Color.green;

            var obj = gameObject;

            if (obj.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<SpriteRenderer>();

            if (obj.TryGetComponent<CircleCollider2D>(out m_Collider) == false)
                m_Collider = obj.AddComponent<CircleCollider2D>();
            
            
            
            //var obj = Pixel.gameObject;
            m_Renderer.sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");
            m_Renderer.color = backgroundColor;
            m_Collider.radius = 0.5f;
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            // CONFIGURE BU DEFAULT //
            var position = Vector3.zero;
            var layerMask = 8;
            
            Transform parent = null;
            if (Seacher.Find<INet>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var networkParent))
                    parent = networkParent != null ? networkParent : transform.parent;
                
            var nodeConfig = new NodeConfig(this, position, backgroundColor, activeColor, layerMask, parent);
            base.Configure(nodeConfig);
            Send($"{ this.GetName() } was configured by default!");
        }



        public override void OnActivated(bool activated)
        {
            if (activated == true)
            {
                if (ColorCurrent == ColorActive)
                    return;

                SetColor(ColorActive);
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
        public static Node2D Get(params object[] args)
            => Get<Node2D>(args);

    }


    public partial class NodeFactory : Factory<INode>
    {
        private Node2D GetNode2D(params object[] args)
        {
            var prefabPath = $"{NodeModel.PREFAB_Folder}/{Node2D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Node");

            obj.SetActive(false);

            var instance = obj.AddComponent<Node2D>();
            
            //var instance = new Pixel2D();
            
            if(args.Length > 0)
            {
                var config = (NodeConfig)args[NodeModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}