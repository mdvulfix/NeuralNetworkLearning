

namespace APP
{
    public abstract class ALoadable: AConfigurable
    {


        
        
        public virtual void Load() { }
        public virtual void Unload() { }

    }

    public interface ILoadable
    {
        bool IsLoaded {get; }

        bool Load();
        bool Unload();

        
    }

}

