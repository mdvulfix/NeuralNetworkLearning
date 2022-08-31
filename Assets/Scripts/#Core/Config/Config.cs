using System;

namespace APP
{
    public struct Config: IConfig
    {
        
        
        
        public static Config Get(params object[] args)
            => Get<Config>(args);
 
 
        public static TConfig Get<TConfig>(params object[] args)
        where TConfig: struct, IConfig
            =>Factory.Create<TConfig>(args);
    
    }
}