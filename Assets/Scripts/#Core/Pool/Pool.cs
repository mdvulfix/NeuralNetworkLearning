using System.Collections;
using UnityEngine;

namespace APP
{
    public class Pool<TPoolable> : Pool, IPool
    where TPoolable: IPoolable
    {
        private PoolConfig m_Config;

        public Transform Parent {get; private set; }
        
        public Pool() { }
        public Pool(params object[] args) =>
            Configure(args);

    
        public override void Configure(params object[] args)
        {

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg is PoolConfig)
                        m_Config = (PoolConfig) arg;
                        Parent = m_Config.Parent;
                }
            }

            base.Configure(args);
        }
    
    
        public bool Push(TPoolable poolable) =>
            Push(poolable);

        public bool Pop(out TPoolable poolable) =>
            Pop(out poolable);

        public bool Peek(out TPoolable poolable) =>
            Peek(out poolable);

        public static Pool<TPoolable> Get(params object[] arg) =>
            new Pool<TPoolable>(arg);
    }
    
    
    
    
    public abstract class Pool
    {
        private Stack m_Poolables;

        public int Count => m_Poolables.Count;
        

        public virtual void Configure(params object[] args)
        {
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

    }

    public struct PoolConfig : IConfig
    {
        public Transform Parent { get; private set; }

        public PoolConfig(Transform parent)
        {
            Parent = parent;
        }
    }

    public interface IPool: IConfigurable, IEnumerable
    {
        Transform Parent {get; }
        
        int Count {get; }
        
        bool Push(object poolable);
        bool Pop(out object poolable);
        bool Peek(out object poolable);

    }

    public interface IPoolable
    {
        Transform PoolParent {get; }
    }

    

}