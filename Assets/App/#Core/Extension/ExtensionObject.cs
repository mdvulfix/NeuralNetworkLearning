
using System;
using UnityEngine;

namespace APP
{
    public static class ExtensionObject
    {

        public static string GetName(this object instance) =>
            instance.GetType().Name;

        public static T To<T>(this object instance) =>
            (T)instance;

        public static int ToInt(this object instance)
            => instance.To<int>();


        public static IMessage Send(this object instance, bool isDebug = true, LogFormat format = LogFormat.None)
            => Send(instance, instance, isDebug, format);


        public static IMessage Send(this object instance, object sender, bool isDebug = true, LogFormat format = LogFormat.None)
        {
            try { return Messager.Send(isDebug, sender, (string)instance, format); }
            catch (Exception exception)
            {
                Debug.LogWarning($"Send log failed! Exception: {exception.Message}");
                Debug.LogWarning(exception.Message);

                return null;
            }
        }

    }
}
