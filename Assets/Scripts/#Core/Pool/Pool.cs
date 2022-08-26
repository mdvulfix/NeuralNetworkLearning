using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using APP.Factory;

namespace APP
{
    public class Pool<TPoolable> : Pool, IPool
    where TPoolable : IPoolable
    {
        public Pool() { }
        public Pool(PoolConfig config, params object[] args)
        {
            Setup(config);
            Configure(args);
            Init();
        }

        public bool Push(TPoolable poolable) =>
            Push<TPoolable>(poolable);

        public bool Pop(out TPoolable poolable) =>
            Pop<TPoolable>(out poolable);

        public bool Peek(out TPoolable poolable) =>
            Peek<TPoolable>(out poolable);

    
        public static Pool<TPoolable> Get(IFactory<Pool<TPoolable>, PoolConfig> factory,
                                          PoolConfig config,
                                          params object[] arg)
        => factory.Get(config, arg);
    
    }




    public abstract class Pool : IPool
    {
        private Stack m_Poolables;
        private Transform m_Parent;

        private int m_Limit;


        public int Count => m_Poolables.Count;

        public PoolConfig? Config { get; private set; }

        private GetPoolableDel GetPoolableDel;
        
        public virtual void Setup(PoolConfig config)
        {
            Config = config;
            m_Parent = config.Parent;
            m_Limit = config.Limit;
            
            GetPoolableDel = config.GetPoolableDel;
        }

        public virtual void Configure(params object[] args)
        {
            string log = Config == null ? 
            ("The configuration must be configured! Setup failed!").Send(LogFormat.Warning) : 
            ("Start configuration... ").Send();
            

            m_Poolables = new Stack(100);

        }
        
        public virtual void Init() { }
        public virtual void Dispose() { }



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
                poolable = (IPoolable)m_Poolables.Pop();
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
                poolable = (IPoolable)m_Poolables.Peek();
                return true;
            }

            return false;
        }


        public void CheckLimit()
        {
            if (Count >= 0)
            {
                if (m_Limit > 0 && Count < m_Limit)
                {
                    var upToLimit = m_Limit - Count;
                    for (int i = 0; i < upToLimit; i++)
                        GetPoolable();
                }

                if (m_Limit == 0 && Count == 0)
                    GetPoolable();
            }
        }


        public IPoolable GetPoolable() 
        => GetPoolableDel();
        
        public IEnumerator GetEnumerator()
        => m_Poolables.GetEnumerator();
    }


    public interface IPool : IConfigurable, IEnumerable
    {
        int Count { get; }

        bool Push(IPoolable poolable);
        bool Pop(out IPoolable poolable);
        bool Peek(out IPoolable poolable);

        void CheckLimit();
        
        IPoolable GetPoolable();
    }

    public interface IPoolable : IConfigurable
    {
        Transform PoolParent { get; }
    }

    
    
    
    public struct PoolConfig : IConfig
    {
        public PoolConfig(int limit, Transform parent, IFactory factory, GetPoolableDel getPoolable)
        {
            Limit = limit;
            Parent = parent;
            Factory = factory;
            GetPoolableDel = getPoolable;
        }

        public int Limit { get; private set; }
        public Transform Parent { get; private set; }
        public IFactory Factory { get; private set; }
        public GetPoolableDel GetPoolableDel{ get; private set; }
    }

    public delegate IPoolable DelegateGetPoolable (IFactory<IPoolable, IConfig> factory,
                                                   IConfig config,
                                                   params object[] args);

    public delegate IPoolable GetPoolableDel ();

}