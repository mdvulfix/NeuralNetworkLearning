using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    public abstract class BrainModel : ModelCacheable
    {
        private BrainConfig m_Config;

        private IEnumerable<ISensible> m_Sensibles;

        private List<INeuron> m_Neurons;
        
        
        
        private NeuronController m_NeuronController;

        private INeuron[,,] m_LayerMatrixInput;
        private INeuron[,,] m_LayerMatrixAnalize;


        private Vector3Int m_MatrixSize;
        private int m_MatrixDimension = 8;
        internal static readonly object PREFAB_Folder;

        public IBrain Brain { get; private set; }
        public int LayerMask {get => gameObject.layer; private set => gameObject.layer = value; }
        public Transform Parent { get => transform.parent; private set { if(value != null) transform.SetParent(value); } }

        public override void Configure(params object[] args)
        {

            if (VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (BrainConfig)args[PARAMS_Config] :
            default(BrainConfig);

            Brain = m_Config.Instance;
            LayerMask = m_Config.LayerMask;
            Parent = m_Config.Parent;

            base.Configure(args);

        }

        public override void Init()
        {            
            m_Neurons = new List<INeuron>();


            //m_MatrixSize = new Vector3Int(m_MatrixDimension, m_MatrixDimension, m_MatrixDimension);
            //m_LayerMatrixAnalize = new Neuron[m_MatrixSize.x, m_MatrixSize.y, m_MatrixSize.z];

            // Build input layer


            //var inputLayerSize = new Vector3Int(sensorNumber/2, sensorNumber - sensorNumber/2, 0);
            //m_LayerMatrixInput = new Neuron[inputLayerSize.x, inputLayerSize.y, inputLayerSize.z];


            /*
            for (int z = 0; z < inputLayerSize.z; z++)
            { 
                for (int y = 0; y < inputLayerSize.y; y++)
                { 
                    for (int x = 0; x < inputLayerSize.x; x++)
                    { 
                        var position = new Vector3(x - (inputLayerSize.x/2), y - (inputLayerSize.y/2), z - inputLayerSize.z/2);
                        var size = Random.Range(0f, 100f);
                        var energy = Random.Range(0f, 100f);
                        m_Neurons.Add(m_LayerMatrixInput[x, y, z] = Clone(size, energy, position));
                    }
                }
            }
            */

            /*
            // Build analize layer
            
            for (int z = 0; z < m_MatrixSize.z; z++)
            { 
                for (int y = 0; y < m_MatrixSize.y; y++)
                { 
                    for (int x = 0; x < m_MatrixSize.x; x++)
                    { 
                        var position = new Vector3(x - (m_MatrixSize.x/2), y - (m_MatrixSize.y/2), z - m_MatrixSize.z/2);
                        var size = Random.Range(0f, 100f);
                        var energy = Random.Range(0f, 100f);
                        m_Neurons.Add(m_Matrix[x, y, z] = Clone(size, energy, position));
                    }
                }
            }
            */

            base.Init();
        }

        public override void Dispose()
        {
            foreach (var neuron in m_Neurons)
                neuron.Dispose();

            base.Dispose();
        }


        public void Recognize(IRecognizable recognizable)
        {
            foreach (var sensible in recognizable.GetSensibles())
            {
                var position = new Vector3(sensible.Position.x, sensible.Position.y, sensible.Position.z - 5);
                var neuron = GetNeuron(position);

                m_Neurons.Add(neuron);
  
                var sensor = neuron.GetSensor();
                sensor.Associate(sensible);
            }


        }

        public override void Activate()
        {
            foreach (var neuron in m_Neurons)
                neuron.Activate(); 

            base.Activate();  
        }

        public override void Deactivate()
        {
            foreach (var neuron in m_Neurons)
                neuron.Deactivate(); 

            base.Deactivate();  
        }



        protected abstract INeuron GetNeuron(Vector3 position);


        private void OnNeuronDivided(INeuron neuron)
        {

        }

        private void OnNeuronDead(INeuron neuron)
        {

        }



        // FACTORY // 
        public static BrainDefault Get(params object[] args)
            => Get<BrainDefault>(args);

        // FACTORY //
        public static TBrain Get<TBrain>(params object[] args)
        where TBrain: IBrain
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new BrainFactory();
            var instance = factory.Get<TBrain>(args);
            
            return instance;
        }
    }

    public interface IBrain : IConfigurable, ICacheable, IActivable, IMessager
    {
        void Recognize(IRecognizable recognizable);
    }

    public interface IRecognizable
    {
        IEnumerable<ISensible> GetSensibles();

    }


    public struct BrainConfig : IConfig
    {
        public BrainConfig(IBrain instance, int layerMask, Transform parent)
        {
            Instance = instance;
            LayerMask = layerMask;
            Parent = parent;
        }

        public IBrain Instance { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; internal set; }
    }



    public partial class BrainFactory : Factory<IBrain>
    {
        private string m_Label = "Brain";
        
        public BrainFactory()
        {
            Set<BrainDefault>(Constructor.Get((args) => GetBrainDefault(args)));
        }
    }

}