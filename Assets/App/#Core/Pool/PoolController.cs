using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Pool
{


    public class PoolController<TPoolable> : PoolController, IPoolController<TPoolable>
    where TPoolable : IPoolable
    {
        private static IPool<TPoolable> m_Pool;

        private IFactory m_FactoryPoolable;
        //private IFactory<TPoolable, TPoolableConfig> m_FactoryPoolable;

        public PoolController() { }
        public PoolController(params object[] args)
        {
            Configure(args);
            Init();
        }



        public bool Push(TPoolable poolable)
        {
            poolable.Dispose();

            return m_Pool.Push(poolable);
        }

        public bool Pop(out TPoolable poolable)
        {
            if (m_Pool.Pop(out poolable))
            {
                poolable.Init();
                return true;
            }

            return false;
        }

        public bool Peek(out TPoolable poolable)
        {
            if (m_Pool.Peek(out poolable))
            {
                poolable.Init();
                return true;
            }

            return false;
        }


        public static new PoolController<TPoolable> Get(params object[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    var config = (PoolControllerConfig)args[PARAMS_Config];
                    var instance = new PoolController<TPoolable>();
                    instance.Configure(config);
                    instance.Init();
                    return instance;
                }

                catch { Debug.Log("Custom factory not found! The instance will be created by default."); }
            }

            return new PoolController<TPoolable>(); ;
        }
    }


    public class PoolController : ModelController, IPoolController
    {
        private static IPool m_Pool;

        private GetPoolableDelegate m_GetPoolableDelegate;

        public PoolController() { }
        public PoolController(params object[] args)
            => Configure(args);



        public override void Configure(params object[] args)
        {
            var config = (PoolControllerConfig)args[PARAMS_Config];

            m_GetPoolableDelegate = config.GetPoolableDelegate;

            base.Configure();
        }

        public override void Init()
        {
            var limit = 5;
            var poolConfig = new PoolConfig(limit, m_GetPoolableDelegate);


            if (m_Pool == null)
            {
                m_Pool = new PoolDefault();
                m_Pool.Configure(poolConfig);
                m_Pool.Init();
            }


            base.Init();
        }

        public virtual void Update()
        {
            m_Pool.Update();
        }


        public bool Push<TPoolable>(TPoolable poolable)
        where TPoolable : IPoolable
            => Push(poolable);

        public bool Pop<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable
            => Pop(out poolable);

        public bool Peek<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable
            => Peek(out poolable);


        public bool Push(IPoolable poolable)
        {
            poolable.Dispose();
            return m_Pool.Push(poolable);
        }

        public bool Pop(out IPoolable poolable)
        {
            poolable = default(IPoolable);

            if (m_Pool.Pop(out var instance))
            {
                poolable = (IPoolable)instance;
                poolable.Init();
                return true;
            }

            return false;
        }

        public bool Peek(out IPoolable poolable)
        {
            poolable = default(IPoolable);

            if (m_Pool.Peek(out var instance))
            {
                poolable = (IPoolable)instance;
                poolable.Init();
                return true;
            }

            return false;
        }



        public static PoolController Get(params object[] args)
        {
            var poolController = new PoolController();
            poolController.Configure(args);
            poolController.Init();

            return poolController;
        }
    }





    public struct PoolControllerConfig : IConfig
    {
        public GetPoolableDelegate GetPoolableDelegate { get; private set; }

        public PoolControllerConfig(GetPoolableDelegate getPoolable)
        {
            GetPoolableDelegate = getPoolable;
        }
    }





    public interface IPoolController : IController, IUpdateble

    {
        bool Peek<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable;

        bool Pop<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable;

        bool Push<TPoolable>(TPoolable poolable)
        where TPoolable : IPoolable;


        bool Peek(out IPoolable poolable);
        bool Pop(out IPoolable poolable);
        bool Push(IPoolable poolable);


    }

    public interface IPoolController<TPoolable> : IPoolController
    where TPoolable : IPoolable
    {
        bool Peek(out TPoolable poolable);
        bool Pop(out TPoolable poolable);
        bool Push(TPoolable poolable);
    }


}

