using System.Collections;

namespace APP
{

    public class Pool : IConfigurable, IPool
    {
        private PoolConfig m_Config;

        private Stack m_Poolables;

        public int Count => m_Poolables.Count;

        public Pool() { }
        public Pool(params object[] args) =>
            Configure(args);

        public virtual void Configure(params object[] args)
        {

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is PoolConfig)
                        m_Config = (PoolConfig) args[0];
                }
            }

            m_Poolables = new Stack(100);
        }

        public virtual void Init() { }
        public virtual void Dispose() { }

        public bool Push(object poolable)
        {
            m_Poolables.Push(poolable);
            return true;
        }

        public bool Pop(out object poolable)
        {
            poolable = null;

            if (m_Poolables.Count > 0)
            {
                poolable = m_Poolables.Pop();
                return true;
            }

            return false;
        }

        public bool Peek(out object poolable)
        {
            poolable = null;

            if (m_Poolables.Count > 0)
            {
                poolable = m_Poolables.Peek();
                return true;
            }

            return false;
        }

        public IEnumerator GetEnumerator() =>
            m_Poolables.GetEnumerator();

        public static Pool Get(params object[] arg) =>
            new Pool(arg);

    }

    public struct PoolConfig : IConfig
    {

    }

    public interface IPool: IEnumerable
    {
        int Count {get; }
        
        bool Push(object poolable);
        bool Pop(out object poolable);
        bool Peek(out object poolable);

    }

    public interface IPoolable
    {

    }

    

}