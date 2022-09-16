using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PictureModel : ModelCacheable
    {
        private PictureConfig m_Config;
        
    
        private IPixel[,] m_Matrix;
        private List<IPixel> m_Pixels;

        public IPicture Instance {get; private set; }
        public int Width {get; private set; }
        public int Height {get; private set; }
        public int LayerMask {get => gameObject.layer; private set => gameObject.layer = value; }
        public Color ColorBackground {get; private set; }
        public Color ColorHover {get; private set; }

        public static readonly string PREFAB_Folder = "Prefab";

        // CONFIGURE //
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (PictureConfig)args[PARAMS_Config] :
            default(PictureConfig);

            Instance = m_Config.Instance;
            Width = m_Config.Width;
            Height = m_Config.Height;
            ColorBackground = m_Config.BackgroundColor;
            ColorHover = m_Config.HoverColor;
            LayerMask = m_Config.LayerMask;

            if(m_Config.Parent != null)
                transform.SetParent(m_Config.Parent);
            
            base.Configure(args);
        }

        public override void Init()
        {
            if(VerifyOnInit())
                return;
            
            m_Matrix = new IPixel[Width, Height];
            m_Pixels = new List<IPixel>();
                        
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new Vector3(x - Width / 2, y - Height / 2);
                    
                    var pixel = GetPixel(position);
                    pixel.Init();

                    m_Pixels.Add(m_Matrix[x, y] = pixel);
                }
            }

            
            base.Init();
        }

        public override void Dispose()
        {
            foreach (var pixel in m_Pixels)
                pixel.Dispose();

            m_Pixels.Clear();

            base.Dispose();
        }


        public override void Activate()
        {
            foreach (var pixel in m_Pixels)
                pixel.Activate(); 

            base.Activate();  
        }

        public override void Deactivate()
        {
            foreach (var pixel in m_Pixels)
                pixel.Deactivate(); 

            base.Deactivate();  
        }

        
        

        
        public IEnumerable<ISensible> GetSensibles() =>
            m_Pixels;

        
        
        protected abstract IPixel GetPixel(Vector3 position);
        
        
        
        // FACTORY //
        public static TPicture Get<TPicture>(params object[] args)
        where TPicture: IPicture
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PictureFactory();
            var instance = factory.Get<TPicture>(args);
            
            return instance;
        }
    }

    public struct PictureConfig
    {
        public PictureConfig(IPicture instance, int width, int height, Color backgroundColor, Color hoverColor, int layerMask, Transform parent)
        {
            Instance = instance;
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            LayerMask = layerMask;
            Parent = parent;
            
        }

        public IPicture Instance { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
        public int LayerMask { get; }
        public Transform Parent { get; internal set; }
        
    }

    public interface IPicture: IConfigurable, ICacheable, IActivable, IComponent, IMessager, IRecognizable
    {

    }

    public partial class PictureFactory : Factory<IPicture>
    {
        private string m_Label = "Picture";
        
        public PictureFactory()
        {
            Set<Picture2D>(Constructor.Get((args) => GetPicture2D(args)));
            Set<Picture3D>(Constructor.Get((args) => GetPicture3D(args)));
        }
    }
}