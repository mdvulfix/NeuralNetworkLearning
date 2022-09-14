using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Brain
{

    public class BrainDefault : BrainModel, IBrain
    {
        public static readonly string PREFAB_Label = "Brain";

        public BrainDefault() { }
        public BrainDefault(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;


            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }


            // CONFIGURE BY DEFAULT //
            Transform parent = null;
            if (Seacher.Find<IScene>(out var scenes))
                parent = scenes[0].Scene;

            var layerMask = 9;
            var brainConfig = new BrainConfig(this, layerMask, parent);
            base.Configure(brainConfig);
            Send($"{this.GetName()} was configured by default!");

        }

        protected override INeuron GetNeuron(Vector3 position)
        {
            var neuron = NeuronModel.Get();
            
            var size = 0.5f;
            var energy = 50;
            var parent = GetTransform();

            var neuronConfig = new NeuronConfig(neuron, position, size, energy, LayerMask, parent);
            neuron.Configure(neuronConfig);
            neuron.Init();
            neuron.Activate();

            return neuron;
        }
    }

    public partial class BrainFactory : Factory<IBrain>
    {       
        private BrainDefault GetBrainDefault(params object[] args)
        {
            
            var prefabPath = $"{BrainModel.PREFAB_Folder}/{BrainDefault.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Brain");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<BrainDefault>();
            
            
            //var instance = new Picture3D();

            if(args.Length > 0)
            {
                var config = (BrainConfig)args[BrainModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }

    }
}