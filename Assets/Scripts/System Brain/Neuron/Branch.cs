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
        public Branch(params object[] args) 
            => Configure(args);

        public override void Configure(params object[] args)
        {
            
            
            base.Configure(args);
        }

        
        
        
        
        
        
        
        
        
        private void OnSensorExcited(Сharge charge)
        { 

        }


    }
}