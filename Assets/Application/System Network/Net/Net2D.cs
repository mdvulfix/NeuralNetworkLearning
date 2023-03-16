using UnityEngine;

namespace APP.Network
{
    public class Net2D : NetModel, INet
    {
        public static readonly string PREFAB_Label = "Net2D";

        public Net2D() { }
        public Net2D(params object[] args)
            => Configure(args);
  

        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            
            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }


            // CONFIGURE BY DEFAULT //
            Transform parent = null;
            if(Seacher.Find<IScene>(out var scenes))
                parent = scenes[0].Scene;

            var inputLayerSize = 4;
            var analyzeLayerSize = 2;
            var analyzeLayerNumber = 2;
            var colorBackground= Color.grey;
            var colorActive = Color.green;
            var layerMask = 9;
            var netConfig = new NetConfig(this, inputLayerSize, analyzeLayerSize, analyzeLayerNumber, colorBackground, colorActive, layerMask, parent);
            base.Configure(netConfig);
            Send($"{ this.GetName() } was configured by default!");
            
        }


        public override void Recognize(IRecognizable recognizable)
        {

        }

        public override void Analyze()
        {

        }

        public override void Response()
        {
            
        }

        
        protected override INode[,,] CreateLayerInput()
            => CreateLayer<Node2D>(InputLayerDimension);

        protected override INode[,,] CreateLayerAnalyze() 
            => CreateLayer<Node2D>(AnalyzeLayerDimension, AnalyzeLayerNumber);
        
        
        public static Net2D Get(params object[] args)
            => Get<Net2D>(args);
    }

    public partial class NetFactory : Factory<INet>
    {
        private Net2D GetNet2D(params object[] args)
        {
            
            var prefabPath = $"{NetModel.PREFAB_Folder}/{Net2D.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Net");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<Net2D>();

            //var instance = new Picture2D();

            if(args.Length > 0)
            {
                var config = (NetConfig)args[NetModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }

    }
}