using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{

    public class Sensor : NerveModel, ISensor
    {
        [SerializeField] private float m_ExciteRate;

        //Fixed frame rate = 0.02;
        //Target frame number = 50 in ms;

        private float m_ExciteRateDefault = 5;


        public ISensible Sensible { get; private set; }

        public event Action<Сharge> Excited;


        public void Attach(ISensible sensible)
        {
            Sensible = sensible;
        }


        public void Excite()
        {
            m_ExciteRate -= Time.fixedDeltaTime;

            if (m_ExciteRate <= 0)
            {
                m_ExciteRate = m_ExciteRateDefault;
                var charge = Сharge.Get();
                charge.SetEnergy();

                Excited?.Invoke(charge);
            }

        }
    }

    public interface ISensor : INerve
    {
        ISensible Sensible {get; }
        
        event Action<Сharge> Excited;
    
        void Attach(ISensible sensible);
    
    }

    public interface ISensible
    {
        Vector3 Position { get; }

        void Excite();

    }

    public struct SensorConfig : IConfig
    {
        public ISensor Sensor {get; private set; }

        public SensorConfig(ISensor sensor)
        {
            Sensor = sensor;
        }
    }
}