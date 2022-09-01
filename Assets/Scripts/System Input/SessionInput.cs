using UnityEngine;
using UCamera = UnityEngine.Camera;

namespace APP.Input
{
    public class SessionInput: AConfigurableOnAwake, IConfigurable, IUpdateble
    {

        [SerializeField] private UCamera m_CameraMain;
        [SerializeField] private InputController m_InputController;

        public override void Configure(params object[] args)
        {
            var config = args.Length > 0 ? 
            (SessionConfig)args[PARAM_INDEX_Config] : 
            default(SessionConfig);

            
            if(m_CameraMain == null)
            {
                Send($"{ m_CameraMain.GetName()} is not set!", LogFormat.Warning);
                return;
            }
            
            base.Configure(args);
        }

        public override void Init()
        {
            
            var inputControllerConfig = new InputControllerConfig(m_CameraMain);
            m_InputController = new InputController(inputControllerConfig);
            
            base.Init();
        }

        public void Update()
        {
            m_InputController.Update();
        }
    }
}
