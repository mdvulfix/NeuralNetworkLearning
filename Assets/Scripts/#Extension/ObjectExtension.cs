using UnityEngine;
using UScene = UnityEngine.SceneManagement.Scene;
using APP;

public static class ObjectExtension
{   
    
    
    
    public static string GetName(this object instance) =>
        instance.GetType().Name;

    public static int ToInt(this object instance) =>
        (int)instance;


}