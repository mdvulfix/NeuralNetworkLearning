using System;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Branch: Nerve<Branch>, IConfigurable
    {
        
        
        private Sensor m_Sensor;

        public Sensor Sensor => m_Sensor;

        public Branch() { }
        public Branch(Vector3 head, Vector3 tail, float width) =>
            Configure(head, tail, width);

        public override void Configure(params object[] args)
        {
            base.Configure(args);
            

        }

        public virtual void Init() 
        { 
            
        }
        public virtual void Dispose() 
        { 
            m_Sensor.Excited -= OnSensorExcited;
        }




        private void OnSensorExcited(Сharge charge)
        { 

        }


    }
}