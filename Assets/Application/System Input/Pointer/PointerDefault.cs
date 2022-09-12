using System;
using UnityEngine;

namespace APP.Input
{

    [Serializable]
    public class PointerDefault : PointerModel, IPointer
    {
        private SpriteRenderer m_Renderer;
    
        public static readonly string PREFAB_Label = "Pointer";

        public PointerDefault() { }
        public PointerDefault(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
        
            var obj = gameObject;

            if (obj.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = obj.AddComponent<SpriteRenderer>();
            
        
            var pointer = transform;
            
            pointer.name = "Pointer";
            pointer.localPosition = Vector3.back;
            pointer.localScale = Vector3.one * 3;

            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }
            
            // CONFIGURE BU DEFAULT //
            var position = Vector3.zero;
            var defaultColor = m_Renderer.color;
            var layerMask = 5;
            
            Transform parent = null;
            

            if(Seacher.Find<IScene>(out var scenes))
            {
                parent = scenes[0].Scene != null ?
                scenes[0].Scene :
                pointer.parent;
            }
                
            var config = new PointerConfig(this, defaultColor, layerMask, parent);
            base.Configure(config);
            
            Send($"{ this.GetName() } was configured by default!");
        }
    

        public override void SetColor(Color color)
        {
            m_Renderer.color = color;
        }


    }

    
    public abstract class PointerModel: ModelCacheable
    {
        private static IFactory m_Factory = new FactoryDefault();

        private PointerConfig m_Config;

        private int m_LayerMask;
        
        public IPointer Instance {get; private set; }
        public Vector3 Position { get => transform.position; private set => transform.position = value; }
        
        public Color ColorDefault { get; private set; }
        
        public static readonly string PREFAB_Folder = "Prefab";


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
                       
            m_Config = args.Length > 0 ?
            (PointerConfig)args[PARAMS_Config] :
            default(PointerConfig);
            
            Instance = m_Config.Instance;
            ColorDefault = m_Config.ColorDefault;

            if(m_Config.Parent != null )
                transform.SetParent(m_Config.Parent);
                        
            base.Configure(args);
        }


        public abstract void SetColor(Color color);
        
        public void SetPosition(Vector3 position)
            => Position = position;

        // FACTORY //
        public static TPointer Get<TPointer>(params object[] args)
        where TPointer : IConfigurable
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PointerFactory();
            var instance = factory.Get<TPointer>(args);
            
            return instance;
        }
    }

    public interface IPointer: IConfigurable, IActivable, IMessager
    {
        Vector3 Position { get; }

        void SetPosition(Vector3 position);
        void SetColor(Color color);
    }


    public class PointerConfig
    {
        public PointerConfig(IPointer instance, Color colorDefault, int layerMask, Transform parent = null)
        {
            Instance = instance;
            ColorDefault = colorDefault;
            LayerMask = layerMask;
            Parent = parent;
        }

        public IPointer Instance { get; private set; }
        public Color ColorDefault { get; }
        public int LayerMask { get; }
        public Transform Parent { get; private set; }
    }



    public partial class PointerFactory : Factory<IPointer>
    {
        public PointerFactory()
        {
            Set<PointerDefault>(Constructor.Get((args) => GetPointerDefault(args)));
        }

        private PointerDefault GetPointerDefault(params object[] args)
        {       
            var prefabPath = $"{PointerModel.PREFAB_Folder}/{PointerDefault.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Pointer");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<PointerDefault>();
            

            //var instance = new Pixel3D();

            if (args.Length > 0)
            {
                var config = (PointerConfig)args[PointerModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }


}