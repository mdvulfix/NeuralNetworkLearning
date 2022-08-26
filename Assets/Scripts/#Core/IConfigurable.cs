using System;

namespace APP
{


    public interface IConfigurable<TConfig>: IConfigurable
    where TConfig: struct, IConfig
    {
        TConfig Config {get; }

        void Setup(TConfig config);
    }
    
    public interface IConfigurable: IDisposable
    {
        void Configure(params object[] args);
        void Init();
    }

}