
using System;
using UnityEngine;

namespace APP
{
    public static class ExtensionObject
    {

        public static string GetName(this object instance) =>
            instance.GetType().Name;


        public static int ToInt(this object instance) =>
            (int)instance;


        public static IMessage Send(this object instance, bool debug = true, LogFormat format = LogFormat.None)
        {
            try { return Messager.Send(debug, instance, (string)instance, format); }
            catch (Exception exception)
            {
                Debug.LogWarning($"Send log failed! Exception: {exception.Message}");
                Debug.LogWarning(exception.Message);

                return null;
            }
        }
    }
}
