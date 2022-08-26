using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Neuron : MonoBehaviour, IConfigurable<NeuronConfig>
    {
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

        private GameObject m_GameObject;
        private Transform m_Transform;
        private SphereCollider m_Collider;
        private MeshRenderer m_Renderer;
        private Rigidbody m_Rigidbody;

        private Color m_ColorDefault = Color.white;
        private Color m_ColorHover = Color.green;
        
        public NeuronConfig Config {get; private set; }

        public float Size { get => m_Size; private set => m_Size = value; }
        public float Energy  { get => m_Energy; private set => m_Energy = value; }
        public Vector3 Position { get => m_Transform.position; private set => m_Transform.position = value; }


        public event Action<Neuron> Divided;
        public event Action<Neuron> Dead;


        
        public virtual void Setup(NeuronConfig config)
        {
            Config = config;
            Size = config.Size;
            Energy = config.Energy;
            Position = config.Position;
        
        }
        
        public virtual void Configure(params object[] args)
        {
       
            m_GameObject = gameObject;

            if (m_GameObject.TryGetComponent<Transform>(out m_Transform) == false)
                m_Transform = m_GameObject.AddComponent<Transform>();

            if (m_GameObject.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
            {
                m_Renderer = m_GameObject.AddComponent<MeshRenderer>();
                //m_Renderer.sprite = HandlerSprite.Circle;
            }

            if (m_GameObject.TryGetComponent<SphereCollider>(out m_Collider) == false)
            {
                m_Collider = m_GameObject.AddComponent<SphereCollider>();
                //m_Collider.radius = m_NeuronSizeDefault / 2;
                //m_Collider.offset = Vector2.zero;
            }

            if (m_GameObject.TryGetComponent<Rigidbody>(out m_Rigidbody) == false)
                m_Rigidbody = m_GameObject.AddComponent<Rigidbody>();

            var neuronPosition = transform.position;

            var axonHead = neuronPosition;
            var axonTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
            var axonWidth = m_NeuronSizeDefault / 2;

            m_Axon = Axon.Get(axonHead, axonTail, axonWidth);

            m_Dendrites = new List<Dendrite>();

            for (int i = 0; i < m_DendriteNumbre; i++)
            {
                var dendriteHead = neuronPosition;
                var dendriteTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
                var dendriteWidth = m_DendriteWidtDefault / m_DendriteNumbre;

                m_Dendrites.Add(Dendrite.Get(dendriteHead, dendriteTail, dendriteWidth));

            }
        }

        public virtual void Init() { }
        public virtual void Dispose() { }

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

            Divided?.Invoke(this);
        }

        private void Die()
        {
            Dead?.Invoke(this);

        }



        private void Awake() =>
            Configure();

        private void OnEnable() =>
            Init();

        private void OnDisable() =>
            Dispose();

        private void Update()
        {
            EnergyCalculate();
            SizeCalculate();
            ForceCalculate();
            MoveCalculate();

        }


        public static Neuron Get()
        {
            var obj = new GameObject("Neuron");
            return obj.AddComponent<Neuron>();
        }
    }

    public struct NeuronConfig: IConfig
    {
        public NeuronConfig(float size, float energy, Vector3 position)
        {
            Size = size;
            Energy = energy;
            Position = position;
        }

        public float Size { get; private set; }
        public float Energy { get; private set; }
        public Vector3 Position { get; private set; }
    }

    public struct NeuronInfo
    {
        public NeuronInfo(Neuron neuron, float size, float energy)
        {
            Neuron = neuron;
        }

        public Neuron Neuron { get; private set; }
    }
}