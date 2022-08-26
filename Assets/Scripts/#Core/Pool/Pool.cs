using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    public partial class Pool<TPoolable> : Pool, IPool
    where TPoolable : class, IPoolable, new()
    {
        
        public Pool() { }
        public Pool(params object[] args) =>
            Configure(args);



        public override void Configure(params object[] args)
        {
            //var config = (PoolConfig)args[PoolConfig.Index];

            var config = new PoolConfig();
            var arg = new List<ConfigArgInfo>();
            arg[PoolConfig.Index].Instance = config;


            arg.Index = PoolConfig.Index;
            arg.Instance = config;
            arg.Type = config.GetType();


            base.Configure(arg.Index);



            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is PoolConfig)
                        var config = (PoolConfig)arg;
                    Parent = m_Config.Parent;
                }
            }

            base.Configure(args);
        }

        /*
        public bool Push(TPoolable poolable) =>
            Push(poolable);

        public bool Pop(out TPoolable poolable) =>
            Pop(out poolable);

        public bool Peek(out TPoolable poolable) =>
            Peek(out poolable);

        */

        public static Pool<TPoolable> Get(params object[] arg) =>
            new Pool<TPoolable>(arg);






        public bool Push(TPoolable poolable)
        {
            m_Poolables.Push(poolable);
            return true;
        }

        public bool Pop(out TPoolable poolable)
        {
            poolable = null;

            if (m_Poolables.Count > 0)
            {
                poolable = m_Poolables.Pop();
                return true;
            }

            return false;
        }

        public bool Peek(out TPoolable poolable)
        {
            poolable = null;

            if (m_Poolables.Count > 0)
            {
                poolable = m_Poolables.Peek();
                return true;
            }

            return false;
        }


    }




    public abstract class Pool
    {
        private PoolConfig m_Config;


        private Stack m_Poolables;
        private IFactory m_Factory;
        private Transform m_Parent;

        private int m_Limit;


        public int Count => m_Poolables.Count;

        public IConfig Config => m_Config;

        public virtual void Configure(params object[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is PoolConfig)
                        m_Config = (PoolConfig)arg;
                    m_Parent = m_Config.Parent;
                    m_Limit = m_Config.Limit;
                    m_Factory = m_Config.Factory;
                }
            }



            m_Poolables = new Stack(100);
        }

        public virtual void Init() { }
        public virtual void Dispose() { }



        public bool Push<TPoolable>(TPoolable poolable)
        where TPoolable : class, IPoolable, new()
        => Push(poolable);

        public bool Push(IPoolable poolable)
        {
            m_Poolables.Push(poolable);
            return true;
        }


        public bool Pop<TPoolable>(out TPoolable poolable)
        where TPoolable : class, IPoolable, new()
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


        public bool Peek<TPoolable>(out IPoolable poolable)
        where TPoolable : class, IPoolable, new()
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
                        Create(m_Factory);
                }

                if (m_Limit == 0 && Count == 0)
                    Create(m_Factory); ;
            }
        }


        public void Create(IFactory factory)
        => Push((IPoolable)factory.Get());

        public IEnumerator GetEnumerator()
        => m_Poolables.GetEnumerator();

    }


    public struct PoolConfig : IConfig
    {
        public static int Index => PoolArgsIndex.Config.ToInt();

        public PoolConfig(int limit, Transform parent, IFactory factory)
        {
            Limit = limit;
            Parent = parent;
            Factory = factory;

        }

        public int Limit { get; private set; }
        public Transform Parent { get; private set; }
        public IFactory Factory { get; private set; }

    }

    public enum PoolArgsIndex
    {
        Config
    }


    public interface IPool : IConfigurable, IEnumerable
    {
        int Count { get; }

        bool Push(IPoolable poolable);
        bool Pop(out IPoolable poolable);
        bool Peek(out IPoolable poolable);

        void CheckLimit();
        void Create(IFactory factory);

    }

    public interface IPoolable : IConfigurable
    {
        Transform PoolParent { get; }
    }



}