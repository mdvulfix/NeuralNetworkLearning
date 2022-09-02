using UnityEngine;
using SceneObject = UnityEngine.GameObject;


namespace APP
{
    public class SceneDefault : AConfigurableOnAwake, IScene
    {




    }

    public interface IScene
    {
        SceneObject SceneObject { get; }
    
    
    }
}

