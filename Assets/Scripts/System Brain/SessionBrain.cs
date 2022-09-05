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
        
            
                
            
            
            base.Configure(args);
        }


        public override void Init()
        {
            m_SceneRootData = new SceneRootData(m_Scene);
            
            var brainConfig = new BrainConfig(m_Picture);
            m_Brain = BrainModel.Get<BrainDefault>();
            m_Brain.Configure();
            m_Brain.Init();

            base.Init();
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