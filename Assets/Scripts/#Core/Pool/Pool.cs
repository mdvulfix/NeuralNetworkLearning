using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    public class Pool<TPoolable> : APool, IPool<TPoolable>, IConfigurable, IUpdateble
    where TPoolable : IPoolable
    {
        private Transform m_Root;
        private int m_Limit = 1;

        private Func<IPoolable> GetPoolable;
        
        public Pool() { }
        public Pool(PoolConfig config, params object[] args)
        {
            Configure(config, args);
            Init();
        }

        public override void Configure(IConfig config, params object[] args)
        {
            var poolConfig = (PoolConfig)config;
            m_Limit = poolConfig.Limit;

            GetPoolable = () => poolConfig.GetPoolable();
            
            base.Configure(args);
        }



        public bool Push(TPoolable poolable)
        => Push(poolable);
        
        public bool Pop(out TPoolable poolable)
        => Pop(out poolable);
        
        public bool Peek(out TPoolable poolable)
        => Peek(out poolable);

       
        public override void PoolUpToLimit()
        {
            if (Count >= 0)
            {
                if (m_Limit > 0 && Count < m_Limit)
                {
                    var upToLimit = m_Limit - Count;
                    for (int i = 0; i < upToLimit; i++)
                        SetPoolable();
                }

                if (m_Limit == 0 && Count == 0)
                    SetPoolable();
            }
        }

        public override IPoolable SetPoolable()
        => GetPoolable();


        // FACTORY //
        public static IPool<TPoolable> Get(IFactory<IPool<TPoolable>, IConfig> factory, IConfig config, params object[] args)
            => factory.Get(config, args);


        public static IPool<TPoolable> Get(IConfig config, params object[] args)
        {
            var pool = new Pool<TPoolable>();
            pool.Configure(config, args);
            pool.Init();

            return pool;
        }
    
    
    
    
    
    }

    public abstract class APool: AConfigurable
    {
        private Stack<IPoolable> m_Poolables;
        
        public int Count => m_Poolables.Count;
        
        public override void Configure(params object[] args)
        {
            m_Poolables = new Stack<IPoolable>(100);
            base.Configure(args);
        }
        
        public override void Init() 
        {
            PoolUpToLimit(); 
            base.Init();
        }
        

        public virtual void Update() 
        { 
            PoolUpToLimit(); 
        }



        public bool Push(IPoolable poolable)
        {
            m_Poolables.Push(poolable);
            return true;
        }

        public bool Pop(out IPoolable poolable)
        {
            poolable = null;

            if (m_Poolables.Count > 0)
            {
                poolable = m_Poolables.Pop();
                return true;
            }

            return false;
        }

        public bool Peek(out IPoolable poolable)
        {
            poolable = null;

            if (m_Poolables.Count > 0)
            {
                poolable = m_Poolables.Peek();
                return true;
            }

            return false;
        }


        public abstract IPoolable SetPoolable();
        public abstract void PoolUpToLimit();

        public IEnumerator GetEnumerator()
        => m_Poolables.GetEnumerator();
    }


    public interface IPool<TPoolable>: IPool
    {
        bool Push(TPoolable poolable);
        bool Pop(out TPoolable poolable);
        bool Peek(out TPoolable poolable);

    }

    public interface IPool : IEnumerable, IConfigurable, IUpdateble
    {
        int Count { get; }

        bool Push(IPoolable poolable);
        bool Pop(out IPoolable poolable);
        bool Peek(out IPoolable poolable);

        IPoolable SetPoolable();
        void PoolUpToLimit();

    }


    public interface IPoolable : IConfigurable
    {

    }

    public delegate IPoolable GetPoolableDelegate ();

    public struct PoolConfig : IConfig
    {
        public PoolConfig(int limit, GetPoolableDelegate getPoolable)
        {
            Limit = limit;

            GetPoolable = getPoolable;
        }

        public int Limit { get; private set; }
        public GetPoolableDelegate GetPoolable{ get; private set; }
    }


    public class FactoryPool<TPoolable> : IFactory<Pool<TPoolable>, PoolConfig>
    where TPoolable: IPoolable
    {
        public Pool<TPoolable> Get(PoolConfig config, params object[] args)
        {
            var instance =  new Pool<TPoolable>();
            instance.Configure(config, args);
            instance.Init();

            return instance;
        }

    }


}