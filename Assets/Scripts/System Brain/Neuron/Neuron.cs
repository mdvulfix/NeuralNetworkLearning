using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    public abstract class NeuronModel : AConfigurableOnAwake, IConfigurable, IUpdateble
    {
        private INeuron m_Neuron;

        private bool m_IsGrowing = false;
        private bool m_IsMoving = false;

        [SerializeField] private float m_Size;
        [Range(0, 2)] private float m_SizeChangeRate = 1;
        private float m_SizeDefault = 50;
        private float m_SizeDivide = 100;

        [SerializeField] private float m_Force;
        [Range(0, 2)] private float m_ForceChangeRate = 1;

        [SerializeField] private float m_Energy;
        private float m_EnergyDefault = 50;
        private float m_EnergyMax = 100;

        [SerializeField] private float m_Speed;
        [Range(0, 2)] private float m_SpeedChangeRate = 1;
        private float m_SpeedDefault = 1;

        [SerializeField] private Vector3 m_Direction;

        private float m_NeuronSizeDefault = 1f;
        private float m_DendriteWidtDefault = 0.5f;

        private int m_DendriteNumbre = 1;

        [SerializeField] private Axon m_Axon;

        [SerializeField] private List<Dendrite> m_Dendrites;

        private Transform m_Transform;

        private SphereCollider m_Collider;
        private MeshRenderer m_Renderer;
        private Rigidbody m_Rigidbody;

        
        private Color m_ColorDefault = Color.white;
        private Color m_ColorHover = Color.green;



        public float Size { get => m_Size; private set => m_Size = value; }
        public float Energy { get => m_Energy; private set => m_Energy = value; }
        public Vector3 Position { get => m_Transform.position; private set => m_Transform.position = value; }


        public event Action<INeuron> Divided;
        public event Action<INeuron> Dead;

        public override void Configure(params object[] args)
        {
            var config = (NeuronConfig)args[PARAM_INDEX_Config];

            m_Neuron = config.Neuron;

            Size = config.Size;
            Energy = config.Energy;
            Position = config.Position;

            m_Transform = SceneObject.transform;


            if (SceneObject.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
                m_Renderer = SceneObject.AddComponent<MeshRenderer>();
            //m_Renderer.sprite = HandlerSprite.Circle;
            

            if (SceneObject.TryGetComponent<SphereCollider>(out m_Collider) == false)
                m_Collider = SceneObject.AddComponent<SphereCollider>();
            //m_Collider.radius = m_NeuronSizeDefault / 2;
            //m_Collider.offset = Vector2.zero;


            if (SceneObject.TryGetComponent<Rigidbody>(out m_Rigidbody) == false)
                m_Rigidbody = SceneObject.AddComponent<Rigidbody>();

            var neuronPosition = transform.position;

            var axonHead = neuronPosition;
            var axonTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
            var axonWidth = m_NeuronSizeDefault / 2;

            m_Axon = NerveModel.Get<Axon>(axonHead, axonTail, axonWidth);

            m_Dendrites = new List<Dendrite>();

            for (int i = 0; i < m_DendriteNumbre; i++)
            {
                var dendriteHead = neuronPosition;
                var dendriteTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
                var dendriteWidth = m_DendriteWidtDefault / m_DendriteNumbre;

                m_Dendrites.Add(NerveModel.Get<Dendrite>(dendriteHead, dendriteTail, dendriteWidth));
            }
        }


        public ISensor GetSensor()
        {
            ISensor sensor = null;
            
            foreach (var dendrite in m_Dendrites)
            {
                if (dendrite.GetSensor(out sensor))
                    return sensor;
            }


            var newDendrite = GrowDendrite();
            newDendrite.GetSensor(out sensor);

            return sensor;
        }


        private void EnergyCalculate()
        {
            if (m_Energy > 0)
                m_Energy -= Time.deltaTime;

            if (m_Energy <= 0)
                m_Energy = 0;

            if (m_Energy >= m_EnergyMax)
                m_IsGrowing = true;
            else
                m_IsGrowing = false;
        }

        private void SizeCalculate()
        {
            if (m_IsGrowing == true)
            {
                if (m_SizeChangeRate < 2)
                    m_SizeChangeRate += Time.deltaTime;
                else
                    m_SizeChangeRate = 2;
            }
            else
            {
                if (m_SizeChangeRate > 1)
                    m_SizeChangeRate -= Time.deltaTime;
                else
                    m_SizeChangeRate = 1;
            }

            m_Size += m_Size * m_SizeChangeRate;

            if (m_Size >= m_SizeDivide)
                Divide();
        }

        private void ForceCalculate()
        {
            if (m_IsGrowing == true)
            {
                if (m_ForceChangeRate < 2)
                    m_ForceChangeRate += Time.deltaTime;
                else
                    m_ForceChangeRate = 2;
            }
            else
            {
                if (m_ForceChangeRate > 0)
                    m_ForceChangeRate -= Time.deltaTime;
                else
                    m_ForceChangeRate = 0;
            }

            m_Force = m_Energy * m_ForceChangeRate;
        }

        private void MoveCalculate()
        {
            m_Rigidbody.velocity = (m_Direction * m_Speed) * Time.deltaTime;
        }


        private void Divide()
        {
            m_Size /= 2;
            m_Energy /= 2;

            Divided?.Invoke(m_Neuron);
        }

        private void Die()
        {
            Dead?.Invoke(m_Neuron);

        }

        private IDendrite GrowDendrite()
        {

            var dendriteHead = neuronPosition;
            var dendriteTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
            var dendriteWidth = m_DendriteWidtDefault / m_DendriteNumbre;

            m_Dendrites.Add(Dendrite.Get(dendriteHead, dendriteTail, dendriteWidth));

        }



        public void Update()
        {
            EnergyCalculate();
            SizeCalculate();
            ForceCalculate();
            MoveCalculate();

        }


        public static TNeuron Get<TNeuron>(params object[] args)
        where TNeuron : INeuron, new() //Component, IBrain
        {
            //var obj = new GameObject("Brain");
            //obj.SetActive(false);

            //var renderer = obj.AddComponent<MeshRenderer>();           
            //var instance = obj.AddComponent<TBrain>();
            var instance = new TNeuron();

            if (args.Length > 0)
            {
                instance.Configure(args);
                instance.Init();
            }

            return instance;
        }
    }

    public class NeuronDefault : NeuronModel, INeuron
    {
        public NeuronDefault() { }
        public NeuronDefault(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {


            base.Configure(args);
        }


    }


    public interface INeuron : IConfigurable
    {

    }

    public struct NeuronConfig : IConfig
    {
        public NeuronConfig(INeuron neuron, float size, float energy, Vector3 position)
        {

            Neuron = neuron;
            Size = size;
            Energy = energy;
            Position = position;
        }

        public INeuron Neuron { get; private set; }
        public float Size { get; private set; }
        public float Energy { get; private set; }
        public Vector3 Position { get; private set; }
    }

    public struct NeuronInfo
    {
        public NeuronInfo(INeuron neuron, float size, float energy)
        {
            Neuron = neuron;
        }

        public INeuron Neuron { get; private set; }
    }
}