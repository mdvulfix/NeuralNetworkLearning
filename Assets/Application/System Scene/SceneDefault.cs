using UnityEngine;


namespace APP
{
    public class SceneDefault : AConfigurableOnAwake, IScene
    {
        public Transform Scene  => gameObject.transform;


    }

    public interface IScene
    {
        Transform Scene { get; }
    
    
    }
}

