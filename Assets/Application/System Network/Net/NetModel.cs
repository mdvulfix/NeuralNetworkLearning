using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Network
{
    public abstract class NetModel : ModelCacheable
    {
        private NetConfig m_Config;
        
        private List<INode> m_Nodes;

        public INet Instance {get; private set; }

        public int InputLayerDimension { get; private set; }
        public int AnalyzeLayerDimension { get; private set; }
        public int AnalyzeLayerNumber { get; private set; }

        public int LayerMask {get => gameObject.layer; private set => gameObject.layer = value; }
        public Transform Parent { get => transform.parent; private set { if (value != null) transform.SetParent(value); } }

        public INodeController NodeController { get; private set; }
        
        public Color ColorBackground {get; private set; }
        public Color ColorActive {get; private set; }

        
                
        public static readonly string PREFAB_Folder = "Prefab";

        // CONFIGURE //
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (NetConfig)args[PARAMS_Config] :
            default(NetConfig);

            Instance = m_Config.Instance;

            InputLayerDimension = m_Config.InputLayerDimension;
            AnalyzeLayerDimension = m_Config.AnalyzeLayerDimension;
            AnalyzeLayerNumber = m_Config.AnalyzeLayerNumber;

            ColorBackground = m_Config.ColorBackground;
            ColorActive = m_Config.ColorActive;
            LayerMask = m_Config.LayerMask;
            Parent = m_Config.Parent;

            if(m_Config.Parent != null)
                transform.SetParent(m_Config.Parent);
            
            base.Configure(args);
        }

        public override void Init()
        {
            if(VerifyOnInit())
                return;
            
            m_Nodes = new List<INode>(100);
                        
            NodeController = NodeControllerDefault.Get();
            var nodeControllerConfig = new NodeControllerConfig();
            NodeController.Configure(nodeControllerConfig);
            NodeController.Init();

            var inputLayer = CreateLayerInput();
            NodeController.SetLayerInput(inputLayer);
            
            var analyzeLayer = CreateLayerAnalyze();
            NodeController.SetLayerAnalyze(analyzeLayer);
            
            
            base.Init();
        }

        public override void Dispose()
        {
            foreach (var node in m_Nodes)
                node.Dispose();

            m_Nodes.Clear();

            base.Dispose();
        }


        public override void Activate()
        {
            foreach (var node in m_Nodes)
                node.Activate(); 

            base.Activate();  
        }

        public override void Deactivate()
        {
            foreach (var node in m_Nodes)
                node.Deactivate(); 

            base.Deactivate();  
        }

        
        public abstract void Recognize(IRecognizable recognizable);
        public abstract void Analyze();
        public abstract void Response();

        protected abstract INode[,,] CreateLayerInput();
        protected abstract INode[,,] CreateLayerAnalyze();

        protected INode[,,] CreateLayer<TNode>(int layerDimension, int layerNumber = 1)
        where TNode: INode
            => CreateLayer<TNode>(layerDimension, layerDimension, layerNumber);

        protected INode[,,] CreateLayer<TNode>(int xSize, int ySize, int layerNumber = 1)
        where TNode: INode
        {
            var matrix = new INode[xSize, ySize, layerNumber];
            for (int l = 0; l < layerNumber; l++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    for (int x = 0; x < xSize; x++)
                    {
                        var position = new Vector3(x - (xSize / 2), y - (ySize / 2), l);
                        var node = GetNode<TNode>(position);

                        matrix[x, y, l] = node;
                        m_Nodes.Add(node);
                    }
                }
            }

            return matrix;
        }


        private INode GetNode<TNode>(Vector3 position)
        where TNode: INode
        {
            var node = NodeModel.Get<TNode>();
            
            GetComponent<Transform>(out var nodeParent);
            var nodelConfig = new NodeConfig(node, position, ColorBackground, ColorActive, LayerMask, nodeParent);
            node.Configure(nodelConfig);
            node.Init();

            return node;
        }
        
        
        
        // FACTORY //
        public static TNet Get<TNet>(params object[] args)
        where TNet: INet
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new NetFactory();
            var instance = factory.Get<TNet>(args);
            
            return instance;
        }
    }

    public struct NetConfig
    {
        public NetConfig(INet instance, int inputLayerDimension, int analyzeLayerDimension, int analyzeLayerNumber, Color colorBackground, Color colorActive, int layerMask, Transform parent)
        {
            Instance = instance;
            InputLayerDimension = inputLayerDimension;
            AnalyzeLayerDimension = analyzeLayerDimension;
            AnalyzeLayerNumber = analyzeLayerNumber;
            ColorBackground = colorBackground;
            ColorActive = colorActive;
            LayerMask = layerMask;
            Parent = parent;

        }

        public INet Instance { get; private set; }
        public int InputLayerDimension { get; private set; }
        public int AnalyzeLayerDimension { get; private set; }
        public int AnalyzeLayerNumber { get; private set; }
        public Color ColorBackground { get; private set; }
        public Color ColorActive { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }

    }

    public interface INet: IConfigurable, ICacheable, IActivable, IComponent, IMessager
    {

    }

    public partial class NetFactory : Factory<INet>
    {
        private string m_Label = "Net";
        
        public NetFactory()
        {
            Set<Net2D>(Constructor.Get((args) => GetNet2D(args)));

        }
    }

    public interface IRecognizable
    {


    }

}