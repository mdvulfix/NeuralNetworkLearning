using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UCamera = UnityEngine.Camera;

using APP.Brain;
using APP.Draw;
using APP.Input;
using System;

namespace APP
{
    public class SessionDefault : ModelLoadable, IUpdateble
    {
        [SerializeField] private Transform m_Scene;
        [SerializeField] private UCamera m_CameraMain; 
        
        
        [Header("SYSTEM BRAIN")]
        private IBrain m_Brain;
        

        private BrainController m_BrainController;
        
        [Header("SYSTEM DRAW")]
        
        private Color m_ColorBackground = Color.black;
        private Color m_ColorHover = Color.grey;
        private Color m_ColorDraw = Color.green;
        
        [Header("Picture settings")]
        [SerializeField] private int m_InputFieldDimension = 10;
        private PictureController m_PictureController;
        private IPicture m_Picture;
        private int m_PictureWidht;
        private int m_PictureHeight;
        private int m_PicturLayerMask;
    

        [Header("Pencil settings")]
        private PencilController m_PencilController; 
        private IPencil m_Pencil;       

        
        [Header("SYSTEM INPUT")]
        private InputController m_InputController;
        
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
            SetupInput();
            SetupPicture();
            SetupPencil();
            SetupBrain();

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
            throw new System.NotImplementedException();
        }
    
    
        private void SetupInput()
        {
            m_InputController = InputController.Get();
            var inputControllerConfig = new InputControllerConfig(m_CameraMain);
            m_InputController.Configure(inputControllerConfig);
            m_InputController.Init();
        }
    
        private void SetupPicture()
        {
            var pictureWidht = m_InputFieldDimension;
            var pictureHeight = m_InputFieldDimension;
            var picturLayerMask = 8;
            m_Picture = Picture3D.Get();
            var pictureConfig = new PictureConfig(m_Picture, pictureWidht, pictureHeight, m_ColorBackground, m_ColorHover, picturLayerMask, m_Scene);
            m_Picture.Configure(pictureConfig);

            m_PictureController = PictureController.Get();
            var pictureControllerConfig = new PictureControllerConfig(m_Picture);
            m_PictureController.Configure(pictureControllerConfig);
            m_PictureController.Init();
        }

        private void SetupPencil()
        {
            m_Pencil = PencilModel.Get();
            var pencilConfig = new PencilConfig(m_Pencil, m_ColorDraw, m_ColorBackground);
            m_Pencil.Configure(pencilConfig);

            m_PencilController = PencilController.Get();
            var pencilControllerConfig = new PencilControllerConfig(m_Pencil);
            m_PencilController.Configure(pencilControllerConfig);
            m_PencilController.Init();
        }
    
        private void SetupBrain()
        {
            var nerveLayerMask = 9;
            m_Brain = BrainModel.Get();

            var inputLayerDimension = m_InputFieldDimension;
            var analyzeLayerDimension = m_InputFieldDimension / 2;
            var analyzeLayerNumber = 2;

            var brainConfig = new BrainConfig(m_Brain, inputLayerDimension, analyzeLayerDimension, analyzeLayerNumber, nerveLayerMask, m_Scene);
            m_Brain.Configure(brainConfig);

            m_BrainController = BrainController.Get();
            var brainControllerConfig = new BrainControllerConfig(m_Brain, m_Picture);
            m_BrainController.Configure(brainControllerConfig);
            m_BrainController.Init();
        }
    
    
    }
}