using System;
using UnityEngine;
using UCamera = UnityEngine.Camera;

using APP.Draw;
using APP.Input;

namespace APP
{
    public class SessionDraw: ModelLoadable, IUpdateble
    {
        [SerializeField] private Transform m_Scene;
        [SerializeField] private UCamera m_CameraMain;
        
        private InputController m_InputController;

        private PencilController m_PencilController;
        private IPencil m_Pencil;
        
        private PictureController m_PictureController;
        private IPicture m_Picture;
        private int m_PictureWidht;
        private int m_PictureHeight;
        private int m_PicturLayerMask;

        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        private Color m_DrawColor = Color.green;

        [SerializeField] private UpdateController m_UpdateController;
        [SerializeField] private AsyncController m_AsyncController;


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
            m_InputController = InputController.Get(new InputControllerConfig(m_CameraMain));
            m_InputController.Init();
            
            m_PictureWidht = 10;
            m_PictureHeight = 10;
            m_PicturLayerMask = 8;
            
            m_Picture = Picture3D.Get();
            var pictureConfig = new PictureConfig(m_Picture, m_PictureWidht, m_PictureHeight, m_BackgroundColor, m_HoverColor, m_PicturLayerMask, m_Scene);
            m_PictureController = PictureController.Get(new PictureControllerConfig(m_Picture));
            m_PictureController.Init();
            
            
            m_Pencil = PencilModel.Get();
            var pencilConfig = new PencilConfig(m_Pencil, m_DrawColor, m_BackgroundColor);
            
            m_PencilController = PencilController.Get(new PencilControllerConfig(m_Pencil));
            m_PencilController.Init();
           
            
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
        }
    }
}