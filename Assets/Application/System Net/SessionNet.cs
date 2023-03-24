using System;
using UnityEngine;


using APP.Input;

namespace APP.Net
{
    public class SessionNetwork : ModelLoadable, IUpdateble
    {
        [SerializeField] private Transform m_Scene;
        [SerializeField] private Transform m_Camera;
        [SerializeField] private Transform m_Light;
        [SerializeField] private Transform m_UI;

        private InputController m_InputController;

        [SerializeField] private UpdateController m_UpdateController;
        [SerializeField] private AsyncController m_AsyncController;


        private NetController m_NetController;

        public override void Load()
        {
            var config = new SessionConfig();
            Configure(config);

            base.Load();
        }

        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            var config = args.Length > 0 ?
            (SessionConfig)args[PARAMS_Config] :
            default(SessionConfig);

            if (m_Scene == null)
            {
                Send($"Scene is not set!", LogFormat.Warning);
                return;
            }

            if (m_Camera == null)
            {
                Send($"Camera is not set!", LogFormat.Warning);
                return;
            }
            
            if (m_Light == null)
            {
                Send($"Light is not set!", LogFormat.Warning);
                return;
            }
            
            if (m_UI == null)
            {
                Send("UI is not set!", LogFormat.Warning);
                return;
            }
            
            base.Configure(args);
        }

        public override void Init()
        {
            m_InputController = InputController.Get();
            
            if(m_Camera.TryGetComponent<Camera>(out var camera) == false)
            {
                Send($"Camera component is not set! Initialization failed!", LogFormat.Error);
                return;
            }
            
            var inputControllerConfig = new InputControllerConfig(camera);
            m_InputController.Configure(inputControllerConfig);
            m_InputController.Init();

            var layerRecognizeDimension = 10;
            var layerRecognizeSize = 2;
            var layerAnalyzeDimension = 4;
            var layerAnalyzeSize = 3;
            var layerResponseDimension = 10;
            var layerResponseSize = 1;

            var colorBackground = Color.grey;
            var colorActive = Color.green;
            var layerMask = 9;
            var net = Net2D.Get();
            var netConfig = new NetConfig(net,
                                          camera,
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
            net.Configure(netConfig);
            net.Init();


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


        public override void Activate()
        {
            base.Activate();
            m_NetController.Activate();
        }

        public override void Deactivate()
        {
            m_NetController.Deactivate();
            base.Deactivate();
        }

        public void Update()
        {
            m_InputController.Update();
        }
    }
}