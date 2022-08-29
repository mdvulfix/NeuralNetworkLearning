
using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{

    public interface IFactory
    {
        T Get<T, TConfig>(TConfig config, params object[] args);
        T Get<T>(params object[] args);

    }

    public abstract class AFactory<T, TConfig> : AFactory<T>
    {

        public override void Set(PredefinedConstructor constructor)
            => base.Set(constructor);


        public T Get(TConfig config, params object[] args)
            => base.Get(config, args);
    }

    public abstract class AFactory<T>
    {
        private static Dictionary<Type, PredefinedConstructor> m_PredefinedConstructors;

        public delegate T PredefinedConstructor(params object[] args);

        public virtual void Set(PredefinedConstructor constructor) =>
            m_PredefinedConstructors.Add(typeof(T), constructor);


        public T Get(params object[] args)
        {
            if (m_PredefinedConstructors.TryGetValue(typeof(T), out var constructor))
            {
                IConfig config;

                foreach (var arg in args)
                {
                    if (arg is IConfig)
                    {
                        config = (IConfig)arg;
                        return constructor(config, args);
                    }
                }
            }
            

            return Constructor.Create<T>(args);
        }
    }


    public static class Constructor
    {
        public static T Create<T>(params object[] args)
        {
            IConfig config;

            foreach (var arg in args)
            {
                if (arg is IConfig)
                {
                    config = (IConfig)arg;
                    return (T)Activator.CreateInstance(typeof(T), config, args);
                }
            }

            return (T)Activator.CreateInstance(typeof(T), args);

        }
    }
















}

