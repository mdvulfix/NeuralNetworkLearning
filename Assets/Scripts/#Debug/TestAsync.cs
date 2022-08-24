using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace APP.Test
{

    public class TestAsync : MonoBehaviour
    {

        [SerializeField] private Transform FOLDER_SPAWNED;

        //public static AsyncController AsyncController => m_Controller;

        private AsyncController m_Controller;
        
        private void Awake()
        {
            m_Controller = AsyncController.Get(new AsyncControllerConfig());

        }

        private void OnEnable()
        {
            m_Controller.Init();
        }

        private void OnDisable()
        {
            m_Controller.Dispose();
        }

        private void Start()
        {

            for (int i = 0; i < 2; i++)
            {
                var label = "Cahce " + i;
                var cache = Spawn(label, FOLDER_SPAWNED);
                cache.Setup(label, Random.Range(1, 4));

                
                //HandlerAsync.ExecuteAsync(cache.LoadAsync);
                //m_Controller.ExecuteAsync(cache.LoadAsync);
            }
        }
        
        private void Update() 
        {
            m_Controller.Update();
        }


        private Cache Spawn(string name, Transform parent)
        {
            var obj = new GameObject(name);

            if (parent == null)
                parent = transform;
           
            obj.transform.SetParent(parent);
            obj.SetActive(false);

            return obj.AddComponent<Cache>();
        }

    }
}