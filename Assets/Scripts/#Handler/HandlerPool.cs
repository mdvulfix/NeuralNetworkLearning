namespace APP
{
    public static class HandlerPool<TPoolable> where TPoolable : class
    {
        private static Pool m_Pool = Pool.Get(new PoolConfig());

        public static bool Push(TPoolable poolable) =>
            m_Pool.Push(poolable);

        public static bool Pop(out TPoolable poolable)
        {
            poolable = null;

            if (m_Pool.Pop(out var instance))
            {
                if (instance is TPoolable)
                {
                    poolable = (TPoolable) instance;
                    return true;
                }

            }

            return false;
        }

        public static bool Peek(out TPoolable poolable)
        {
            poolable = null;

            if (m_Pool.Peek(out var instance))
            {
                if (instance is TPoolable)
                {
                    poolable = (TPoolable) instance;
                    return true;
                }

            }

            return false;
        }

    }

}