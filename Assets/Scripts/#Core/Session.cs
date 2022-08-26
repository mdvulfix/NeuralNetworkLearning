using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    
    public class Session : MonoBehaviour, IConfigurable
    {
        
        
        
        [SerializeField] private GameObject m_Brain;


        public static SceneRootData m_SceneRootData;
        private SessionConfig m_Config;

        public IConfig Config => m_Config;

        public virtual void Configure(params object[] args)
        {
            m_SceneRootData = new SceneRootData(m_Brain);
        }

        public virtual void Init() { }
        public virtual void Dispose() { }













        private void Awake() =>
            Configure();

        private void OnEnable() =>
            Init();

        private void OnDisable() =>
            Dispose();



        private void Update()
        {


        }

        
        
        public static SceneRootData GetSceneRootData() =>
            m_SceneRootData;

    }

    public struct SessionConfig: IConfig
    {

    }

    public struct SceneRootData
    {
        public SceneRootData(GameObject brain)
        {
            Brain = brain;
        }

        public GameObject Brain { get; private set; }
    }
}