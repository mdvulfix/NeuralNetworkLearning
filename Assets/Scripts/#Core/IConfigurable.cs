using System;

namespace APP
{
    public interface IConfigurable: IDisposable
    {
        void Configure(params object[] args);
        void Init();
    }
}