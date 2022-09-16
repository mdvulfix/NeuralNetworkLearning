using System;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace APP.Brain
{

    public class NeuronAnalyzer : NeuronModel, INeuron
    {
        private MeshRenderer m_MeshRenderer;
        private LineRenderer m_LineRenderer;
        private SphereCollider m_Collider;
        
        public static readonly string PREFAB_Label = "NeuronAnalyzer";

        public NeuronAnalyzer() { }
        public NeuronAnalyzer(params object[] args)
            => Configure(args);


        public void Load()
        { 
            // CONFIGURE BY DEFAULT //
            var position = Vector3.zero;
            var size = URandom.Range(0f, 100f);
            var energy = URandom.Range(0f, 100f);
            var layerMask = 9;
            
            Transform parent = null;
            if (Seacher.Find<IScene>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var neuronParent))
                    parent = neuronParent != null ? neuronParent : transform.parent;
    

            var config = new NeuronConfig(this, position, size, energy, layerMask, parent);

            base.Configure(config);
            Send($"The instance was configured on load!");
                        
        }

        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;


            if (GetComponent<MeshRenderer>(out m_MeshRenderer) == false)
                m_MeshRenderer = SetComponent<MeshRenderer>();
            
            if (GetComponent<LineRenderer>(out m_LineRenderer) == false)
                m_LineRenderer = SetComponent<LineRenderer>();

            if (GetComponent<SphereCollider>(out m_Collider) == false)
                m_Collider = SetComponent<SphereCollider>();


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
            if (Seacher.Find<IScene>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var neuronParent))
                    parent = neuronParent != null ? neuronParent : transform.parent;

            var config = new NeuronConfig(this, position, size, energy, layerMask, parent);

            base.Configure(config);
            Send($"The instance was configured by default!");
        }

        protected override void UpdateBond(Color color, params Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
                m_LineRenderer.SetPosition(i, points[i]);
 
        }

        //protected override void Impulse() { }
        
        // FACTORY //
        public static NeuronAnalyzer Get(params object[] args)
            => Get<NeuronAnalyzer>(args);
    }

    public partial class NeuronFactory : Factory<INeuron>
    {
        private NeuronAnalyzer GetNeuronAnalyzer(params object[] args)
        {       
            var prefabPath = $"{NeuronModel.PREFAB_Folder}/{NeuronInput.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Neuron");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<NeuronAnalyzer>();
            obj.name = $"Analyzer { instance.GetHashCode() } ";

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