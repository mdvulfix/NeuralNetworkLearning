using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using APP.Factory;

namespace APP
{
    public class Pool<TPoolable> : APool, IPool, IConfigurable<PoolConfig>, IUpdateble
    where TPoolable : IPoolable
    {
        
        private Transform m_Root;
        private int m_Limit = 1;

        private Func<IPoolable> GetPoolable;

        public event Action<IConfigurable> Initialized;
        public event Action<IConfigurable> Disposed;

        public PoolConfig? Config { get; private set; }
        
        public Pool() { }
        public Pool(PoolConfig config, params object[] args)
        {
            Setup(config);
            Configure(args);
        }

        
        public void Setup(PoolConfig config)
        {
            Config = config;
            m_Limit = config.Limit;

            GetPoolable = () => config.GetPoolable();
        }
        
        public override void Configure(params object[] args)
        {
            string log = Config == null ? 
            ("The configuration must be configured! Setup failed!").Send(LogFormat.Warning) : 
            ("Start configuration... ").Send();


            base.Configure(args);
        }

        public bool Push(TPoolable poolable) =>
            Push(poolable);

        public bool Pop(out TPoolable poolable) =>
            Pop(out poolable);

        public bool Peek(out TPoolable poolable) =>
            Peek(out poolable);

    
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



        public static Pool<TPoolable> Get(IFactory<Pool<TPoolable>, PoolConfig> factory,
                                          PoolConfig config,
                                          params object[] arg)
        => factory.Get(config, arg);


    }

    public abstract class PoolModel
    {
        private Stack<IPoolable> m_Poolables;
        
        public int Count => m_Poolables.Count;
        
        public virtual void Configure(params object[] args)
        {
            m_Poolables = new Stack<IPoolable>(100);
        }
        
        public virtual void Init() { PoolUpToLimit(); }
        public virtual void Dispose() { }

        public virtual void Update() { PoolUpToLimit(); }


        public bool Push<TPoolable>(TPoolable poolable)
        where TPoolable : IPoolable
        => Push(poolable);

        public bool Push(IPoolable poolable)
        {
            m_Poolables.Push(poolable);
            return true;
        }

        public bool Pop<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable
        => Pop(out poolable);

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

        public bool Peek<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable
        => Peek(out poolable);

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


    public interface IPool : IEnumerable
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
}