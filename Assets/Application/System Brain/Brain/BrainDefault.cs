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

            var inputLayerSize = 4;
            var analyzeLayerSize = 2;
            var analyzeLayerNumber = 2;
            var layerMask = 9;
            var brainConfig = new BrainConfig(this, inputLayerSize, analyzeLayerSize, analyzeLayerNumber, layerMask, parent);
            base.Configure(brainConfig);
            Send($"{this.GetName()} was configured by default!");

        }


        public override void Recognize(IRecognizable recognizable)
        {
            var sensibles = recognizable.GetSensibles();
        }

        public override void Analyze()
        {

        }

        public override void Response()
        {
            
        }

        
        protected override INeuron[,,] CreateInputLayer()
            => CreateLayer<NeuronInput>(InputLayerDimension);

        protected override INeuron[,,] CreateAnalyzeLayer() 
            => CreateLayer<NeuronAnalyzer>(AnalyzeLayerDimension, AnalyzeLayerNumber);
   
    }

    public partial class BrainFactory : Factory<IBrain>
    {
        public partial void Setup() 
            => Set<BrainDefault>(Constructor.Get((args) => GetBrainDefault(args)));


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