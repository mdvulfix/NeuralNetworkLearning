using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace APP.Console
{

    public class TestAsync : MonoBehaviour
    {

        [SerializeField] private GameObject FOLDER_SPAWNED;
        [SerializeField] private GameObject FOLDER_AWAITERS;
        [SerializeField] private GameObject FOLDER_AWAITERS_POOL;

        private Pool m_AwaiterPool;
        private int m_AwaiterPoolLimit = 5;


        private Awaiter m_AwaiterActive;

        private List<Awaiter> m_AwaiterIsReady;

        private Action AwaiterCallback;


        private void Awake()
        {
            m_AwaiterPool = Pool.Get(new PoolConfig());
            m_AwaiterIsReady = new List<Awaiter>(15);

            for (int i = 0; i < 3; i++)
                m_AwaiterPool.Push(Awaiter.Get(FOLDER_AWAITERS_POOL));

            //m_Funcs.Enqueue(OperationAsync_1());
            //m_Funcs.Enqueue(OperationAsync_2());
            //m_Funcs.Enqueue(OperationAsync_3());
            //m_Funcs.Enqueue(OperationAsync_4);
            //m_Funcs.Enqueue(OperationAsync_5);
        }

        private void Start()
        {

            for (int i = 0; i < 3; i++)
            {
                var label = "Cahce " + i;
                var cache = Spawn(label, FOLDER_SPAWNED);
                cache.Setup(label, Random.Range(1, 10));

                var awaiter = GetAwaiterInReadyState();
                ExecuteAsync(awaiter, cache.LoadAsync(() => awaiter.Stop()));

            }

        }

        private void ExecuteAsync(Awaiter awaiter, IEnumerator operationAsync)
        {
            awaiter.SetFunc(operationAsync);
            awaiter.Run();
        }

        private Awaiter GetAwaiterInReadyState()
        {
            if (m_AwaiterIsReady.Count > 0)
            {
                var awaiter = (from Awaiter awaiterIsReady in m_AwaiterIsReady
                               where awaiterIsReady.IsReady == true
                               select awaiterIsReady).First();
                return awaiter;
            }
            else
            {
                if (PoolPopAwaiter(out var awaiter) == false)
                    throw new Exception("Pool is empty!");

                PoolUpdate();
                return awaiter;
            }
        }

        private bool PoolPopAwaiter(out Awaiter awaiter)
        {
            awaiter = null;

            if (m_AwaiterPool.Pop(out var instance) == false)
                return false;

            awaiter = (Awaiter)instance;
            awaiter.transform.SetParent(FOLDER_AWAITERS.transform);
            awaiter.Initialized += OnAwaiterInitialized;
            awaiter.Disposed += OnAwaiterDisposed;
            awaiter.FuncReceived += OnAwaiterFuncReceived;
            awaiter.FuncStarted += OnAwaiterBusy;
            awaiter.FuncCompleted += OnAwaiterFuncComplete;
            awaiter.Load();

            return true;
        }

        private void PoolPushAwaiter(Awaiter awaiter)
        {
            awaiter.transform.SetParent(FOLDER_AWAITERS_POOL.transform);
            awaiter.Initialized -= OnAwaiterInitialized;
            awaiter.Disposed -= OnAwaiterDisposed;
            awaiter.FuncReceived -= OnAwaiterFuncReceived;
            awaiter.FuncStarted -= OnAwaiterBusy;
            awaiter.FuncCompleted -= OnAwaiterFuncComplete;
            awaiter.Unload();

            m_AwaiterPool.Push(awaiter);
        }

        private void PoolUpdate()
        {
            var awaiterNumber = m_AwaiterPool.Count;

            if (awaiterNumber < m_AwaiterPoolLimit)
            {
                for (int i = 0; i < m_AwaiterPoolLimit - awaiterNumber; i++)
                    m_AwaiterPool.Push(Awaiter.Get(FOLDER_AWAITERS_POOL));
            }
        }


        private IEnumerator OperationAsync_1(int attamps = 5, int awaiting = 1)
        {
            var cache = Spawn("Cahce 1", FOLDER_SPAWNED);
            cache.Load();

            while (true)
            {
                if (cache.IsLoaded == true)
                {
                    Debug.Log("Operation: Success!");
                    break;
                }

                Debug.Log("Waiting for Cahce 1 loading... Attemps: " + attamps);
                yield return new WaitForSeconds(awaiting);

                attamps--;
                if (attamps == 0)
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

            while (true)
            {
                if (cache.IsLoaded == true)
                {
                    Debug.Log("Operation: Success!");
                    break;
                }

                Debug.Log("Waiting for Cahce 2 loading... Attemps: " + attamps);
                yield return new WaitForSeconds(awaiting);

                attamps--;
                if (attamps == 0)
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

            while (true)
            {
                if (cache.IsLoaded == true)
                {
                    Debug.Log("Operation: Success!");
                    break;
                }

                Debug.Log("Waiting for Cahce 3 loading... Attemps: " + attamps);
                yield return new WaitForSeconds(awaiting);

                attamps--;
                if (attamps == 0)
                {
                    Debug.LogWarning("Operation: Failed!");
                    throw new Exception("Operation done by time delay!");
                }
            }

            Debug.Log("Cahce 3 loaded");
        }


        private void OnAwaiterInitialized(Awaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);
        }

        private void OnAwaiterDisposed(Awaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

        }

        private void OnAwaiterFuncReceived(Awaiter awaiter)
        {
            if (m_AwaiterIsReady.Contains(awaiter))
                m_AwaiterIsReady.Remove(awaiter);

        }

        private void OnAwaiterBusy(Awaiter awaiter)
        {

        }

        private void OnAwaiterFuncComplete(Awaiter awaiter)
        {
            m_AwaiterIsReady.Add(awaiter);
        }


        private void OnEnable()
        {



        }

        private void OnDisable()
        {

        }

        private void Undate()
        {

        }

        private Cache Spawn(string name, GameObject parent)
        {
            var obj = new GameObject(name);

            if (parent == null)
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

    public class Cache : MonoBehaviour
    {

        [SerializeField] private bool m_IsLoaded;
        [SerializeField] private float m_LoadingTime;
        [SerializeField] private string m_Label;

        public bool IsLoaded => m_IsLoaded;

        public void Setup(string label, int loadingTime)
        {
            m_LoadingTime = loadingTime;
            m_Label = label;
        }

        public void Load()
        {
            gameObject.SetActive(true);
        }

        public IEnumerator LoadAsync(Action callback)
        {
            while (m_LoadingTime > 0)
            {
                m_LoadingTime -= Time.deltaTime;
                Debug.Log($"{m_Label}: waiting for operation complite...");
                yield return new WaitForSeconds(0.01f);
            }

            
            
            gameObject.SetActive(true);
            Debug.Log("Cache.LoadAsync() is done");

            callback.Invoke();
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