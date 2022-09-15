using UnityEngine;


namespace APP
{
    public class SceneDefault : ModelCacheable, IScene
    {
        public Transform Scene  => gameObject.transform;

        public bool IsLoaded {get; private set;}

        public void Load()
        {
            
        }
    }

    public interface IScene: IActivable
    {
        Transform Scene { get; }
    
    
    }
}

