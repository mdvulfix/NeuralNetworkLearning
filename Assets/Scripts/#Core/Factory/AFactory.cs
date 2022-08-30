using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    public class Factory<T> : Factory
    where T : IConfigurable
    {
        public T Get(params object[] args)
            => Get<T>(args);


        public void Set(IConstructor constructor)
            => Set<T>(constructor);

    }


    public class Factory : IFactory
    {
        private Dictionary<Type, IConstructor> m_Constractors;

        public Factory()
        {
            m_Constractors = new Dictionary<Type, IConstructor>(15);
        }

        public T Get<T>(params object[] args)
        where T : IConfigurable
        {
            if (Get<T>(out var constructor))
            {
                var instance = constructor.Create<T>(args);
                return (T)instance;
            }

            return default(T);
        }

        protected void Set<T>(IConstructor constructor)
        {
            //m_Constractors.Add(typeof(T), constructor);
            
            try { m_Constractors.Add(typeof(T), constructor); }
            catch (Exception exeption) { Debug.LogWarning($"The instance constructor is already added! Exeption: { exeption.Message }"); }

        }


        protected bool Get<T>(out IConstructor constructor)
            => m_Constractors.TryGetValue(typeof(T), out constructor);
    }



    public interface IFactory
    {
        T Get<T>(params object[] args)
        where T : IConfigurable;
    }



    public delegate T GetInstanceDelegate<T>(params object[] args);

    public class Constructor : IConstructor
    {
        private GetInstanceDelegate<IConfigurable> m_Func;

        public Constructor(GetInstanceDelegate<IConfigurable> func)
        {
            m_Func = func;
        }

        public T Create<T>(params object[] args)
        where T : IConfigurable
            => (T)m_Func.Invoke(args);


        public static IConstructor Get(GetInstanceDelegate<IConfigurable> func)
            => new Constructor(func);
    }

    public interface IConstructor
    {
        T Create<T>(params object[] args)
        where T : IConfigurable;
    }


}