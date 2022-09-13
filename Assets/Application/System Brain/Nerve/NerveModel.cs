using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public abstract class NerveModel: ModelCacheable
    {
        private NerveConfig m_Config;
        
        [SerializeField] private float m_Size;

        private List<Сharge> m_СhargeReceived;
        internal static readonly object PREFAB_Folder;

        public INerve Nerve { get; private set; }
        public Vector3 Position { get => transform.position; private set => transform.position = value; }
        public float Size { get => m_Size; private set => m_Size = value; }
        public int LayerMask {get => gameObject.layer; private set => gameObject.layer = value; }
        public Transform Parent { get => transform.parent; private set { if(value != null) transform.SetParent(value); } }

        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (NerveConfig)args[PARAMS_Config] :
            default(NerveConfig);
        
            Nerve = m_Config.Instance;
            Size = m_Config.Size;
            Position = m_Config.Position;
            LayerMask = m_Config.LayerMask;
            Parent = m_Config.Parent;

            
            
            
            base.Configure(args);
        }

        
        
        

        
        public virtual void CargeCalculate()
        { 
            foreach (var charge in m_СhargeReceived)
            {
                
            }
        }

        protected abstract void Impulse();

        
        
        protected TNerve Sprout<TNerve>(Vector3 position, float size, Transform parent)
        where TNerve: INerve
        {
            var nerve = NerveModel.Get<TNerve>();
            var nerveConfig = new NerveConfig(nerve, position, size, LayerMask, parent);

            nerve.Configure(nerveConfig);
            nerve.Init();
            
            return nerve;
        }
        
        
        
        
        
        
        // FACTORY //
        public static TNerve Get<TNerve>(params object[] args)
        where TNerve: INerve
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new NerveFactory();
            var instance = factory.Get<TNerve>(args);
            
            return instance;
        }

    }

    public interface INerve: IConfigurable, IActivable
    {
        
    }

    public class NerveConfig
    {
        public NerveConfig(INerve instance, Vector3 position, float size, int layerMask, Transform parent)
        {
            Instance = instance;
            Size = size;
            Position = position;
            LayerMask = layerMask;
            Parent = parent;
        }

        public INerve Instance { get; private set; }
        public float Size { get; private set; }
        public Vector3 Position { get; private set; }
        public int LayerMask { get; private set; }
        public Transform Parent { get; private set; }
    }

    public partial class NerveFactory : Factory<INerve>
    {
        public NerveFactory()
        {
            Set<NeuronDefault>(Constructor.Get((args) => GetNeuronDefault(args)));
            Set<AxonDefault>(Constructor.Get((args) => GetAxonDefault(args)));
            Set<DendriteDefault>(Constructor.Get((args) => GetDendriteDefault(args)));
            Set<SensorDefault>(Constructor.Get((args) => GetSensorDefault(args)));
        
        }
    }


}