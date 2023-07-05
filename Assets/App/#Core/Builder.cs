using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using APP.Draw;

namespace APP
{
    public class Builder : AConfigurableOnAwake, IConfigurable, IUpdateble
    {
        [SerializeField] private PencilController m_PencilController;
        [SerializeField] private PictureController m_PictureController;
        
        [SerializeField] private UpdateController m_UpdateController;
        [SerializeField] private AsyncController m_AsyncController;
        
        
        private Color m_BackgroundColor = Color.black;
        private Color m_HoverColor = Color.grey;
        private Color m_DrawColor = Color.green;
        

        public override void Configure(params object[] args)
        {
            var config = new BuilderConfig();
            base.Configure(config);
        }

        public override void Init()
        {
            var updateControllerConfig = new UpdateControllerConfig();
            m_UpdateController = new UpdateController(updateControllerConfig);
            
            //var pictureControllerConfig = new PictureControllerConfig(m_BackgroundColor, m_HoverColor);
            //m_PictureController = new PictureController(pictureControllerConfig);
            
            //var pencilControllerConfig = new PencilControllerConfig(m_BackgroundColor, m_DrawColor, m_PictureController);
            //m_PencilController = new PencilController(pencilControllerConfig);



            base.Init();
        }

        public override void Dispose()
        {
            m_PencilController.Dispose();
            m_PictureController.Dispose();
        
            m_UpdateController.Dispose();

            base.Dispose();
        }
        
        
        public void Update() 
            => m_UpdateController.Update();


    }

    public struct BuilderConfig: IConfig
    {
        
    }
}