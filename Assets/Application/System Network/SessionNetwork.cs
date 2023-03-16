using System;
using UnityEngine;
using UCamera = UnityEngine.Camera;

using APP.Input;

namespace APP.Network
{
    public class SessionNetwork: ModelLoadable, IUpdateble
    {
        [SerializeField] private Transform m_Scene;
        [SerializeField] private UCamera m_CameraMain;
        
        private InputController m_InputController;

        [SerializeField] private UpdateController m_UpdateController;
        [SerializeField] private AsyncController m_AsyncController;
        
        
        
        
        private NetController m_NetController;

        public override void Load()
        {
            var config =  new SessionConfig();
            Configure(config);

            base.Load();
        }
        
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            var config = args.Length > 0 ? 
            (SessionConfig)args[PARAMS_Config] : 
            default(SessionConfig);

            if(m_CameraMain == null)
            {
                Send($"{ m_CameraMain.GetName()} is not set!", LogFormat.Warning);
                return;
            }
            
            if(m_Scene == null)
            {
                Send($"{ m_Scene.GetName()} is not set!", LogFormat.Warning);
                return;
            }
            
            base.Configure(args);
        }

        public override void Init()
        {
            m_InputController = InputController.Get();
            var inputControllerConfig = new InputControllerConfig(m_CameraMain);
            m_InputController.Configure(inputControllerConfig);
            m_InputController.Init();
            
            var inputLayerSize = 4;
            var analyzeLayerSize = 2;
            var analyzeLayerNumber = 2;
            var colorBackground= Color.grey;
            var colorActive = Color.green;
            var layerMask = 9;
            var net = Net2D.Get();
            var netConfig = new NetConfig(net, inputLayerSize, analyzeLayerSize, analyzeLayerNumber, colorBackground, colorActive, layerMask, m_Scene);
            net.Configure(netConfig);


            m_NetController = NetController.Get();
            var netControllerConfig = new NetControllerConfig(net);
            m_NetController.Configure(netControllerConfig);
            m_NetController.Init();

            base.Init();
        }


        public override void Dispose()
        {
            m_NetController.Dispose();
            m_InputController.Dispose();
            base.Dispose();
        }





        public void Update()
        {
            m_InputController.Update();
        }
    }
}