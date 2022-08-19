using System;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public abstract class Nerve<T> where T : IConfigurable, new()
    {
        
        [SerializeField] private Vector3 m_Head;
        [SerializeField] private Vector3 m_Tail;
        [SerializeField] private float m_Width;
        
        
        public Vector3 Head => m_Head;
        public Vector3 Tail => m_Tail;
        public float Width => m_Width;

        public virtual void Configure(params object[] args)
        {
            m_Head = (Vector3) args[0];
            m_Tail = (Vector3) args[1];
            m_Width = (float) args[2];
        }

        public static T Get(Vector3 head, Vector3 tail, float width)
        {
            var instance = new T();
            instance.Configure(head, tail, width);

            return instance;
        }

    }
}