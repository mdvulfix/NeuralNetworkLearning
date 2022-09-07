using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Brain
{
    
    public class BrainDefault :  BrainModel, IBrain
    {
        public BrainDefault() { }
        public BrainDefault(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {
            

            base.Configure(args);
        }
    }
    
    
        
    public abstract class BrainModel : AConfigurable, IConfigurable
    {       
        private IRecognizable m_Recognizable;
        private IEnumerable<ISensible> m_Sensibles;
    
        private List<INeuron> m_Neurons;
        private NeuronController m_NeuronController;

        private INeuron[,,] m_LayerMatrixInput;
        private INeuron[,,] m_LayerMatrixAnalize;
        
    
        private Vector3Int m_MatrixSize;
        private int m_MatrixDimension = 8;


        public override void Configure(params object[] args)
        {
            
            
            var config = (BrainConfig)args[PARAM_INDEX_Config];
            m_Recognizable = config.Recognizable;
            
            base.Configure(args);

        }   

        public override void Init()
        {
            m_Sensibles = m_Recognizable.GetSensibles();

            m_Neurons = new List<INeuron>();
            //m_MatrixSize = new Vector3Int(m_MatrixDimension, m_MatrixDimension, m_MatrixDimension);
            //m_LayerMatrixAnalize = new Neuron[m_MatrixSize.x, m_MatrixSize.y, m_MatrixSize.z];
            
            // Build input layer
            
            var sensorNumber  = (from ISensible sensible in m_Sensibles select sensible).Count();
            
            //var inputLayerSize = new Vector3Int(sensorNumber/2, sensorNumber - sensorNumber/2, 0);
            //m_LayerMatrixInput = new Neuron[inputLayerSize.x, inputLayerSize.y, inputLayerSize.z];
            
            
            foreach (var sensible in m_Sensibles)
            {
                var neuron = NeuronModel.Get<NeuronDefault>();

                var neuronPosition = new Vector3(sensible.Position.x, sensible.Position.y, sensible.Position.z - 5);
                var neuronSize = Random.Range(0f, 100f);
                var neuronEnergy = Random.Range(0f, 100f);
                var neuronConfig = new NeuronConfig(neuron, neuronSize, neuronEnergy, neuronPosition);
                neuron.Configure(neuronConfig);
                neuron.Init();
                
                var sensor = neuron.GetSensor();
                sensor.Attach(sensible);
            }
            
            
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
            {
                neuron.Dispose();

                neuron.Divided -= OnNeuronDivided;
                neuron.Dead -= OnNeuronDead;
            }

            base.Dispose();
        }


        private INeuron Clone(float size, float energy, Vector3 position)
        { 
            var neuron = NeuronModel.Get<NeuronDefault>();
            var neuronConfig = new NeuronConfig(neuron, size, energy, position);

            neuron.Divided += OnNeuronDivided;
            neuron.Dead += OnNeuronDead;
            neuron.Configure(neuronConfig);
            neuron.Init();

            return neuron;
        }


        private void OnNeuronDivided(INeuron neuron)
        { 

        }

        private void OnNeuronDead(INeuron neuron)
        { 
            
        }


        public static TBrain Get<TBrain>(params object[] args)
        where TBrain: IBrain, new() //Component, IBrain
        {
            var obj = new GameObject("Brain");
            obj.SetActive(false);

            //var renderer = obj.AddComponent<MeshRenderer>();           
            //var instance = obj.AddComponent<TBrain>();
            var instance = new TBrain();
            
            if(args.Length > 0)
            {
                instance.Configure(args);
                instance.Init();
            }
            
            return instance;
        }






    }

    public interface IBrain: IConfigurable
    {

    }

    public struct BrainConfig: IConfig
    {
        public BrainConfig(IBrain brain, IRecognizable recognizable)
        {
            Brain = brain;
            Recognizable = recognizable;
        }

        public IBrain Brain { get; private set;}
        public IRecognizable Recognizable { get; private set; }
    }

    public interface IRecognizable
    {
        IEnumerable<ISensible> GetSensibles();

    }
}