using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    public class Sensor : MonoBehaviour, IConfigurable
    {

        private SensorConfig m_Config;
        
        [SerializeField] private float m_ExciteRate;
        
        //Fixed frame rate = 0.02;
        //Target frame number = 50 in ms;
        
        private float m_ExciteRateDefault = 5;

        private List<Branch> m_Branches;

        private GameObject m_GameObject;
        private Transform m_Transform;
        private MeshRenderer m_Renderer;
        

        public IConfig Config => m_Config;

        public event Action<Сharge> Excited;


        public virtual void Configure(params object[] args)
        {
            m_GameObject = gameObject;

            if (m_GameObject.TryGetComponent<Transform>(out m_Transform) == false)
                m_Transform = m_GameObject.AddComponent<Transform>();

            if (m_GameObject.TryGetComponent<MeshRenderer>(out m_Renderer) == false)
            {
                m_Renderer = m_GameObject.AddComponent<MeshRenderer>();
                //m_Renderer.sprite = HandlerSprite.Circle;
            }

            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is SensorConfig)
                    {
                        m_Config = (SensorConfig)arg;
                        

                    }
                }
            }



             
        }


        public virtual void Init()
        {


        }

        public virtual void Dispose()
        {


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

        private void Awake() =>
            Configure();

        private void OnEnable() =>
            Init();

        private void OnDisable() =>
            Dispose();

    
    
    
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