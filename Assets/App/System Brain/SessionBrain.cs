using UnityEngine;
using UCamera = UnityEngine.Camera;


using APP.Brain;
using APP.Input;
using APP.Draw;

namespace APP
{
    
    public class SessionBrain : ModelLoadable, IUpdateble
    {
        
        [SerializeField] private Transform m_Scene;
        [SerializeField] private UCamera m_CameraMain;
        
        [SerializeField] private int m_InputFieldDimension = 10;
        
        private InputController m_InputController;
        private PencilController m_PencilController;        
        private PictureController m_PictureController;
        private BrainController m_BrainController;

        public override void Load()
        {
            // CONFIGURE ON LOAD //
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
            
            var colorBackground= Color.black;
            var colorHover = Color.grey;
            var colorDraw = Color.green;

            var pictureWidht = m_InputFieldDimension;
            var pictureHeight = m_InputFieldDimension;
            var picturLayerMask = 8;
            var picture = Picture3D.Get();
            var pictureConfig = new PictureConfig(picture, pictureWidht, pictureHeight, colorBackground, colorHover, picturLayerMask, m_Scene);
            picture.Configure(pictureConfig);
            
            
            m_PictureController = PictureController.Get();
            var pictureControllerConfig = new PictureControllerConfig(picture);
            m_PictureController.Configure(pictureControllerConfig);
            m_PictureController.Init();
            
            
            var pencil = PencilModel.Get();
            var pencilConfig = new PencilConfig(pencil, colorDraw, colorBackground);
            pencil.Configure(pencilConfig);
            
            m_PencilController = PencilController.Get();
            var pencilControllerConfig = new PencilControllerConfig(pencil);
            m_PencilController.Configure(pencilControllerConfig);
            m_PencilController.Init();
            
            var nerveLayerMask = 9;
            var brain = BrainModel.Get();

            var inputLayerDimension = m_InputFieldDimension;
            var analyzeLayerDimension = m_InputFieldDimension/2;
            var analyzeLayerNumber = 2;
            
            var brainConfig = new BrainConfig(brain, inputLayerDimension, analyzeLayerDimension, analyzeLayerNumber, nerveLayerMask, m_Scene);
            brain.Configure(brainConfig);
            
            m_BrainController = BrainController.Get();
            var brainControllerConfig = new BrainControllerConfig(brain, picture);
            m_BrainController.Configure(brainControllerConfig);
            m_BrainController.Init();


            base.Init();
        }

        public override void Dispose()
        {
            m_BrainController.Dispose();
            m_PencilController.Dispose();
            m_PictureController.Dispose();
            m_InputController.Dispose();
        
            base.Dispose();
        }



        public void Update()
        {
            
            m_InputController.Update();
            m_BrainController.Update();
        }
    }

    public struct SessionConfig : IConfig
    {

    
    }
}