using System;
using UnityEngine;
using UnityEngine.UI;

namespace APP.Network
{
    [Serializable]
    public abstract class NodeModel: ModelCacheable
    {        

        [SerializeField] private float m_Weight;
        
        private NodeConfig m_Config;

        private int m_LayerMask;
        
        
        public INode Instance {get; private set;}
        
        public Transform Parent { get => transform.parent; private set { if (value != null) transform.SetParent(value); } }
        public Vector3 Position { get => transform.position; protected set => transform.position = value; }

        public Color ColorDefault {get; protected set; } = Color.grey;
        public Color ColorActive {get; protected set; } = Color.green;

        public float Weight {get => m_Weight; protected set => m_Weight = value;  }

                
        public static readonly string PREFAB_Folder = "Prefab";

        

        // CONFIGURE //
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;


            m_Config = args.Length > 0 ?
            (NodeConfig)args[PARAMS_Config] :
            default(NodeConfig);
        
            Instance = m_Config.Instance;
            Position = m_Config.Position;
            
            transform.name = $"Node ({Position.x.ToString()}; {Position.y.ToString()})";
            gameObject.layer = m_Config.LayerMask;

            if(m_Config.Parent != null )
                transform.SetParent(m_Config.Parent);
                        
            ColorDefault = m_Config.ColorDefault;
            ColorActive = m_Config.ColorActive;
            Parent = m_Config.Parent;

            base.Configure(args);
        }

    
        public abstract void OnActivated(bool activated);
        
        public abstract void SetColor(Color color);
        public abstract void SetPosition(Vector3 position);
        public abstract void SetWeight(float weight);


        // FACTORY //
        public static TNode Get<TNode>(params object[] args)
        where TNode: INode
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new NodeFactory();
            var instance = factory.Get<TNode>(args);
            
            return instance;
        }
    }
    
    
    
    
    
    
    public interface INode: IConfigurable, ICacheable, IActivable, IMessager
    {
        Vector3 Position { get; }

        Color ColorDefault {get; }
        Color ColorActive {get; }
    
    
        void SetColor(Color color);
        void SetPosition(Vector3 position);
        void SetWeight(float weight);
    
    
    }



    public struct NodeConfig
    {
        public INode Instance { get; private set; }
        public Vector3 Position { get; private set; }
        public Color ColorDefault { get; private set; }
        public Color ColorActive { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }


        public NodeConfig(INode instance, Vector3 position, Color colorDefault, Color colorActive, int layerMask, Transform parent = null)
        {
            ColorDefault = colorDefault;
            ColorActive = colorActive;
            LayerMask = layerMask;
            Instance = instance;
            Position = position;
            Parent = parent;

        }
    }

    public partial class NodeFactory : Factory<INode>
    {
        public NodeFactory()
        {
            Set<Node2D>(Constructor.Get((args) => GetNode2D(args)));
        }
    }

}