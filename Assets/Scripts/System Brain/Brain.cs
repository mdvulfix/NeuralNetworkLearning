using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    public class Brain : MonoBehaviour, IConfigurable
    {

        private List<Neuron> m_Neurons;
        private NeuronController m_NeuronController;

        private Neuron[,,] m_Matrix;
        private Vector3Int m_MatrixSize;
        private int m_MatrixDimension = 8;



        public virtual void Configure(params object[] args)
        {
            m_Neurons = new List<Neuron>();
            m_MatrixSize = new Vector3Int(m_MatrixDimension, m_MatrixDimension, m_MatrixDimension);
            m_Matrix = new Neuron[m_MatrixSize.x, m_MatrixSize.y, m_MatrixSize.z];
        }   

        public virtual void Init()
        {
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
}