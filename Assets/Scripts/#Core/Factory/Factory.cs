
namespace APP.Factory
{
    public class Factory<T, TConfig>: AFactory, IFactory<T, TConfig>
    where T: class, IConfigurable<TConfig>, new()
    where TConfig: struct, IConfig
    {
        public T Get(TConfig config, params object[] args) 
        {
            var instance = new T();
            instance.Setup(config);
            instance.Configure(args);

            return instance;
        }
    }

    public class Factory<T>: AFactory, IFactory<T>
    where T: class, IConfigurable, new()
    {
        
        public T Get(params object[] args) 
        {
            var instance = new T();
            instance.Configure(args);

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

