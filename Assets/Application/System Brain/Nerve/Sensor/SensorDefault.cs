using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{

    public class SensorDefault : NerveModel, ISensor
    {
        
        [SerializeField] private float m_ExciteRate;

        //Fixed frame rate = 0.02;
        //Target frame number = 50 in ms;

        private float m_ExciteRateDefault = 5;


        public ISensible Sensible { get; private set; }

        public event Action<Сharge> Excited;

        public static readonly string PREFAB_Label = "Sensor";

        public void Associate(ISensible sensible)
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

        protected override void Impulse() {}

    }

    public interface ISensor : INerve
    {
        ISensible Sensible {get; }
        
        event Action<Сharge> Excited;
    
        void Associate(ISensible sensible);
    
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

    public partial class NerveFactory : Factory<INerve>
    {
        private SensorDefault GetSensorDefault(params object[] args)
        {       
            var prefabPath = $"{NerveModel.PREFAB_Folder}/{SensorDefault.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Sensor");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<SensorDefault>();
            obj.name = $"Sensor { instance.GetHashCode() } ";

            //var instance = new Pixel3D();

            if (args.Length > 0)
            {
                var config = (NerveConfig)args[NerveModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }
}