

namespace APP
{
    public abstract class ACacheable: AConfigurable
    {
        public ICache Cache { get; private set; }


        public override void Configure(params object[] args)
        {
            
            
            
            
            base.Configure(args);
        }



        public virtual void Record() { }
        public virtual void Clear() { }


        private void OnInitialized() =>
            Record();

        private void OnDisposed() =>
            Clear();


    }

    public interface ICacheable
    {
        ICache Cache { get; }
    }

    public interface ICache
    {

    }


}

