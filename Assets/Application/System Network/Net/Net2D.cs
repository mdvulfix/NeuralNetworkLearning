using UnityEngine;

namespace APP.Network
{
    
    [RequireComponent(typeof(Canvas))]
    public class Net2D : NetModel, INet
    {
        
        private Camera m_Camera;
        private Canvas m_Canvas;
        
        
        public static readonly string PREFAB_Label = "Net2D";

        public Net2D() { }
        public Net2D(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            var obj = gameObject;

            if (obj.TryGetComponent<Canvas>(out m_Canvas) == false)
                m_Canvas = obj.AddComponent<Canvas>();
            
            
            if (args.Length > 0)
            {
                var config = (NetConfig)args[PARAMS_Config];
                
                m_Canvas.worldCamera = config.Camera;


                base.Configure(args);
                return;
            }

        #region CONFIGURE BY DEFAULT

            Camera netCamera = null;
            if (Seacher.Find<Camera>(out var cameras))
                netCamera = cameras[0];
            
            var layerRecognizeDimension = 5;
            var layerRecognizeSize = 1;
            var layerAnalyzeDimension = 3;
            var layerAnalyzeSize = 3;
            var layerResponseDimension = 5;
            var layerResponseSize = 1;
            var colorBackground = Color.grey;
            var colorActive = Color.green;
            var layerMask = 9;
            var netConfig = new NetConfig(this,
                                          netCamera,
                                          layerRecognizeDimension,
                                          layerRecognizeSize,
                                          layerAnalyzeDimension,
                                          layerAnalyzeSize,
                                          layerResponseDimension,
                                          layerResponseSize,
                                          colorBackground,
                                          colorActive,
                                          layerMask,
                                          null);

            base.Configure(netConfig);
            Send($"{this.GetName()} was configured by default!");

        #endregion

        }


        public override void Init()
        {

            
            base.Init();
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


        protected override INode[,] CreateLayerRecognize()
            => CreateLayer<Node2D>(LayerRecognizeDimension, LayerRecognizeSize);

        protected override INode[,] CreateLayerAnalyze()
            => CreateLayer<Node2D>(LayerAnalyzeDimension, LayerAnalyzeSize);

        protected override INode[,] CreateLayerResponse()
            => CreateLayer<Node2D>(LayerResponseDimension, LayerResponseSize);




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
            new GameObject("Net2D");

            obj.SetActive(false);

            var instance = obj.AddComponent<Net2D>();

            //var instance = new Picture2D();

            if (args.Length > 0)
            {
                var config = (NetConfig)args[NetModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }

    }
}