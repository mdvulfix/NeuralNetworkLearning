using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public abstract class NeuronModel : ModelCacheable
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

        [SerializeField] private float m_MoveSpeed = 0.1f;
        [Range(0, 2)] private float m_MoveSpeedChangeRate = 1;
        private float m_MoveSpeedDefault = 0.1f;
        [SerializeField] private float m_MoveDistanceLimitDefault = 10;
        [SerializeField] private float m_MoveDistanceLimit;
        [SerializeField] private float m_MoveDistance;
        private Vector3 m_PositionOnStart;

        [SerializeField] private Vector3 m_Direction;
        [SerializeField] private Vector3 m_DirectionPrevious;
        [SerializeField] private float m_DirectionCooldawnDefault = 5;
        [SerializeField] private float m_DirectionCooldawn;
        [SerializeField] private float m_DirectionChangeDurationDefault = 3;
        [SerializeField] private float m_DirectionChangeDuration;
        private float m_DirectionChangeElapsedTime;


        private float m_NeuronSizeDefault = 1f;
        private float m_DendriteWidtDefault = 0.5f;

        [SerializeField] private IAxon m_Axon;
        [SerializeField] private List<IDendrite> m_Dendrites;


        private Color m_ColorDefault = Color.white;
        private Color m_ColorHover = Color.green;
        

        public INeuron Neuron { get; private set; }
        public Vector3 Position { get => transform.position; private set => transform.position = value; }
        public float Size { get => m_Size; private set => m_Size = value; }
        public float Energy { get => m_Energy; private set => m_Energy = value; }
        public int LayerMask { get => gameObject.layer; private set => gameObject.layer = value; }

        public Transform Parent { get => transform.parent; private set { if (value != null) transform.SetParent(value); } }


        public event Action<INeuron> Divided;
        public event Action<INeuron> Dead;

        public static readonly string PREFAB_Folder ="Prefab";

        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (NeuronConfig)args[PARAMS_Config] :
            default(NeuronConfig);

            Neuron = m_Config.Instance;
            Energy = m_Config.Energy;
            Size = m_Config.Size;
            Position = m_Config.Position;
            LayerMask = m_Config.LayerMask;
            Parent = m_Config.Parent;

            //m_Collider.offset = Vector2.zero;

            //if (SceneObject.TryGetComponent<Rigidbody>(out m_Rigidbody) == false)
            //   m_Rigidbody = SceneObject.AddComponent<Rigidbody>();
            //m_Rigidbody.mass = 0f;
            //m_Rigidbody.useGravity = false;


            base.Configure(args);

        }

        public override void Init()
        {
            if (VerifyOnInit())
                return;

            m_PositionOnStart = Position;
            m_Direction = HandlerVector.GetRandomVector(-2, 2);
            m_DirectionPrevious = m_Direction;

            m_Dendrites = new List<IDendrite>();

            var parent = GetComponent<Transform>();

            var dendritePosition = new Vector3(Position.x - 1, Position.y, Position.z - 1);
            var dendrite = Grow<DendriteDefault>(dendritePosition, Size, parent);
            m_Dendrites.Add(dendrite);


            var axonPosition = new Vector3(Position.x + 1, Position.y, Position.z + 1);
            m_Axon = Grow<AxonDefault>(axonPosition, Size, parent);
            Debug.DrawLine(Position, axonPosition, Color.yellow);


            base.Init();
        }

        


        public override void Dispose()
        {
            m_Axon.Dispose();

            foreach (var dendrite in m_Dendrites)
                dendrite.Dispose();


            m_Dendrites.Clear();
            base.Dispose();
        }


        public override void Activate()
        {
            if (VerifyOnActivate())
                return;

            m_Axon.Activate();

            foreach (var dendrite in m_Dendrites)
                dendrite.Activate();

            base.Activate();
        }

        public override void Deactivate()
        {
            m_Axon.Deactivate();

            foreach (var dendrite in m_Dendrites)
                dendrite.Deactivate();

            base.Deactivate();
        }






        public ISensor GetSensor()
        {
            ISensor sensor = null;

            foreach (var dendrite in m_Dendrites)
            {
                if (dendrite.GetSensor(out sensor))
                    return sensor;
            }

            var parent = GetComponent<Transform>();
            var newDendrite = Grow<DendriteDefault>(Position, Size, parent);
            newDendrite.GetSensor(out sensor);

            return sensor;
        }

        protected TNerve Grow<TNerve>(Vector3 position, float size, Transform parent)
        where TNerve : INerve
        {
            var nerve = NerveModel.Get<TNerve>();
            var nerveConfig = new NerveConfig(nerve, position, size, LayerMask, parent);

            nerve.Configure(nerveConfig);
            nerve.Init();

            return nerve;
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

            Size += Size * m_SizeChangeRate;

            if (Size >= m_SizeDivide)
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

        private void CalculateMove()
        {
            m_DirectionCooldawn -= Time.deltaTime;

            if (m_DirectionCooldawn <= 0)
            {
                m_DirectionChangeElapsedTime = 0;
                m_DirectionChangeDuration = m_DirectionChangeDurationDefault;
                m_DirectionPrevious = m_Direction;
                m_DirectionCooldawn = m_DirectionCooldawnDefault;
                m_Direction = HandlerVector.GetRandomVector(-2, 2);
            }

            m_DirectionChangeElapsedTime += Time.deltaTime;
            var percentageComplite = m_DirectionChangeElapsedTime / m_DirectionChangeDuration;
            Position += Vector3.Lerp(m_DirectionPrevious, m_Direction, Mathf.SmoothStep(0, 1, percentageComplite)) * m_MoveSpeed * Time.deltaTime;
        }

        private void Divide()
        {

            Size /= 2;
            Energy /= 2;

            Divided?.Invoke(Neuron);
        }

        private void Die()
        {
            Dead?.Invoke(Neuron);

        }




        public virtual void Update()
        {
            //Debug.DrawLine(Position, m_Axon.Position, Color.yellow);
            UpdateBond(Color.yellow, Position, m_Axon.Position);

            foreach (var dendrite in m_Dendrites)
                dendrite.UpdateBond(Color.yellow, dendrite.Position, Position);


            //EnergyCalculate();
            //SizeCalculate();
            //ForceCalculate();
            CalculateMove();



        }

        protected abstract void UpdateBond(Color color, params Vector3[] positions);

        // FACTORY //
        public static TNeuron Get<TNeuron>(params object[] args)
        where TNeuron : INeuron
        {
            IFactory factoryCustom = null;

            if (args.Length > 0)
                try { factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }


            var factory = (factoryCustom != null) ? factoryCustom : new NeuronFactory();
            var instance = factory.Get<TNeuron>(args);

            return instance;
        }

    }


    public interface INeuron : IConfigurable, IActivable, IUpdateble
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

            //NerveConfig = new NerveConfig(Instance, Position, Size, LayerMask, Parent);
        }

        public INeuron Instance { get; private set; }
        public float Size { get; private set; }
        public float Energy { get; private set; }
        public Vector3 Position { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }

        //public NerveConfig NerveConfig { get; private set; }

    }

    public struct NeuronInfo
    {
        public NeuronInfo(INeuron neuron, float size, float energy)
        {
            Neuron = neuron;
        }

        public INeuron Neuron { get; private set; }
    }




    public partial class NeuronFactory : Factory<INeuron>
    {
        public NeuronFactory()
        {
            Set<NeuronInput>(Constructor.Get((args) => GetNeuronInput(args)));
            Set<NeuronAnalyzer>(Constructor.Get((args) => GetNeuronAnalyzer(args)));

        }
    }

}