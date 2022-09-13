using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(SphereCollider))]
    //[RequireComponent(typeof(Rigidbody))]
    public abstract class NeuronModel : NerveModel  
    {
        private NeuronConfig m_Config;
        
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

        [SerializeField] private IAxon m_Axon;
        [SerializeField] private List<IDendrite> m_Dendrites;


        private Color m_ColorDefault = Color.white;
        private Color m_ColorHover = Color.green;
        

        public INeuron Neuron { get; private set; }
        public float Energy { get => m_Energy; private set => m_Energy = value; }

        
        public event Action<INeuron> Divided;
        public event Action<INeuron> Dead;

        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (NeuronConfig)args[PARAMS_Config] :
            default(NeuronConfig);
        
            Neuron = m_Config.Instance;
            Energy = m_Config.Energy;

            
            //m_Collider.offset = Vector2.zero;

            //if (SceneObject.TryGetComponent<Rigidbody>(out m_Rigidbody) == false)
            //   m_Rigidbody = SceneObject.AddComponent<Rigidbody>();
            //m_Rigidbody.mass = 0f;
            //m_Rigidbody.useGravity = false;


            base.Configure(m_Config.NerveConfig);

        }

        public override void Init()
        {
            var parent = GetTransform();
            
            m_Axon = Sprout<AxonDefault>(Position, Size, parent);

            m_Dendrites = new List<IDendrite>();
            m_Dendrites.Add(Sprout<DendriteDefault>(Position, Size, parent));

            base.Init();
        }


        public ISensor GetSensor()
        {
            ISensor sensor = null;

            foreach (var dendrite in m_Dendrites)
            {
                if (dendrite.GetSensor(out sensor))
                    return sensor;
            }

            var parent = GetTransform();
            var newDendrite = Sprout<DendriteDefault>(Position, Size, parent);
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
            //m_Rigidbody.velocity = (m_Direction * m_Speed) * Time.deltaTime;
        }


        private void Divide()
        {
            m_Size /= 2;
            m_Energy /= 2;

            Divided?.Invoke(Neuron);
        }

        private void Die()
        {
            Dead?.Invoke(Neuron);

        }




        public void Update()
        {
            EnergyCalculate();
            SizeCalculate();
            ForceCalculate();
            MoveCalculate();

        }

        // FACTORY //
        public static NeuronDefault Get(params object[] args)
            => Get<NeuronDefault>(args);
        

    }


    public interface INeuron : INerve
    {
        event Action<INeuron> Divided;
        event Action<INeuron> Dead;

        ISensor GetSensor();
   

    }

    public struct NeuronConfig : IConfig
    {
        public NeuronConfig(INeuron instance, Vector3 position, float size, float energy, int layerMask, Transform parent)
        {
            Instance = instance;
            Size = size;
            Energy = energy;
            Position = position;
            LayerMask = layerMask;
            Parent = parent;
        
            NerveConfig = new NerveConfig(Instance, Position, Size, LayerMask, Instance.GetTransform());
        }

        public INeuron Instance { get; private set; }
        public float Size { get; private set; }
        public float Energy { get; private set; }
        public Vector3 Position { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }
        
        public NerveConfig NerveConfig { get; private set; }
    
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