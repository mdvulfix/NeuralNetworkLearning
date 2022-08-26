using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using APP.Factory;

namespace APP.Pool
{
    public class PoolController<TPoolable>: AController<PoolController<TPoolable>, PoolControllerConfig>, IController, IUpdateble
    where TPoolable: IPoolable
    {
        private static Pool<TPoolable> m_Pool;
        private int m_AwaiterPoolLimit = 1;

        public PoolController() { }
        public PoolController(PoolControllerConfig config, params object[] args)
        {
            Setup(config);
            Configure(args);
            Init();
        }

        public override void Setup(PoolControllerConfig config)
        {
            Config = config;

            
            
            

        }
        
        public override void Configure(params object[] args)
        {
        
            var poolFactory = AFactory.Get<Factory<Pool<TPoolable, PoolConfig>>>();
            var poolConfig = new PoolConfig();

            
            if (m_Pool == null)
                m_Pool = Pool<TPoolable>.Get(poolFactory, poolConfig);

            base.Configure(m_Config);
        }

        public override void Init()
        {
            PoolUpdate();

            base.Init();
        }

        public override void Dispose()
        {

            base.Dispose();
        }

        
        public void Update()
        {
            PoolUpdate();
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









        private void PoolUpdate()
        {
            PoolCheckLimit();
        }


    }

    public struct PoolControllerConfig: IConfig
    {


    }
}