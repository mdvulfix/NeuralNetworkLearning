
using UnityEngine;

namespace APP
{
    public abstract class ModelController: ModelConfigurable
    {
        
        
        
        // FACTORY //
        public static TController Get<TController>(params object[] args)
        where TController: IController
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new FactoryDefault();
            var instance = factory.Get<TController>(args);
            
            return instance;
        }
    }
    
    
    
    public interface IController: IConfigurable
    {

    }

}