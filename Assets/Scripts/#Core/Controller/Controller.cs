using APP.Factory;

namespace APP
{
    public abstract class AController<TController, TConfig>: AController, IConfigurable<TConfig>
    where TController: IController
    where TConfig: struct, IConfig
    {
        public TConfig Config {get; protected set; }

        public abstract void Setup(TConfig config);

        // FACTORY //
        public static TController Get(IFactory<TController, TConfig> factory, TConfig config, params object[] arg)
        => Get<TController, TConfig>(factory, config, arg);

    }
    
    
    public abstract class AController<TController>: AController
    where TController: IController
    {
        // FACTORY //
        public static TController Get(IFactory<TController> factory, params object[] arg)
        => Get<TController>(factory, arg);
    }

    
    public abstract class AController: IConfigurable
    {
        public virtual void Configure(params object[] args) { }
        public virtual void Init() { }
        public virtual void Dispose() { }
    
        // FACTORY //
        public static TController Get<TController>(IFactory<TController> factory, params object[] arg)
        => factory.Get(arg);
    
        public static TController Get<TController, TConfig>(IFactory<TController, TConfig> factory, TConfig config, params object[] arg)
        => factory.Get(config, arg);
    }
    
    public interface IController
    {

    }

}