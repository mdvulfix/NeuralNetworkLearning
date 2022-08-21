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
        public Dendrite(Vector3 head, Vector3 tail, float width) =>
            Configure(head, tail, width);

        public override void Configure(params object[] args)
        {
            base.Configure(args);
            
            m_Sensors = new List<Sensor>();
            
        }

        public virtual void Init() { }
        
        
        public virtual void Dispose() 
        { 
            
            if(m_Sensors.Count > 0)
                foreach (var sensor in m_Sensors)
                    sensor.Excited -= OnSensorExcited;
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