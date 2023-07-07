using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Net
{
    public abstract class NetModel : ModelLoadable
    {
        private NetConfig m_Config;

        public INet Instance { get; private set; }
        
        public int LayerRecognizeDimension { get; private set; }
        public int LayerRecognizeSize { get; private set; }
        public int LayerAnalyzeDimension { get; private set; }
        public int LayerAnalyzeSize { get; private set; }
        public int LayerResponseDimension { get; private set; }
        public int LayerResponseSize { get; private set; }
        
        public Color ColorBackground { get; private set; }
        public Color ColorActive { get; private set; }
        public int LayerMask { get => gameObject.layer; private set => gameObject.layer = value; }
        
        public Transform Parent { get => transform.parent; private set { if (value != null) transform.SetParent(value); } }

        public float Weight { get; private set; }
        
        public INodeController NodeController { get; private set; }


        public static readonly string PREFAB_Folder = "Prefab";

        // CONFIGURE //
        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (NetConfig)args[PARAMS_Config] :
            default(NetConfig);

            Instance = m_Config.Instance;

            LayerRecognizeDimension = m_Config.LayerRecognizeDimension;
            LayerRecognizeSize = m_Config.LayerRecognizeSize;

            LayerAnalyzeDimension = m_Config.LayerAnalyzeDimension;
            LayerAnalyzeSize = m_Config.LayerAnalyzeSize;
            
            LayerResponseDimension = m_Config.LayerResponseDimension;
            LayerResponseSize = m_Config.LayerResponseSize;

            ColorBackground = m_Config.ColorBackground;
            ColorActive = m_Config.ColorActive;
            LayerMask = m_Config.LayerMask;
            Parent = m_Config.Parent;

            if (m_Config.Parent != null)
                transform.SetParent(m_Config.Parent);

            base.Configure(args);
        }

        public override void Init()
        {
            if (VerifyOnInit())
                return;

            NodeController = NodeControllerDefault.Get();
            var nodeControllerConfig = new NodeControllerConfig();
            NodeController.Configure(nodeControllerConfig);
            NodeController.Init();

            var layerRecognize = CreateLayerRecognize();
            NodeController.SetLayerRecognize(layerRecognize);

            var layerAnalyze = CreateLayerAnalyze();
            NodeController.SetLayerAnalyze(layerAnalyze);

            var layerResponse = CreateLayerResponse();
            NodeController.SetLayerResponse(layerResponse);



            base.Init();
        }

        public override void Dispose()
        {

            NodeController.Dispose();
            base.Dispose();
        }

        public override void Activate()
        {

            base.Activate();
            NodeController.Activate();
        }

        public override void Deactivate()
        {
            NodeController.Deactivate();
            base.Deactivate();
        }


        public abstract void Recognize(IRecognizable recognizable);
        public abstract void Analyze();
        public abstract void Response();

        protected abstract INode[,] CreateLayerRecognize();
        protected abstract INode[,] CreateLayerAnalyze();
        protected abstract INode[,] CreateLayerResponse();


        protected INode[,] CreateLayer<TNode>(int dimension)
        where TNode : INode
            => CreateLayer<TNode>(dimension, 1);

        protected INode[,] CreateLayer<TNode>(int dimension, int size)
        where TNode : INode
        {
            var matrix = new INode[dimension, size];

            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < dimension; i++)
                {
                    var node = CreateNode<TNode>(Vector3.zero);
                    node.SetWeight(UnityEngine.Random.Range(0.0f, 1.0f));
                    matrix[i, j] = node;
                }
            }

            return matrix;
        }

        protected INode CreateNode<TNode>(Vector3 position)
        where TNode : INode
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
        where TNet : INet
        {
            IFactory factoryCustom = null;

            if (args.Length > 0)
                try { factoryCustom = (IFactory)args[PARAMS_Factory]; }
                catch { Debug.Log("Custom factory not found! The instance will be created by default."); }


            var factory = (factoryCustom != null) ? factoryCustom : new NetFactory();
            var instance = factory.Get<TNet>(args);

            return instance;
        }
    }

    public struct NetConfig
    {
        public NetConfig(INet instance,
                         Camera camera,
                         int layerRecognizeDimension,
                         int layerRecognizeSize,
                         int layerAnalyzeDimension,
                         int layerAnalyzeSize,
                         int layerResponseDimension,
                         int layerResponseSize,
                         Color colorBackground,
                         Color colorActive,
                         int layerMask,
                         Transform parent)
        {
            Instance = instance;
            Camera = camera;
            LayerRecognizeDimension = layerRecognizeDimension;
            LayerRecognizeSize = layerRecognizeSize;
            LayerAnalyzeDimension = layerAnalyzeDimension;
            LayerAnalyzeSize = layerAnalyzeSize;
            LayerResponseDimension = layerResponseDimension;
            LayerResponseSize = layerResponseSize;
            ColorBackground = colorBackground;
            ColorActive = colorActive;
            LayerMask = layerMask;
            Parent = parent;
        }

        public INet Instance { get; private set; }
        public Camera Camera { get; private set; }
        public int LayerRecognizeDimension { get; private set; }
        public int LayerRecognizeSize { get; private set; }
        public int LayerAnalyzeDimension { get; private set; }
        public int LayerAnalyzeSize { get; private set; }
        public int LayerResponseDimension { get; private set; }
        public int LayerResponseSize { get; private set; }

        public Color ColorBackground { get; private set; }
        public Color ColorActive { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }

    }

    public interface INet : IConfigurable, ICacheable, IActivable, IComponent, IMessager
    {
        void Recognize(IRecognizable recognizable);
        void Analyze();
        void Response();
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