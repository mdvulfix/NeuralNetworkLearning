using System;

namespace APP
{

    public struct ConfigArgInfo
    {
        public ConfigArgInfo(int index, object instance)
        {
            Index = index;
            Instance = instance;
            Type = instance.GetType();
        }

        public int Index { get; set; }
        public object Instance { get; set; }
        public Type Type { get; set; }

    }
}