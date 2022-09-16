using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace APP.Brain
{

    public class DendriteDefault : NerveModel, IDendrite
    {
        private List<ISensor> m_Sensors;
        private LineRenderer m_LineRenderer;
        
        public static readonly string PREFAB_Label = "Dendrite";

        public DendriteDefault() { }
        public DendriteDefault(params object[] args)
            => Configure(args);


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

        public override void Init()
        {
            m_Sensors = new List<ISensor>(); 
            
            GetComponent<Transform>(out var parent);
            
            var sensorPosition = new Vector3(Position.x - 0.25f, Position.y, Position.z - 0.25f);
            var sensor = Grow<SensorDefault>(sensorPosition, Size, parent);  
            sensor.Excited += OnSensorExcited;        
            m_Sensors.Add(sensor);
        
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

        
        public override void Activate()
        {
            foreach (var sensor in m_Sensors)
                sensor.Activate(); 

            base.Activate();  
        }

        public override void Deactivate()
        {
            foreach (var sensor in m_Sensors)
                sensor.Deactivate(); 

            base.Deactivate();  
        }
        
        
        public override void Update() 
        {
            foreach (var sensor in m_Sensors)
                sensor.UpdateBond(Color.yellow, Position, sensor.Position);
            
            base.Update();
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
                GetComponent<Transform>(out var parent);
                sensor = Grow<SensorDefault>(Position, Size, parent);
                sensor.Excited += OnSensorExcited;
                sensor.Activate();
                return true;
            }
            
            return false;
        }


        private int LimitCalculate()
        {
            return 1;
        }        
        
        protected override void Impulse() {}
        
        /*
        public void Attach(ISensible sensible)
        {
            var sensor = Sensor.Get();

            sensor.Excited += OnSensorExcited;
            sensible.SetSensor(sensor);
        }
        */

        public override void UpdateBond(Color color, params Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
                m_LineRenderer.SetPosition(i, points[i]);
        }

        private void OnSensorExcited(Сharge charge)
        {

        }


    }

    public interface IDendrite : INerve
    {
        bool GetSensor(out ISensor sensor);


    }

    public partial class NerveFactory : Factory<INerve>
    {
        private DendriteDefault GetDendriteDefault(params object[] args)
        {       
            var prefabPath = $"{NerveModel.PREFAB_Folder}/{DendriteDefault.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Dendrite");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<DendriteDefault>();
            obj.name = $"Dendrite";

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