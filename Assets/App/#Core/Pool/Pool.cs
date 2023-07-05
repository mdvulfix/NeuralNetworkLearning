using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    public class Pool<TPoolable> : PoolDefault, IPool<TPoolable>, IConfigurable, IUpdateble
    where TPoolable : IPoolable
    {

        public Pool() { }
        public Pool(params object[] args)
        {
            Configure(args);
            Init();
        }

        public bool Push(TPoolable poolable)
            => Push<TPoolable>(poolable);

        public bool Pop(out TPoolable poolable)
            => Pop<TPoolable>(out poolable);

        public bool Peek(out TPoolable poolable)
            => Peek<TPoolable>(out poolable);


        public static new Pool<TPoolable> Get(params object[] args)
            => Get<Pool<TPoolable>>(args);

    }


    public class PoolDefault : PoolModel, IPool, IConfigurable, IUpdateble
    {



        private Transform m_Root;
        private int m_Limit = 1;

        private Func<IPoolable> GetPoolable;



        public PoolDefault() { }
        public PoolDefault(params object[] args)
        {
            Configure(args);
            Init();
        }

        public override void Configure(params object[] args)
        {
            var config = (PoolConfig)args[PARAM_INDEX_Config];
            m_Limit = config.Limit;

            GetPoolable = () => config.GetPoolable();
            base.Configure(args);
        }

        public override void Init()
        {
            PoolUpToLimit();
            base.Init();
        }


        public bool Push<TPoolable>(TPoolable poolable)
        where TPoolable : IPoolable
        {
            base.Push(poolable);
            return true;
        }


        public bool Pop<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable
        {
            poolable = default(TPoolable);

            if (base.Pop(out var iPoolable))
            {
                poolable = (TPoolable)iPoolable;
                return true;
            }

            return false;
        }

        public bool Peek<TPoolable>(out TPoolable poolable)
        where TPoolable : IPoolable
        {
            poolable = default(TPoolable);

            if (base.Peek(out var iPoolable))
            {
                poolable = (TPoolable)iPoolable;
                return true;
            }

            return false;
        }


        public virtual void Update()
            => PoolUpToLimit();


        private void PoolUpToLimit()
        {
            if (Count >= 0)
            {
                if (m_Limit > 0 && Count < m_Limit)
                {
                    var upToLimit = m_Limit - Count;
                    for (int i = 0; i < upToLimit; i++)
                        Push(GetPoolable());
                }

                if (m_Limit == 0 && Count == 0)
                    Push(GetPoolable());
            }
        }


        public static PoolDefault Get(params object[] args)
            => Get<PoolDefault>(args);
    }

    public abstract class PoolModel : AConfigurable
    {

        private Stack<IPoolable> m_Poolables = new Stack<IPoolable>(100);

        public int Count => m_Poolables.Count;

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





        public IEnumerator GetEnumerator()
        => m_Poolables.GetEnumerator();



        // FACTORY //
        public static TPool Get<TPool>(params object[] args)
        where TPool : IPool, new()
        {
            var pool = new TPool();
            pool.Configure(args);
            pool.Init();

            return pool;
        }
    }


    public interface IPool<TPoolable> : IPool
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

    }


    public interface IPoolable : IConfigurable
    {

    }

    public delegate IPoolable GetPoolableDelegate();

    public struct PoolConfig : IConfig
    {
        public PoolConfig(int limit, GetPoolableDelegate getPoolable)
        {
            Limit = limit;

            GetPoolable = getPoolable;
        }

        public int Limit { get; private set; }
        public GetPoolableDelegate GetPoolable { get; private set; }
    }

    public partial class PoolFactory<TPoolable> : Factory<IPool>, IFactory
    where TPoolable : IPoolable
    {

        public PoolFactory()
        {
            Set<TPoolable>(Constructor.Get((args) => GetPool(args)));

        }





    }

    public partial class PoolFactory<TPoolable> : Factory<IPool>, IFactory
    where TPoolable : IPoolable
    {
        private Pool<TPoolable> GetPool(params object[] args)
        {
            var instance = new Pool<TPoolable>();
            instance.Configure(args);
            instance.Init();

            return instance;

        }
    }



}