using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    
    [Serializable]
    public abstract class NerveModel: AConfigurable
    {
        private INerve m_Nerve;
        
        
        [SerializeField] private Vector3 m_Head;
        [SerializeField] private Vector3 m_Tail;
        [SerializeField] private float m_Width;

        private List<Сharge> m_СhargeReceived;

        public Vector3 Head => m_Head;
        public Vector3 Tail => m_Tail;
        public float Width => m_Width;

        public override void Configure(params object[] args)
        {
            var config = (NerveConfig)args[PARAM_INDEX_Config];
            
            m_Nerve = config.Nerve;
            
            m_Head = config.Head;
            m_Tail = config.Tail;
            m_Width = config.Width;

            
            base.Configure(args);
        }

        
        
        public void Impulse()
        { 

        }

        public virtual void CargeCalculate()
        { 
            foreach (var charge in m_СhargeReceived)
            {
                
            }
        }


        public static TNerve Get<TNerve>(params object[] args)
        where TNerve: INerve, new() //Component, IBrain
        {
            //var obj = new GameObject("Brain");
            //obj.SetActive(false);

            //var renderer = obj.AddComponent<MeshRenderer>();           
            //var instance = obj.AddComponent<TBrain>();
            var instance = new TNerve();
            
            if(args.Length > 0)
            {
                instance.Configure(args);
                instance.Init();
            }
            
            return instance;
        }

    }

    public interface INerve: IConfigurable
    {

    }

    public class NerveConfig
    {
        public NerveConfig(INerve nerve, Vector3 head, Vector3 tail, float width)
        {
            Head = head;
            Tail = tail;
            Width = width;
            Nerve = nerve;
        }


        public INerve Nerve { get; private set; }
        public Vector3 Head { get; private set; }
        public Vector3 Tail { get; private set; }
        public float Width { get; private set; }
        
    }
}