using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Brain
{

    public class Dendrite : NerveModel, IDendrite
    {
        private List<ISensor> m_Sensors;
      
        
        public Dendrite() { }
        public Dendrite(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {


            base.Configure(args);
        }

        public override void Init()
        {
            m_Sensors = new List<ISensor>();            
            m_Sensors.Add(GrowSensor());
        
            base.Init();
        }


        public override void Dispose()
        {
            if (m_Sensors.Count > 0)
                foreach (var sensor in m_Sensors)
                    sensor.Excited -= OnSensorExcited;

            
            m_Sensors.Clear();

            base.Dispose();
        }

        public bool GetSensor(out ISensor sensor)
        {
            sensor = null;
            var sensors = (from ISensor sensorAvailable in m_Sensors
                          where sensorAvailable.Sensible == null
                          select sensorAvailable).ToArray();

            if(sensors.Count() > 0)
            {
                sensor = sensors[0];
                return true;
            }
                
            if(m_Sensors.Count < LimitCalculate())
            {
                sensor = GrowSensor();
                return true;
            }
            
            return false;
        }

        private ISensor GrowSensor()
        {
            var sensor = NerveModel.Get<Sensor>();
            var sensorConfig = new SensorConfig(sensor);
            
            sensor.Excited += OnSensorExcited;
            sensor.Configure(sensorConfig);
            sensor.Init();

            return sensor;
        }

        private int LimitCalculate()
        {
            return 1;
        }        
        
        
        /*
        public void Attach(ISensible sensible)
        {
            var sensor = Sensor.Get();

            sensor.Excited += OnSensorExcited;
            sensible.SetSensor(sensor);
        }
        */


        private void OnSensorExcited(Сharge charge)
        {

        }


    }

    public interface IDendrite : INerve
    {
        bool GetSensor(out ISensor sensor);


    }
}