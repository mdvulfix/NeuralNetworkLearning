using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace APP
{
    public class PoolController<TPoolable> : Controller<PoolController<TPoolable>>, IController, IUpdateble
    where TPoolable : class, IPoolable, IConfigurable
    {
        private PoolControllerConfig m_Config;

        private static Pool<TPoolable> m_Pool;
        private int m_AwaiterPoolLimit = 1;

        public PoolController() { }
        public PoolController(params object[] args) =>
            Configure(args);


        public override void Configure(params object[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is PoolControllerConfig)
                    {
                        m_Config = (PoolControllerConfig)arg;
                    }
                }
            }


            if (m_Pool == null)
                m_Pool = Pool<TPoolable>.Get(new PoolConfig());



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

    public class PoolControllerConfig
    {


    }
}