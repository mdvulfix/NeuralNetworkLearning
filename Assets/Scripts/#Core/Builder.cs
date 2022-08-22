using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    public class Builder : MonoBehaviour
    {
        
        [SerializeField] private Picture m_Picture;
        [SerializeField] private Pencil m_Pencil;
        
        
        private BuilderConfig m_Config;
        
        [SerializeField] private PencilController m_PencilController;
        [SerializeField] private PictureController m_PictureController;
        
        [SerializeField] private UpdateController m_UpdateController;
        [SerializeField] private AsyncController m_AsyncController;
        
        
        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        private Color m_DrawColor = Color.green;
        

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is BuilderConfig)
                        m_Config = (BuilderConfig)args[0];
                }
            }


            
            m_PencilController = new PencilController();
            m_PictureController = new PictureController();
            m_UpdateController = new UpdateController();
            
        }

        public virtual void Init()
        {
            m_UpdateController.Configure();
            m_UpdateController.Init();
            
            
            var pictureControllerConfig = new PictureControllerConfig(m_BackgroundColor, m_HoverColor);
            m_PictureController.Configure();
            m_PictureController.Init();
            m_Picture = m_PictureController.Picture;
            
            var pencilControllerConfig = new PencilControllerConfig(m_BackgroundColor, m_DrawColor, m_PictureController);
            m_PencilController.Configure(pencilControllerConfig);
            m_PencilController.Init();
            m_Pencil = m_PencilController.Pencil;



            
            
        }

        public virtual void Dispose()
        {
            m_PencilController.Dispose();
            m_PictureController.Dispose();
        
            m_UpdateController.Dispose();
        }
        
        
        
        private void Awake() =>
            Configure();

        private void OnEnable() =>
            Init();

        private void OnDisable() =>
            Dispose();
        
        private void Update() =>
            m_UpdateController.Update();

    }

    public struct BuilderConfig
    {
        
    }
}