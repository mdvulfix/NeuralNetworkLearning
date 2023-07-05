using System;
using System.Collections;
using UnityEngine;

namespace APP.Pool
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


    public interface IPool<TPoolable> : IPool
    {
        bool Push(TPoolable poolable);
        bool Pop(out TPoolable poolable);
        bool Peek(out TPoolable poolable);

    }


    public partial class PoolFactory : Factory<IPool>, IFactory
    {
        private PoolDefault GetPoolDefault(params object[] args)
        {

            var instance = new PoolDefault();

            if (args.Length > 0)
            {
                var config = (PoolConfig)args[PoolModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }

        private Pool<TPoolable> GetPool<TPoolable>(params object[] args)
        where TPoolable : IPoolable
        {
            var instance = new Pool<TPoolable>();
            if (args.Length > 0)
            {
                var config = (PoolConfig)args[PoolModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }



    }


    public partial class PoolFactory<TPoolable> : Factory<IPool>, IFactory
    where TPoolable : IPoolable
    {

        private Pool<TPoolable> GetPool(params object[] args)
        {
            var instance = new Pool<TPoolable>();
            if (args.Length > 0)
            {
                var config = (PoolConfig)args[PoolModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }



    }

}