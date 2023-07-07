using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace APP.Test
{
    [Serializable]
    public class DebugAsync : MonoBehaviour
    {
        [SerializeField] private GameObject m_Boll;


        private static Transform m_ObjSpawnHolder;
        private static Transform m_ObjAsyncHolder;

        //public static AsyncController AsyncController => m_Controller;

        private List<IAsyncController> m_Controllers;

        private void Awake()
        {
            m_Controllers = new List<IAsyncController>(10);
            for (int i = 0; i < 1; i++)
            {



                //m_Controllers.Add();
            }



        }

        private void OnEnable()
        {

            if (m_ObjSpawnHolder == null)
                m_ObjSpawnHolder = new GameObject("Spawn").transform;

            if (m_ObjAsyncHolder == null)
                m_ObjAsyncHolder = new GameObject("Async").transform;


            //foreach (var controller in m_Controllers)
            //    controller.Init();

        }

        private void OnDisable()
        {
            //foreach (var controller in m_Controllers)
            //   controller.Dispose();
        }

        private void Start()
        {

            var controller = new AsyncController();
            var config = new AsyncControllerConfig(m_ObjAsyncHolder);
            controller.Configure(config);
            controller.Init();

            for (int i = 0; i < 5; i++)
            {
                var label = "Boll " + i;
                var position = new Vector3(Random.Range(0f, 2f), Random.Range(0f, 3f), Random.Range(0f, 2f));
                var boll = Spawn<BollDefault>(label, position, m_Boll, m_ObjSpawnHolder);
                boll.Configure();
                boll.Init();
                boll.Activate();


                //m_Controllers[0].ExecuteAsync(cache.LoadAsync);


                controller.ExecuteAsync(boll.SetColorAsync);

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


        private T Spawn<T>(string name, Vector3 position, GameObject prefab, Transform parent)
        where T : Component
        {
            GameObject obj;

            if (prefab == null)
            {
                obj = new GameObject();
                obj.AddComponent<T>();
                obj.transform.position = position;
            }
            else
            {
                obj = Instantiate(prefab, position, Quaternion.identity);
            }

            if (parent == null)
                parent = transform;

            obj.name = name;
            obj.transform.SetParent(parent);
            obj.SetActive(false);

            return obj.GetComponent<T>();
        }

    }
}