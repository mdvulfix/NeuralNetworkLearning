using UnityEngine;
using UCamera = UnityEngine.Camera;

using APP.Input;

namespace APP.Draw
{
    public class SessionDraw: AConfigurableOnAwake, IConfigurable, IUpdateble
    {

        [SerializeField] private Transform m_Scene;

        [SerializeField] private UCamera m_CameraMain;
        [SerializeField] private InputController m_InputController;

        [SerializeField] private PencilController m_PencilController;
        [SerializeField] private PictureController m_PictureController;
        
        [SerializeField] private UpdateController m_UpdateController;
        [SerializeField] private AsyncController m_AsyncController;
        
        
        
        
        
        
        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        private Color m_DrawColor = Color.green;

        public Transform Root => m_Scene;

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
            
            if(m_Scene == null)
            {
                Send($"{ m_Scene.GetName()} is not set!", LogFormat.Warning);
                return;
            }
            
            
            
            base.Configure(args);
        }

        public override void Init()
        {
            
            var inputControllerConfig = new InputControllerConfig(m_CameraMain);
            m_InputController = new InputController(inputControllerConfig);
            
            var pictureControllerConfig = new PictureControllerConfig(m_BackgroundColor, m_HoverColor, m_Scene);
            m_PictureController = new PictureController(pictureControllerConfig);
            
            var pencilControllerConfig = new PencilControllerConfig(m_BackgroundColor, m_DrawColor, m_PictureController);
            m_PencilController = new PencilController(pencilControllerConfig);
           
            
            base.Init();
        }


        public override void Dispose()
        {
            m_PencilController.Dispose();
            m_PictureController.Dispose();
            m_InputController.Dispose();
        
            base.Dispose();
        }





        public void Update()
        {
            m_InputController.Update();
            m_PencilController.Update();
        }
    }
}