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

        private List<AsyncController> m_Controllers;
        
        private void Awake()
        {
            m_Controllers = new List<AsyncController>(10);
            //for (int i = 0; i < 1; i++)
                //m_Controllers.Add(AsyncController.Get(new AsyncControllerConfig()));
            

        }

        private void OnEnable()
        {
            foreach (var controller in m_Controllers)
                controller.Init();

        }

        private void OnDisable()
        {
            foreach (var controller in m_Controllers)
                controller.Dispose();
        }

        private void Start()
        {

            for (int i = 0; i < 2; i++)
            {
                var label = "Cahce " + i;
                var cache = Spawn(label, FOLDER_SPAWNED);
                cache.Setup(label, Random.Range(1, 4));

                
                m_Controllers[0].ExecuteAsync(cache.LoadAsync);
                
                //HandlerAsync.ExecuteAsync(cache.LoadAsync);
                //m_Controller.ExecuteAsync(cache.LoadAsync);
                //m_Controller2.ExecuteAsync(cache.LoadAsync);
            }
        }
        
        private void Update() 
        {
            foreach (var controller in m_Controllers)
                controller.Update();
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