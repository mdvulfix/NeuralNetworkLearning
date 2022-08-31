using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    public class Sensor : AConfigurableOnAwake, IConfigurable
    {       
        [SerializeField] private float m_ExciteRate;
        
        //Fixed frame rate = 0.02;
        //Target frame number = 50 in ms;
        
        private float m_ExciteRateDefault = 5;

        private List<Branch> m_Branches;

        
        public event Action<Сharge> Excited;


        public override void Configure(params object[] args)
        {
            var config = (SensorConfig)args[PARAM_INDEX_Config];
             
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


        public static Sensor Get()
        {
            var obj = new GameObject("Sensor");
            return obj.AddComponent<Sensor>();
        }
    
    }

    public struct SensorConfig: IConfig
    {
        
    }
}