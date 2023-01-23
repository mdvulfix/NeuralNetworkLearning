using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Brain
{
    public abstract class BrainModel : ModelCacheable
    {
        private BrainConfig m_Config;

        private IEnumerable<ISensible> m_Sensibles;
        
        
        private List<INeuron> m_Neurons;


        public IBrain Brain { get; private set; }
        public int InputLayerDimension { get; private set; }
        public int AnalyzeLayerDimension { get; private set; }
        public int AnalyzeLayerNumber { get; private set; }
        public int LayerMask { get => gameObject.layer; private set => gameObject.layer = value; }
        public Transform Parent { get => transform.parent; private set { if (value != null) transform.SetParent(value); } }

        public INeuronController NeuronController { get; private set; }

        public static readonly string PREFAB_Folder = "Preefab";

        // CONFIGURE //
        public override void Configure(params object[] args)
        {

            if (VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (BrainConfig)args[PARAMS_Config] :
            default(BrainConfig);

            Brain = m_Config.Instance;
            InputLayerDimension = m_Config.InputLayerDimension;
            AnalyzeLayerDimension = m_Config.AnalyzeLayerDimension;
            AnalyzeLayerNumber = m_Config.AnalyzeLayerNumber;
            
            
            LayerMask = m_Config.LayerMask;
            Parent = m_Config.Parent;

            base.Configure(args);

        }

        public override void Init()
        {
            m_Neurons = new List<INeuron>(100);

            NeuronController = NeuronControllerDefault.Get();
            var neuronControllerConfig = new NeuronControllerConfig();
            NeuronController.Configure(neuronControllerConfig);
            NeuronController.Init();

            var inputLayer = CreateInputLayer();
            NeuronController.SetInputLayer(inputLayer);
            
            var analyzeLayer = CreateAnalyzeLayer();
            NeuronController.SetAnalyzeLayer(analyzeLayer);
            
            base.Init();
        }

        public override void Dispose()
        {
            NeuronController.Dispose();

            base.Dispose();
        }

        // ACTIVATE //
        public override void Activate()
        {
            NeuronController.Activate();

            base.Activate();
        }

        public override void Deactivate()
        {
            NeuronController.Deactivate();

            base.Deactivate();
        }

        // UPDATE //
        public void Update()
        {
            NeuronController.Update();

        }

        
        public abstract void Recognize(IRecognizable recognizable);
        public abstract void Analyze();
        public abstract void Response();

        protected abstract INeuron[,,] CreateInputLayer();
        protected abstract INeuron[,,] CreateAnalyzeLayer();

        protected INeuron[,,] CreateLayer<TNeuron>(int layerDimension, int layerNumber = 1)
        where TNeuron: INeuron
            => CreateLayer<TNeuron>(layerDimension, layerDimension, layerNumber);

        protected INeuron[,,] CreateLayer<TNeuron>(int xSize, int ySize, int layerNumber = 1)
        where TNeuron: INeuron
        {
            var matrix = new INeuron[xSize, ySize, layerNumber];
            for (int l = 0; l < layerNumber; l++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    for (int x = 0; x < xSize; x++)
                    {
                        var position = new Vector3(x - (xSize / 2), y - (ySize / 2), l);
                        var neuron = GetNeuron<TNeuron>(position);

                        matrix[x, y, l] = neuron;
                        m_Neurons.Add(neuron);
                    }
                }
            }

            return matrix;
        }


        private void OnNeuronDivided(INeuron neuron)
        {

        }

        private void OnNeuronDead(INeuron neuron)
        {

        }

        private INeuron GetNeuron<TNeuron>(Vector3 position)
        where TNeuron: INeuron
        {
            var neuron = NeuronModel.Get<TNeuron>();
            
            var size = 0.5f;
            var energy = 50;
            GetComponent<Transform>(out var parent);

            var neuronConfig = new NeuronConfig(neuron, position, size, energy, LayerMask, parent);
            neuron.Configure(neuronConfig);
            neuron.Init();

            return neuron;
        }

        // FACTORY // 
        public static BrainDefault Get(params object[] args)
            => Get<BrainDefault>(args);

        // FACTORY //
        public static TBrain Get<TBrain>(params object[] args)
        where TBrain : IBrain
        {
            IFactory factoryCustom = null;

            if (args.Length > 0)
                try { factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }


            var factory = (factoryCustom != null) ? factoryCustom : new BrainFactory();
            var instance = factory.Get<TBrain>(args);

            return instance;
        }
    }



    public interface IBrain : IConfigurable, ICacheable, IActivable, IUpdateble, IMessager
    {
        void Recognize(IRecognizable recognizable);
        void Analyze();
        void Response();
    }

    public interface IRecognizable
    {
        IEnumerable<ISensible> GetSensibles();

    }


    public struct BrainConfig : IConfig
    {
        public BrainConfig(IBrain instance, int inputLayerDimension, int analyzeLayerDimension, int analyzeLayerNumber, int layerMask, Transform parent)
        {
            Instance = instance;
            InputLayerDimension = inputLayerDimension;
            AnalyzeLayerDimension = analyzeLayerDimension;
            AnalyzeLayerNumber = analyzeLayerNumber;
            LayerMask = layerMask;
            Parent = parent;
        }

        public IBrain Instance { get; private set; }
        public int InputLayerDimension { get; }
        public int AnalyzeLayerDimension { get; }
        public int AnalyzeLayerNumber { get; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; internal set; }
    }



    public partial class BrainFactory : Factory<IBrain>
    {
        private string m_Label = "Brain";

        public BrainFactory()
        {

            Setup();
            //Set<BrainDefault>(Constructor.Get((args) => GetBrainDefault(args)));
        }

        public partial void Setup();
    }

}