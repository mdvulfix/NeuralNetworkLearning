using System;

namespace APP
{
    public struct Config: IConfig
    {


        
        public static TConfig Get<TConfig>(params object[] args)
        where TConfig: struct, IConfig
            =>FactoryModel.Create<TConfig>(args);
    
    }
}