
using UnityEngine;

namespace APP.Factory
{
    public class Factory<TConfigurable, TConfig>: AFactory, IFactory<TConfigurable, TConfig>
    where TConfigurable: Component, IConfigurable<TConfig>, new()
    where TConfig: struct, IConfig
    {
        public TConfigurable Get(TConfig config, params object[] args) 
        {
            if (typeof(TConfigurable) is IScenable)
            { 
                var name = (string) args[0];
                var obj = new GameObject(name);
                obj.SetActive(false);
            
                var configurable = obj.AddComponent<TConfigurable>();

                if (configurable is IPoolable)
                { 
                    var rootPool = (Transform) args[1];
                    obj.transform.SetParent(rootPool);
                }
                else
                { 
                    var root = (Transform) args[2];
                    obj.transform.SetParent(root);
                }
                

                obj.transform.position = Vector3.zero;
                obj.name += configurable.GetHashCode();
                return configurable;
            
            }

            var instance = new TConfigurable();
            instance.Configure(config, args);
            instance.Init();

            return instance;
        }
    }

    public class Factory<TConfigurable>: AFactory, IFactory<TConfigurable>
    where TConfigurable: class, IConfigurable, new()
    {
        
        public TConfigurable Get(params object[] args) 
        {
            
            if (typeof(TConfigurable) is IScenable)
            { 
                var name = (string) args[0];
                var obj = new GameObject(name);
                obj.SetActive(false);
            
                var configurable = obj.AddComponent<TConfigurable>();

                if (configurable is IPoolable)
                { 
                    var rootPool = (Transform) args[1];
                    obj.transform.SetParent(rootPool);
                }
                else
                { 
                    var root = (Transform) args[2];
                    obj.transform.SetParent(root);
                }
                

                obj.transform.position = Vector3.zero;
                obj.name += configurable.GetHashCode();
                return configurable;
            
            }
            
            
            
            
            
            
            
            var instance = new TConfigurable();
            instance.Configure(args);
            instance.Init();

            return instance;
        }
    }

    public abstract class AFactory: IFactory
    {
        // FACTORY //
        public static TFactory Get<TFactory>()
        where TFactory: IFactory, new()
        {
            var instance = new TFactory();
            return instance;
        }
    }

}

namespace APP
{
    
    public interface IFactory<T, TConfig>
    {
        T Get(TConfig config, params object[] args);
    }
    
    public interface IFactory<T>
    {
        T Get(params object[] args);
    }
    
    public interface IFactory
    {

    }
}

