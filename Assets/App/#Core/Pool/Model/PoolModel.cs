using System.Collections;
using System.Collections.Generic;

namespace APP
{
    public abstract class PoolModel : AConfigurable
    {

        private Stack<IPoolable> m_Poolables = new Stack<IPoolable>(100);


        public int Count => m_Poolables.Count;

        public static int PARAMS_Config = 0;

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

    public interface IPool : IEnumerable, IConfigurable
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

}