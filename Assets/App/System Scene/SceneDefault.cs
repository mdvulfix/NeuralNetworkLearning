using System;
using UnityEngine;


namespace APP
{
    [Serializable]
    public class SceneDefault : ModelCacheable, IScene
    {

        
        
        public Transform Scene  => gameObject.transform;

        public bool IsLoaded {get; private set;}

        public void Load()
        {
            
        }
    }

    public interface IScene: IActivable, ILoadable, IComponent
    {
        Transform Scene { get; }
    
    
    }
}

