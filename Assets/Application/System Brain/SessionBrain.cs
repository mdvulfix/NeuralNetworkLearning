using UnityEngine;
using UCamera = UnityEngine.Camera;


using APP.Brain;

namespace APP
{
    
    public class SessionBrain : AConfigurableOnAwake, IConfigurable, IUpdateble
    {
        
        [SerializeField] private Transform m_Scene;
        [SerializeField] private IRecognizable m_Picture;
        [SerializeField] private IBrain m_Brain;

        public static SceneRootData m_SceneRootData;


        public override void Configure(params object[] args)
        {
            if (IsConfigured == true)
                return;

            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }
                
            var config = new SessionConfig();
            base.Configure(config);
        }


        public override void Init()
        {
            m_SceneRootData = new SceneRootData(m_Scene);
            
            m_Brain = BrainModel.Get<BrainDefault>();
            
            //TODO: Add picture!
            m_Picture = null;
            
            var brainConfig = new BrainConfig(m_Brain, m_Picture);
            
            m_Brain.Configure(brainConfig);
            m_Brain.Init();

            base.Init();
        }


        public override void Dispose()
        {
            
            m_Brain.Dispose();
            
            base.Dispose();
        }



        public void Update()
        {


        }

        
        
        public static SceneRootData GetSceneRootData() =>
            m_SceneRootData;

    }

    public struct SessionConfig : IConfig
    {

    
    }

    public struct SceneRootData
    {
        public SceneRootData(Transform scene)
        {
            Scene = scene;
            //Brain = brain;
        }

        public Transform Scene { get; private set; }
        //public Transform Brain { get; private set; }
    }
}