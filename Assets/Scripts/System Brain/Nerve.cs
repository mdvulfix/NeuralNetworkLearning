using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public abstract class Nerve<T>: AConfigurable, IConfigurable
    where T : IConfigurable, new()
    {
        
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


        public static T Get(Vector3 head, Vector3 tail, float width)
        {
            var instance = new T();
            instance.Configure(head, tail, width);

            return instance;
        }

    }

    public class NerveConfig
    {
        public NerveConfig(Vector3 head, Vector3 tail, float width)
        {
            Head = head;
            Tail = tail;
            Width = width;
        }

        public Vector3 Head { get; internal set; }
        public Vector3 Tail { get; internal set; }
        public float Width { get; internal set; }
    }
}