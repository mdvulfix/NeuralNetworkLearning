using UnityEngine;

namespace APP
{
    
    public abstract class Controller<TController>: Controller
    where TController: IController, new()
    {
    
        // FACTORY //
        public static TController Get(params object[] args)
        {
            
            var controller = new TController();
            controller.Configure(args);
            
            return controller;
        }
    
    
    
    }
    public abstract class Controller
    {
        public virtual void Configure(params object[] args) { } 
        public virtual void Init() { }
        public virtual void Dispose() { }
    
    }
    

    public interface IController: IConfigurable
    {

    }

}