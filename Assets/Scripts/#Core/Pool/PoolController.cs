using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Pool
{


    public class PoolController<TPoolable> : AController, IPoolController<TPoolable>
    where TPoolable : IPoolable
    {
        private static IPool<TPoolable> m_Pool;

        private IFactory<TPoolable> m_FactoryPoolable;
        private IFactory<TPoolable, TPoolableConfig> m_FactoryPoolable;

        public PoolController() { }
        public PoolController(PoolControllerConfig config, params object[] args)
        {
            Configure(config, args);
            Init();
        }

        public override void Configure(IConfig config, params object[] args)
        {
            var poolControllerConfig = (PoolControllerConfig)config;

            m_FactoryPoolable = poolControllerConfig.PoolableFactory;

            base.Configure();
        }

        public override void Init()
        {
            var limit = 5;
            var poolConfig = new PoolConfig(limit, () => (TPoolable)m_FactoryPoolable.Get());
            

            if (m_Pool == null)
                m_Pool = Pool<TPoolable>.Get(poolConfig);

            base.Init();
        }


        public virtual void Update()
        {
            m_Pool.Update();
        }


        public void Push(TPoolable poolable)
        {
            poolable.Dispose();

            m_Pool.Push(poolable);
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



        // FACTORY //
        public static IPoolController<TPoolable> Get(IFactory<IPoolController<TPoolable>, IConfig> factory, IConfig config, params object[] args)
            => factory.Get(config, args);

        public static IPoolController<TPoolable> Get(IConfig config, params object[] args)
        {
            var poolController = new PoolController<TPoolable>();
            poolController.Configure(config, args);
            poolController.Init();

            return poolController;
        }
    }

    public struct PoolControllerConfig<IPoolable> : IConfig
    {
        public IFactory<IPoolable> PoolableFactory {get; private set; }


        public PoolControllerConfig(IFactory<IPoolable> poolableFactory)
        {
            PoolableFactory = poolableFactory;
        }
    }

    public struct PoolControllerConfig<TPoolable, TPoolableConfig>  : IConfig
    {
        public IFactory<TPoolable, TPoolableConfig> PoolableFactory {get; private set; }


        public PoolControllerConfig(IFactory<TPoolable, TPoolableConfig> poolableFactory)
        {
            PoolableFactory = poolableFactory;
        }
    }



    public interface IPoolController<TPoolable> : IController, IUpdateble
    where TPoolable : IPoolable
    {
        bool Peek(out TPoolable poolable);
        bool Pop(out TPoolable poolable);
        void Push(TPoolable poolable);
    }







}

