using System;
using System.Linq;
using UnityEngine;



namespace APP
{
    public static class Seacher
    {
        public static bool Find<T>(out T[] instances)
        where T : class
        {
            instances = default(T[]);
            
            var includeInactive = true;
            var components = GameObject.FindObjectsOfType<Component>(includeInactive);

            instances = (from Component instance in components
                         where instance is T
                         select instance as T).ToArray();

            if (instances.Length > 0)
                return true;


            return false;
        }
    }
}