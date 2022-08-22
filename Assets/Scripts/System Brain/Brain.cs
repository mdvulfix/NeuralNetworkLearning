using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Brain
{
    public class Brain : MonoBehaviour, IConfigurable
    {
        private BrainConfig m_Config;

        private IRecognizable m_Recognizable;
        private IEnumerable<ISensible> m_Sensibles;
        

        private List<Neuron> m_Neurons;
        private NeuronController m_NeuronController;

        private Neuron[,,] m_LayerMatrixInput;
        private Neuron[,,] m_LayerMatrixAnalize;
        
        
        
        private Vector3Int m_MatrixSize;
        private int m_MatrixDimension = 8;
        

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is BrainConfig)
                    {
                        m_Config = (BrainConfig)arg;
                        
                        m_Recognizable = m_Config.Recognizable;
                        m_Sensibles = m_Recognizable.GetSensibles();


                    }
                }
            }
            
            
            
            
            m_Neurons = new List<Neuron>();
            m_MatrixSize = new Vector3Int(m_MatrixDimension, m_MatrixDimension, m_MatrixDimension);
            m_LayerMatrixAnalize = new Neuron[m_MatrixSize.x, m_MatrixSize.y, m_MatrixSize.z];
        }   

        public virtual void Init()
        {
            
            // Build input layer
            
            var sensorNumber  = (from ISensible sensible in m_Sensibles select sensible).Count();
            
            var inputLayerSize = new Vector3Int(sensorNumber/2, sensorNumber - sensorNumber/2, 0);
            m_LayerMatrixInput = new Neuron[inputLayerSize.x, inputLayerSize.y, inputLayerSize.z];
            
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
        }

        public virtual void Dispose()
        {
            foreach (var neuron in m_Neurons)
            {
                neuron.Dispose();

                neuron.Divided -= OnNeuronDivided;
                neuron.Dead -= OnNeuronDead;
            }
        }


        private Neuron Clone(float size, float energy, Vector3 position)
        { 
            var neuron = Neuron.Get();

            neuron.Divided += OnNeuronDivided;
            neuron.Dead += OnNeuronDead;

            var neuronConfig = new NeuronConfig(size, energy, position);
            neuron.Configure(neuronConfig);
            neuron.Init();

            return neuron;
        }


        private void OnNeuronDivided(Neuron neuron)
        { 

        }

        private void OnNeuronDead(Neuron neuron)
        { 
            
        }


    }

    public class BrainConfig
    {
        public BrainConfig(IRecognizable recognizable)
        {
            Recognizable = recognizable;
        }

        public IRecognizable Recognizable { get; private set; }
    }

    public interface IRecognizable
    {
        IEnumerable<ISensible> GetSensibles();

    }
}