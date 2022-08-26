
using System;
using UnityEngine;

namespace APP
{
    public static class ObjectExtension
    {

        public static string GetName(this object instance) =>
            instance.GetType().Name;


        public static int ToInt(this object instance) =>
            (int)instance;


        public static string Send(this object instance, LogFormat format = LogFormat.None)
        {
            try
            {
                var message = (string)instance;
                
                switch (format)
                {
                    default:
                        Debug.Log(message);
                        break;

                    case LogFormat.Warning:
                        Debug.LogWarning(message);
                        break;

                    case LogFormat.Error:
                        Debug.LogError(message);
                        break;
                }

                return message;

            }
            catch (Exception exception)
            {

                Debug.LogWarning($"Send log failed! Exception: {exception.Message}");
                Debug.LogWarning(exception.Message);

                return null;
            }

            
        }
    }

    public enum LogFormat
    {
        None,
        Warning,
        Error
    }

}
