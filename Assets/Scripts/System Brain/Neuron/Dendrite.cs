using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Dendrite : Nerve<Dendrite>, IConfigurable
    {
        private List<Sensor> m_Sensors;

        public Dendrite() { }
        public Dendrite(params object[] args) 
            => Configure(args);

        public override void Configure(params object[] args)
        {
            
            
            base.Configure(args);
        }

        public override void Init() 
        {
            m_Sensors = new List<Sensor>();
            
            base.Init();
        }
        
        
        public override void Dispose() 
        { 
            
            if(m_Sensors.Count > 0)
                foreach (var sensor in m_Sensors)
                    sensor.Excited -= OnSensorExcited;

            base.Dispose();
        }

        public void Attach(ISensible sensible)
        {
            var sensor = Sensor.Get();

            sensor.Excited += OnSensorExcited;
            sensible.SetSensor(sensor);
        }



        private void OnSensorExcited(Сharge charge)
        { 

        }
    
    }
}