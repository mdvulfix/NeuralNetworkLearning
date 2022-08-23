using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APP.Console
{

    public class TestAsync : MonoBehaviour
    {

        [SerializeField] private GameObject FOLDER_SPAWNED;
        [SerializeField] private GameObject FOLDER_AWAITERS;
        [SerializeField] private GameObject FOLDER_AWAITERS_POOL;
        
        
        private List<Awaiter> m_Pool;
        private List<Awaiter> m_Awaiter;

        private Queue<FuncAsyncInfo> m_FuncAwaitingList;

        private void Awake()
        {
            m_Pool = new List<Awaiter>(10);
            m_Awaiter = new List<Awaiter>(10);
            m_FuncAwaitingList = new Queue<FuncAsyncInfo>(50);

            for (int i = 0; i < 10; i++)
                m_Pool.Add(Awaiter.Get(FOLDER_AWAITERS_POOL));

            //m_Funcs.Enqueue(OperationAsync_1());
            //m_Funcs.Enqueue(OperationAsync_2());
            //m_Funcs.Enqueue(OperationAsync_3());
            //m_Funcs.Enqueue(OperationAsync_4);
            //m_Funcs.Enqueue(OperationAsync_5);
        }

        private void Start()
        {

            var awaitersInReadyStateNumber = 5;
            var awaitersAwaliable = m_Pool.Count - awaitersInReadyStateNumber;

            if (awaitersAwaliable < 0)
            {
                for (int i = 0; i < -1 * awaitersAwaliable; i++)
                    m_Pool.Add(Awaiter.Get());
            }

            for (int i = 0; i < awaitersInReadyStateNumber; i++)
            {
                var awaiterInReadyState = m_Pool[i];
                awaiterInReadyState.transform.SetParent(FOLDER_AWAITERS.transform);
                awaiterInReadyState.Load();

            }


            var cache = Spawn("Cahce 1", FOLDER_SPAWNED);
            
            var awaiter = FuncAsyncGetAwaiterInReadyState();
            
            FuncAsyncAdd(awaiter, cache.LoadAsync(() => awaiter.Stop()));
            FuncAsyncExecute();

        }

        private Awaiter FuncAsyncGetAwaiterInReadyState()
        {
            var awaitersInReadyState = from awaiter in m_Awaiter
                                       where awaiter.IsReady == true
                                       select awaiter;

            if (awaitersInReadyState.Count() == 0)
                if(m_Pool[0].Load(out var awaiter))
                    return awaiter;
        
            return awaitersInReadyState.First();
        
        }
        
        private void FuncAsyncAdd(Awaiter awaiter, IEnumerator operationAsync)
        {
            m_FuncAwaitingList.Enqueue(new FuncAsyncInfo(awaiter, operationAsync));
        }
        
        private void FuncAsyncExecute()
        {
            if(m_FuncAwaitingList.Count == 0)
                return;
            
            foreach (var funcInfo in m_FuncAwaitingList)
            {
                var awaiter = funcInfo.Awaiter;
                var operation = funcInfo.OperationAsync;
                awaiter.Run(operation);

            }
        }

        private IEnumerator OperationAsync_1(int attamps = 5, int awaiting = 1)
        {
            var cache = Spawn("Cahce 1", FOLDER_SPAWNED);
            cache.Load();
            
            while(true)
            {
                if(cache.IsLoaded == true)
                {
                    Debug.Log("Operation: Success!");
                    break;
                }
                
                Debug.Log("Waiting for Cahce 1 loading... Attemps: " + attamps);
                yield return new WaitForSeconds(awaiting);

                attamps--;
                if(attamps == 0)
                {
                    Debug.LogWarning("Operation: Failed!");
                    throw new Exception("Operation done by time delay!");
                }
            } 

            Debug.Log("Cahce 1 loaded");
        }

        private IEnumerator OperationAsync_2(int attamps = 5, int awaiting = 1)
        {
            var cache = Spawn("Cahce 2", FOLDER_SPAWNED);
            cache.Load();
            
            while(true)
            {
                if(cache.IsLoaded == true)
                {
                    Debug.Log("Operation: Success!");
                    break;
                }
                
                Debug.Log("Waiting for Cahce 2 loading... Attemps: " + attamps);
                yield return new WaitForSeconds(awaiting);

                attamps--;
                if(attamps == 0)
                {
                    Debug.LogWarning("Operation: Failed!");
                    throw new Exception("Operation done by time delay!");
                }
            } 

            Debug.Log("Cahce 2 loaded");
        }

        private IEnumerator OperationAsync_3(int attamps = 5, int awaiting = 1)
        {
            var cache = Spawn("Cahce 3", FOLDER_SPAWNED);
            cache.Load();
            
            while(true)
            {
                if(cache.IsLoaded == true)
                {
                    Debug.Log("Operation: Success!");
                    break;
                }
                
                Debug.Log("Waiting for Cahce 3 loading... Attemps: " + attamps);
                yield return new WaitForSeconds(awaiting);

                attamps--;
                if(attamps == 0)
                {
                    Debug.LogWarning("Operation: Failed!");
                    throw new Exception("Operation done by time delay!");
                }
            } 

            Debug.Log("Cahce 3 loaded");
        }



        private void OnAwaiterInitialized(Awaiter awaiter)
        {
            if(m_Pool.Contains(awaiter))
                m_Pool.Remove(awaiter);
            
            m_Awaiter.Add(awaiter);
        }

        private void OnAwaiterDisposed(Awaiter awaiter)
        {
            if(m_Awaiter.Contains(awaiter))
                m_Awaiter.Remove(awaiter);
            
            m_Pool.Add(awaiter);
        }




        private void OnEnable()
        {

            foreach (var awaiter in m_Pool)
            {
                awaiter.Initialized += OnAwaiterInitialized;

            }

        }

        private void OnDisable()
        {
            foreach (var awaiter in m_Pool)
            {
                awaiter.Initialized -= OnAwaiterInitialized;

            }
        }

        private void Undate()
        {
            FuncAsyncExecute();
        }


        private Cache Spawn(string name, GameObject parent)
        {
            var obj = new GameObject(name);
            
            if(parent == null)
                parent = gameObject;
            obj.transform.SetParent(parent.transform);
            
            obj.SetActive(false);
            
           return obj.AddComponent<Cache>();
        }

    }

    public struct FuncAsyncInfo
    {
        public FuncAsyncInfo(Awaiter awaiter, IEnumerator operationAsync)
        {
            OperationAsync = operationAsync;
            Awaiter = awaiter;
        }

        public IEnumerator OperationAsync { get; private set; }
        public Awaiter Awaiter { get; private set; }
    }

    public class Cache: MonoBehaviour
    {

        [SerializeField] private bool m_IsLoaded;
        
        public bool IsLoaded => m_IsLoaded;

        
        public void Load()
        {
            gameObject.SetActive(true);
        }
        
        public IEnumerator LoadAsync(Action result)
        {
            yield return new WaitForSeconds(5);
            gameObject.SetActive(true);
            Debug.Log("Cache.LoadAsync() is done");

            result.Invoke();
        }
        
        private void OnEnable()
        {

            m_IsLoaded = true;
        }

        private void OnDisable()
        {
            m_IsLoaded = false;
        }
    }

}
