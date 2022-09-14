using System;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace APP.Brain
{

    public class NeuronDefault : NeuronModel, INeuron
    {
        private MeshRenderer m_Renderer;
        private SphereCollider m_Collider;
        
        public static readonly string PREFAB_Label = "Neuron";

        public NeuronDefault() { }
        public NeuronDefault(params object[] args)
            => Configure(args);


        public override void Load()
        { 
            if (VerifyOnLoad())
                return;
            
            Configure();
            Init();
            Activate();

            base.Load();
        }

        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;


            var obj = gameObject;

            if (obj.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<MeshRenderer>();
            
    
            if (obj.TryGetComponent<SphereCollider>(out m_Collider) == false)
                m_Collider = obj.AddComponent<SphereCollider>();

            m_Collider.isTrigger = true;
            


            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }
            
            // CONFIGURE BY DEFAULT //
            var position = Vector3.zero;
            var size = URandom.Range(0f, 100f);
            var energy = URandom.Range(0f, 100f);
            var layerMask = 9;
            
            Transform parent = null;
            if (Seacher.Find<IBrain>(out var brains))
            {
                var neuronParent = brains[0].GetTransform();
                parent = neuronParent != null ? neuronParent : transform.parent;
            }

            var config = new NeuronConfig(this, position, size, energy, layerMask, parent);

            base.Configure(config);
            Send($"{this.GetName()} was configured by default!");
        }

        protected override void Impulse() { }

    }

    public partial class NerveFactory : Factory<INerve>
    {
        private NeuronDefault GetNeuronDefault(params object[] args)
        {       
            var prefabPath = $"{NerveModel.PREFAB_Folder}/{NeuronDefault.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Neuron");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<NeuronDefault>();
            obj.name = $"Neuron { instance.GetHashCode() } ";

            //var instance = new Pixel3D();

            if (args.Length > 0)
            {
                var config = (NeuronConfig)args[NeuronModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }

    }
}