using System;
using UnityEngine;
using UnityEngine.UI;

namespace APP.Net
{
    [Serializable]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Node2D : NodeModel, INode
    {
        private readonly string FOLDER_SPRITES = "Sprite";
        private string m_SpriteLabel = "Circle";
        
        [SerializeField] private Text m_WeightField;

        private Image m_Image;
        private CircleCollider2D m_Collider;

        private Color ColorCurrent => m_Image.color;

        private float m_Padding = 1.2f;
        private float m_Spacing = 1.2f;
        
        public static readonly string PREFAB_Label = "Node2D";
        
        public Node2D() { }
        public Node2D(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
        
            var colorBackground = Color.grey;
            var colorActive = Color.green;

            var obj = gameObject;

            if (obj.TryGetComponent<Image>(out m_Image) == false)
                m_Image = obj.AddComponent<Image>();

            if (obj.TryGetComponent<CircleCollider2D>(out m_Collider) == false)
                m_Collider = obj.AddComponent<CircleCollider2D>();
            

            m_WeightField = obj.GetComponentInChildren<Text>();
            m_WeightField.text = Weight.ToString();
            
            //var obj = Pixel.gameObject;
            m_Image.sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");
            m_Image.color = colorBackground;
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
                
            var nodeConfig = new NodeConfig(this, position, colorBackground, colorActive, layerMask, parent);
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

       
        public override void SetColor(Color color)
        {
            if (m_Image.color == color)
                return;

            m_Image.color = color;
        }
        
        public override void SetPosition(Vector3 position)
        {
            var canvasObj = transform.parent.gameObject;
            var canvasRectTransform = canvasObj.GetComponent<RectTransform>();
            var canvasSize = new Vector3(canvasRectTransform.rect.width, canvasRectTransform.rect.height, 0);

            Position= new Vector3(m_Padding * Size.x + position.x * Size.x * m_Spacing,
                                  m_Padding * Size.y + position.y * Size.y * m_Spacing,
                                  0);
        }
        
        public override void SetWeight(float weight)
        {
            if (Weight == weight)
                return;

            Weight = weight;
            m_WeightField.text = String.Format("{0:0.00}", Weight);
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
            var objRect = obj.GetComponent<RectTransform>();



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