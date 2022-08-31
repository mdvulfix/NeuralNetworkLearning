using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    
    public class Session : AConfigurableOnAwake, IConfigurable, IUpdateble
    {
        
        [SerializeField] private GameObject m_Brain;
        
        public static SceneRootData m_SceneRootData;


        public override void Configure(params object[] args)
        {
            
            
            m_SceneRootData = new SceneRootData(m_Brain);

            base.Configure(args);
        }



        public void Update()
        {


        }

        
        
        public static SceneRootData GetSceneRootData() =>
            m_SceneRootData;

    }

    public struct SessionConfig : IConfig
    {
        public void Setup(IConfigurable configurable) { }
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