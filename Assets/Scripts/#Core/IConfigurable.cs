using System;

namespace APP
{
    public interface IConfigurable: IDisposable
    {
        IConfig Config {get; }

        void Configure(params object[] args);
        void Init();
    }
}