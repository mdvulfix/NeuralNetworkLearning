using System.Collections;
using UnityEngine;

namespace APP
{
    public interface ILoadable
    {
        bool IsLoaded {get; }
        bool IsActive {get; }

        bool Load();
        bool Activate();
        bool Deactivate();
        bool Unload();

        
    }
}