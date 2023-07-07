using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace APP.Brain
{

    public class SensorDefault : NerveModel, ISensor
    {
        
        private LineRenderer m_LineRenderer;
        
        [SerializeField] private float m_ExciteRate;

        //Fixed frame rate = 0.02;
        //Target frame number = 50 in ms;

        private float m_ExciteRateDefault = 5;


        public ISensible Sensible { get; private set; }

        public event Action<Сharge> Excited;

        public static readonly string PREFAB_Label = "Sensor";

        
        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            if (GetComponent<LineRenderer>(out m_LineRenderer) == false)
                m_LineRenderer = SetComponent<LineRenderer>();


            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }
            
            // CONFIGURE BY DEFAULT //
            var position = Vector3.zero;
            var size = URandom.Range(0f, 100f);
            var energy = URandom.Range(0f, 100f);
            var layerMask = 9;
            
            Transform parent = null;
            if (Seacher.Find<IScene>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var neuronParent))
                    parent = neuronParent != null ? neuronParent : transform.parent;

            var config = new NerveConfig(this, position, size, layerMask, parent);

            base.Configure(config);
            Send($"The instance was configured by default!");
        }
        
        
        
        
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

        public override void UpdateBond(Color color, params Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
                m_LineRenderer.SetPosition(i, points[i]);
 
        }
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
            obj.name = $"Sensor";

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