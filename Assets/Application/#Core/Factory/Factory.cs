using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{
    public class Factory<T> : FactoryDefault
    where T : IConfigurable
    {
        public T Get(params object[] args)
            => Get<T>(args);


        public void Set(IConstructor constructor)
            => Set<T>(constructor);
    }



    public class FactoryDefault : FactoryModel, IFactory
    {
        public FactoryDefault()
        {
            m_Constractors = new Dictionary<Type, IConstructor>(15);
        }

        public override T Get<T>(params object[] args)
        {
            if (Get<T>(out var constructor))
                return (T)constructor.Create<T>(args);

            return Create<T>(args);
        }

    }


    public abstract class FactoryModel
    {
        protected Dictionary<Type, IConstructor> m_Constractors;

        public abstract T Get<T>(params object[] args)
        where T : IConfigurable;

        protected bool Get<T>(out IConstructor constructor, params object[] args)
            => m_Constractors.TryGetValue(typeof(T), out constructor);

        protected void Set<T>(IConstructor constructor)
            => Set(typeof(T), constructor);

        protected void Set(Type instanceType, IConstructor constructor)
        {
            try { m_Constractors.Add(instanceType, constructor); }
            catch (Exception exeption) { Debug.LogWarning($"The instance constructor is already added! Exeption: {exeption.Message}"); }
        }
        
        
        
        public static T Create<T>(params object[] args)
            => (T)Activator.CreateInstance(typeof(T), args);
    }


    public interface IFactory
    {
        T Get<T>(params object[] args)
        where T : IConfigurable;
    }



    public delegate T PredefinedConstructor<T>(params object[] args);

    public class Constructor : IConstructor
    {
        private PredefinedConstructor<IConfigurable> m_Func;

        public Constructor(PredefinedConstructor<IConfigurable> func)
        {
            m_Func = func;
        }

        
        public T Create<T>(params object[] args)
        where T : IConfigurable
            => (T)m_Func.Invoke(args);


        public static IConstructor Get(PredefinedConstructor<IConfigurable> func)
            => new Constructor(func);
    }   
        
    public interface IConstructor
    {
        T Create<T>(params object[] args)
        where T : IConfigurable;
    }


}