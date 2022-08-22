namespace APP
{
    public abstract class Controller : IController
    {
        public virtual void Configure(params object[] args) { } 
        public virtual void Init() { }
        public virtual void Dispose() { }



    }


    public interface IController: IConfigurable
    {

    }

}